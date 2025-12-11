using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.User
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // Favori filmler
        public List<int> FavoriteMovieIds { get; set; } = new();
    }
}
