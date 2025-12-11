using MovieList.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Interfaces
{
    public interface IWatchlistRepository : IRepository<Watchlist>
    {
        Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
        Task<Watchlist?> GetWatchlistItemAsync(int userId, int movieId);
    }
}
