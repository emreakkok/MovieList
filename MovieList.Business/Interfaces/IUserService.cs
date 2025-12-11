using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.User;

namespace MovieList.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId, int? currentUserId = null);
        Task<IEnumerable<MovieDto>> GetFavoriteMoviesAsync(int userId, int count = 4);
        Task<IEnumerable<MovieDto>> GetRecentWatchedMoviesAsync(int userId, int count = 4);

        Task<IEnumerable<MovieDto>> GetRatedMoviesAsync(int userId);  // Tüm oylanmış filmler
        Task<IEnumerable<MovieDto>> GetMoviesByRatingAsync(int userId, int rating);  // Belirli puana göre

    }
}