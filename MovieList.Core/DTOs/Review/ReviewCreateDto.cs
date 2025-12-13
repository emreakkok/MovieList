using System.ComponentModel.DataAnnotations;

namespace MovieList.Core.DTOs.Review
{
    public class ReviewCreateDto
    {
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Yorum içeriği gereklidir")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Yorum en az 3, en fazla 1000 karakter olmalıdır")]
        public string Content { get; set; } = string.Empty;
    }
}