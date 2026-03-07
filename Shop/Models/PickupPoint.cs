using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class PickupPoint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле!")]
        [StringLength(100)]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Обязательное поле!")]
        [StringLength(200)]
        [Display(Name = "Адрес")]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Часы работы")]
        public string? WorkingHours { get; set; }

        [StringLength(20)]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}