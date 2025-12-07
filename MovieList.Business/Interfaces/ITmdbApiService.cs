using MovieList.Core.DTOs.Tmdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface ITmdbApiService
    {
        Task<TmdbMovieResponse?> GetMovieDetailsAsync(int tmdbId);

        Task<TmdbSearchResponse?> SearchMoviesAsync(string query, int page = 1);

        Task<TmdbSearchResponse?> GetPopularMoviesAsync(int page = 1);

        Task<TmdbSearchResponse?> GetNowPlayingMoviesAsync(int page = 1);

        Task<TmdbSearchResponse?> GetUpcomingMoviesAsync(int page = 1);
    }
}
