using MovieList.Core.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface IWatchlistService
    {
        Task<bool> AddToWatchlistAsync(int userId, int movieId);
        Task<bool> RemoveFromWatchlistAsync(int userId, int movieId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
        Task<IEnumerable<MovieDto>> GetUserWatchlistAsync(int userId);
    }
}
