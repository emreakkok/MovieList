using MovieList.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie?> GetByTmdbIdAsync(int tmdbId);
        Task<Movie?> GetMovieWithDetailsAsync(int movieId);
        Task<IEnumerable<Movie>> GetPopularMoviesAsync(int count = 20);
        Task<IEnumerable<Movie>> GetMostWatchedAsync(int count = 20);
        Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm);

        Task UpdateMovieStatsAsync(int movieId);
    }
}
