using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    public class Movie : BaseEntity
    {
        // Temel film bilgileri
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? OriginalTitle { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public string? BackdropPath { get; set; }
        public int? Runtime { get; set; }
        public string? Genres { get; set; }

        // Değerlendirme bilgileri
        public decimal? VoteAverage { get; set; }
        public int? VoteCount { get; set; }

        // İstatistikler
        public int WatchCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;

        // Navigation Properties

        public virtual ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<MovieLike> MovieLikes { get; set; } = new List<MovieLike>();
        public virtual ICollection<ListMovie> ListMovies { get; set; } = new List<ListMovie>();
        public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    }
}
