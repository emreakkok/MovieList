using MovieList.Core.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.User
{
    public class UserProfileDto
    {
        // UserDto'dan gelen temel alanlar
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }

        // Profile özel istatistikler ve listeler
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int WatchCountTotal { get; set; } // Toplam izlenen film sayısı

        public IEnumerable<MovieDto> WatchlistMovies { get; set; } = new List<MovieDto>();

        // Profil Sayfası Listeleri
        public IEnumerable<MovieDto> FavoriteMovies { get; set; } = new List<MovieDto>();
        public IEnumerable<MovieDto> RecentWatchedMovies { get; set; } = new List<MovieDto>();

        // Takip durumunu göstermek için (Profilin sahibi değilse)
        public bool IsCurrentUserFollowing { get; set; }
    }
}
