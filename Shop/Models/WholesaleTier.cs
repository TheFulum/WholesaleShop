using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    public class WholesaleTier
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Минимальное количество должно быть больше 0")]
        [Display(Name = "Мин. количество")]
        public int MinQuantity { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Скидка должна быть от 1 до 99%")]
        [Display(Name = "Скидка (%)")]
        public int DiscountPercent { get; set; }
    }
}