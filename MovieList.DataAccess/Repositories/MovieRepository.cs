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
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Movie?> GetByTmdbIdAsync(int tmdbId)
        {
            return await _context.Movies
                .Include(m => m.UserMovies)
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
        }

        public async Task<IEnumerable<Movie>> GetMostWatchedAsync(int count = 20)
        {
            return await _dbSet.OrderByDescending(m => m.WatchCount)
                .ThenByDescending(m => m.LikeCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieWithDetailsAsync(int movieId)
        {
            var movie = await _dbSet
                .Include(m => m.Reviews.OrderByDescending(r=>r.CreatedDate))
                    .ThenInclude(r => r.User)
                .Include(m => m.UserMovies)
                .Include(m => m.MovieLikes)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            return movie;

        }

        public async Task<IEnumerable<Movie>> GetPopularMoviesAsync(int count = 20)
        {
            return await _dbSet.OrderByDescending(m => m.VoteCount)
                .ThenByDescending(m => m.VoteAverage)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(m => m.Title.Contains(searchTerm) ||
                           (m.OriginalTitle != null && m.OriginalTitle.Contains(searchTerm)))
                .OrderByDescending(m => m.VoteAverage)
                .ToListAsync();
        }

        public async Task UpdateMovieStatsAsync(int movieId)
        {
            var movie = await _dbSet.FindAsync(movieId);

            if (movie == null)
                return;
            movie.WatchCount = await _context.UserMovies
                .CountAsync(um => um.MovieId == movieId && um.IsWatched);

            movie.LikeCount = await _context.MovieLikes
               .CountAsync(ml => ml.MovieId == movieId);

            _dbSet.Update(movie);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Movie>> GetAllWithUserMoviesAsync()
        {
            return await _dbSet
                .Include(m => m.UserMovies)
                .ToListAsync();
        }
    }
}
