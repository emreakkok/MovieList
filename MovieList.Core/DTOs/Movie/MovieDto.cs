using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.Movie
{
    public class MovieDto
    {
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

        public int? UserRating { get; set; }

        public decimal? UserAverageRating { get; set; }

        public string FullPosterUrl
        {
            get
            {
                if (string.IsNullOrEmpty(PosterPath))
                {
                    return "https://via.placeholder.com/500x750?text=No+Poster";
                }
                
                return $"https://image.tmdb.org/t/p/w500{PosterPath}";
            }
        }
    }
}
