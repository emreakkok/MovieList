// MovieList.Core/DTOs/Movie/MovieDetailDto.cs
using MovieList.Core.DTOs.Review;

namespace MovieList.Core.DTOs.Movie
{
    // MovieDto'dan miras alıyoruz, böylece Title, FullPosterUrl vb. hepsi buraya gelir.
    public class MovieDetailDto : MovieDto
    {
        public string? BackdropPath { get; set; } // MovieDto'da olmayan ek özellik

        // Film Yorumları
        public List<ReviewDto> Reviews { get; set; } = new();

        // Kullanıcıya özel alanlar
        public bool IsWatched { get; set; }
        public int? UserRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}