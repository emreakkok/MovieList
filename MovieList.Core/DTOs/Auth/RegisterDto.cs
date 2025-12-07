using System.ComponentModel.DataAnnotations;

namespace MovieList.Core.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arası olmalı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email gerekli")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gerekli")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı gerekli")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}