using MovieList.Core.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface IMovieService
    {
        // Ana sayfa için popüler filmleri getirir
        Task<IEnumerable<MovieDto>> GetPopularMoviesAsync(int count = 20);

        // Film detaylarını, yorumları ve kullanıcının etkileşim durumunu (izledi mi/beğendi mi) içerir.
        Task<MovieDetailDto?> GetMovieDetailAsync(int movieId, int? currentUserId);

        // Arama sorgularını yönetir
        Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm);
    }
}
