using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public string Product { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Новый";

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Тип доставки: "Доставка" или "Самовывоз"
        [Required]
        [StringLength(20)]
        public string DeliveryType { get; set; } = "Доставка";

        // Ссылка на пункт выдачи (только для самовывоза)
        public int? PickupPointId { get; set; }

        [ForeignKey("PickupPointId")]
        public PickupPoint? PickupPoint { get; set; }
    }
}