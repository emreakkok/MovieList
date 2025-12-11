using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Context;

namespace MovieList.DataAccess.Repositories
{
    public class UserMovieRepository : Repository<UserMovie>, IUserMovieRepository
    {
        public UserMovieRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserMovie?> GetUserMovieAsync(int userId, int movieId)
        {
            return await _dbSet
                .Include(um => um.Movie)
                .FirstOrDefaultAsync(um => um.UserId == userId && um.MovieId == movieId);
        }

        public async Task<IEnumerable<UserMovie>> GetUserFavoriteMoviesAsync(int userId, int count = 4)
        {
            return await _dbSet
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId && um.IsFavorite)
                .OrderByDescending(um => um.WatchedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserMovie>> GetUserRecentWatchesAsync(int userId, int count = 4)
        {
            return await _dbSet
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId && um.IsWatched)
                .OrderByDescending(um => um.WatchedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserMovie>> GetUserWatchedMoviesAsync(int userId)
        {
            return await _dbSet
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId && um.IsWatched)
                .OrderByDescending(um => um.WatchedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserMovie>> GetUserMoviesByRatingAsync(int userId, int rating)
        {
            return await _dbSet
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId && um.Rating == rating)
                .OrderByDescending(um => um.WatchedDate)
                .ToListAsync();
        }
    }
}