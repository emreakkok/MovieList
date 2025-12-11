using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Context;

namespace MovieList.DataAccess.Repositories
{
    public class WatchlistRepository : Repository<Watchlist>, IWatchlistRepository
    {
        public WatchlistRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Watchlist>> GetUserWatchlistAsync(int userId)
        {
            return await _dbSet
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();
        }

        public async Task<bool> IsInWatchlistAsync(int userId, int movieId)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.MovieId == movieId);
        }

        public async Task<Watchlist?> GetWatchlistItemAsync(int userId, int movieId)
        {
            return await _dbSet.FirstOrDefaultAsync(w =>
                w.UserId == userId && w.MovieId == movieId);
        }
    }
}