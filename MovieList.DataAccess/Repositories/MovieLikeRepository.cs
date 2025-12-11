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
    public class MovieLikeRepository : Repository<MovieLike>, IMovieLikeRepository
    {
        public MovieLikeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<MovieLike?> GetMovieLikeAsync(int userId, int movieId)
        {
            return await _dbSet.FirstOrDefaultAsync(ml =>
                ml.UserId == userId && ml.MovieId == movieId);
        }

        public async Task<bool> IsLikedAsync(int userId, int movieId)
        {
            return await _dbSet.AnyAsync(ml => ml.UserId == userId && ml.MovieId == movieId);
        }
    }
}
