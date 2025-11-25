using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithMoviesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.UserMovies)
                    .ThenInclude(um => um.Movie)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /* 
        public async Task<User?> GetUserWithFollowersAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Followers)
                    .ThenInclude(f => f.Follower)
                .Include(u => u.Followings)
                    .ThenInclude(f => f.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        */

        public async Task<IEnumerable<User>> GetFollowersAsync(int userId)
        {
            return await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Select(f => f.Follower)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetFollowingAsync(int userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Following)
                .ToListAsync();
        }
    }
}
