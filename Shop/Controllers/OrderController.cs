using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Helpers;
using System.Text.Json;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(
        string cartData,
        string? Comment,
        string DeliveryType,
        string? CustomerAddress,
        int? PickupPointId)
    {
        var userLogin = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

        if (user == null)
        {
            TempData["Error"] = "Пользователь не найден.";
            return RedirectToAction("Index", "Cart");
        }

        var cart = JsonSerializer.Deserialize<List<CartItem>>(cartData);

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Корзина пуста.";
            return RedirectToAction("Index", "Cart");
        }

        // Определяем адрес
        string finalAddress;
        if (DeliveryType == "Самовывоз")
        {
            if (PickupPointId == null)
            {
                TempData["Error"] = "Выберите пункт самовывоза.";
                return RedirectToAction("Index", "Cart");
            }
            var pickupPoint = _context.PickupPoints.Find(PickupPointId);
            if (pickupPoint == null)
            {
                TempData["Error"] = "Пункт самовывоза не найден.";
                return RedirectToAction("Index", "Cart");
            }
            finalAddress = pickupPoint.Address;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(CustomerAddress))
            {
                TempData["Error"] = "Укажите адрес доставки.";
                return RedirectToAction("Index", "Cart");
            }
            finalAddress = CustomerAddress;
        }

        // Списываем товары со склада и считаем итоговую цену
        decimal recalculatedTotal = 0;

        foreach (var item in cart)
        {
            var product = _context.Products
                .Include(p => p.WholesaleTiers)
                .FirstOrDefault(p => p.Id == item.Product.Id);

            if (product == null || product.StockQuantity < item.Quantity)
            {
                TempData["Error"] = $"Недостаточно товара на складе: {item.Product.Name}";
                return RedirectToAction("Index", "Cart");
            }

            decimal unitPrice = user.IsWholesale
                ? WholesalePriceHelper.GetDiscountedPrice(product.Price, item.Quantity, product.WholesaleTiers)
                : product.Price;

            recalculatedTotal += unitPrice * item.Quantity;

            product.StockQuantity -= item.Quantity;
            _context.Update(product);
        }

        var productNames = string.Join(", ", cart.Select(c => $"{c.Product.Name} (x{c.Quantity})"));
        var totalQuantity = cart.Sum(c => c.Quantity);

        var order = new Order
        {
            UserId = user.Id,
            Product = productNames,
            Quantity = totalQuantity,
            Price = recalculatedTotal,
            CustomerAddress = finalAddress,
            Comment = Comment,
            Status = "Новый",
            OrderDate = DateTime.Now,
            DeliveryType = DeliveryType,
            PickupPointId = DeliveryType == "Самовывоз" ? PickupPointId : null
        };

        try
        {
            _context.Order.Add(order);
            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Заказ успешно оформлен!";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Произошла ошибка при оформлении заказа: {ex.Message}";
            return RedirectToAction("Index", "Cart");
        }
    }
}