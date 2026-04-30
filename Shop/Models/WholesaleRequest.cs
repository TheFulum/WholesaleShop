using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class WholesaleRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Укажите название компании")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Название компании должно быть от 2 до 150 символов")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите ИНН/УНП")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "ИНН/УНП должен содержать 9 цифр")]
        public string TaxId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите контактное лицо")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Контактное лицо должно быть от 2 до 100 символов")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [RegularExpression(@"^\+375\((29|33|44|25)\)\d{3}-\d{2}-\d{2}$", ErrorMessage = "Формат телефона: +375(29/33/44/25)XXX-XX-XX")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        [StringLength(100)]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите юридический адрес")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Юридический адрес должен быть от 5 до 250 символов")]
        public string LegalAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Comment { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        [StringLength(300)]
        public string? ReviewComment { get; set; }

        public User? User { get; set; }
    }
}
