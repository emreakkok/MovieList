using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface IUserMovieService
    {
        Task<bool> MarkAsWatchedAsync(int userId, int movieId);
        Task<bool> UnmarkAsWatchedAsync(int userId, int movieId);
        Task<bool> RateMovieAsync(int userId, int movieId, int rating);
        Task<bool> RemoveRatingAsync(int userId, int movieId); 
        Task<bool> AddToFavoritesAsync(int userId, int movieId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int movieId);
        Task<bool> ToggleLikeMovieAsync(int userId, int movieId);
        Task<bool> LikeMovieAsync(int userId, int movieId);
        Task<bool> UnlikeMovieAsync(int userId, int movieId);
        Task<bool> SetFavoriteMoviesAsync(int userId, List<int> movieIds);  
        Task<List<int>> GetFavoriteMovieIdsAsync(int userId);
    }
}
