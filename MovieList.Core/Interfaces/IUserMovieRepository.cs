using MovieList.Core.Entities;
using System.Threading.Tasks;

namespace MovieList.Core.Interfaces
{
    public interface IUserMovieRepository : IRepository<UserMovie>
    {
        Task<UserMovie?> GetUserMovieAsync(int userId, int movieId);
        Task<IEnumerable<UserMovie>> GetUserFavoriteMoviesAsync(int userId, int count = 4);
        Task<IEnumerable<UserMovie>> GetUserRecentWatchesAsync(int userId, int count = 4);
        Task<IEnumerable<UserMovie>> GetUserWatchedMoviesAsync(int userId);

        Task<IEnumerable<UserMovie>> GetUserMoviesByRatingAsync(int userId, int rating);  // ✅ YENİ

    }
}