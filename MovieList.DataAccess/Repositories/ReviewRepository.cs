using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Context;

namespace MovieList.DataAccess.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetMovieReviewsAsync(int movieId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Review?> GetUserReviewForMovieAsync(int userId, int movieId)
        {
            return await _dbSet
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task<bool> HasUserReviewedAsync(int userId, int movieId)
        {
            return await _dbSet.AnyAsync(r => r.UserId == userId && r.MovieId == movieId);
        }
    }
}