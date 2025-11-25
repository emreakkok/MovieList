using MovieList.Core.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.Movie
{
    public class MovieDetailDto
    {
        // Temel film bilgileri
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Runtime { get; set; }
        public decimal? VoteAverage { get; set; }
        public int WatchCount { get; set; }
        public int LikeCount { get; set; }

        // Film Yorumları
        public List<ReviewDto> Reviews { get; set; } = new();

        // Kullanıcıya özel alanlar
        public bool IsWatched { get; set; }
        public int? UserRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}
