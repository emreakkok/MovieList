using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Context;

namespace MovieList.DataAccess.Repositories
{
    public class FollowRepository : Repository<Follow>, IFollowRepository
    {
        public FollowRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followingId)
        {
            return await _dbSet.AnyAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<Follow?> GetFollowAsync(int followerId, int followingId)
        {
            return await _dbSet.FirstOrDefaultAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<int> GetFollowerCountAsync(int userId)
        {
            return await _dbSet.CountAsync(f => f.FollowingId == userId);
        }

        public async Task<int> GetFollowingCountAsync(int userId)
        {
            return await _dbSet.CountAsync(f => f.FollowerId == userId);
        }

        public async Task<IEnumerable<Follow>> GetFollowersAsync(int userId)
        {
            return await _dbSet
                .Include(f => f.Follower)
                .Where(f => f.FollowingId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Follow>> GetFollowingAsync(int userId)
        {
            return await _dbSet
                .Include(f => f.Following)
                .Where(f => f.FollowerId == userId)
                .ToListAsync();
        }
    }
}