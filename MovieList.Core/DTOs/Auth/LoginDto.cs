using System.ComponentModel.DataAnnotations;

namespace MovieList.Core.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email veya kullanıcı adı gerekli")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gerekli")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}