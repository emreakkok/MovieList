using MovieList.Core.DTOs.Movie;
using MovieList.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface IMovieService
    {
        Task<MovieDetailDto?> GetMovieDetailAsync(int movieId, int? currentUserId);
        Task<IEnumerable<MovieDto>> GetPopularMoviesAsync(int count = 20);
        Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm);
        Task<Movie?> SaveMovieFromTmdbAsync(int tmdbId);
    }
}
