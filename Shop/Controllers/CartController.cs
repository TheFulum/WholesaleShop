using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string? returnUrl = null)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.StockQuantity <= 0)
                return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1
                });
            }

            SaveCart(cart);
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string action)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (action == "increase")
                    item.Quantity++;
                else if (action == "decrease")
                {
                    item.Quantity--;
                    if (item.Quantity <= 0)
                        cart.Remove(item);
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // AJAX: установить конкретное количество товара
        [HttpPost]
        public IActionResult SetQuantity(int productId, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return Json(new { success = false, error = "Товар не найден" });

            // Клампим к допустимому диапазону
            quantity = Math.Max(1, Math.Min(quantity, product.StockQuantity));

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
                item.Quantity = quantity;

            SaveCart(cart);

            // Считаем новую сумму по всей корзине
            decimal cartTotal = cart.Sum(ci => ci.Product.Price * ci.Quantity);

            return Json(new
            {
                success = true,
                quantity,
                itemTotal = Math.Round(product.Price * quantity, 2),
                cartTotal = Math.Round(cartTotal, 2),
                stockQuantity = product.StockQuantity
            });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cart ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }
    }
}