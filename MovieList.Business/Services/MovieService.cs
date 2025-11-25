using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.Review;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        private MovieDto MapToMovieDto(Movie m)
        {
            // Tekrarlanan manuel dönüşüm metodunu ayırıyoruz
            return new MovieDto
            {
                Id = m.Id,
                TmdbId = m.TmdbId,
                Title = m.Title,
                Overview = m.Overview,
                PosterPath = m.PosterPath,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                VoteAverage = m.VoteAverage,
                WatchCount = m.WatchCount,
                LikeCount = m.LikeCount
            };
        }

        public async Task<MovieDetailDto?> GetMovieDetailAsync(int movieId, int? currentUserId)
        {
            var movieEntity = await _movieRepository.GetMovieWithDetailsAsync(movieId);
            if (movieEntity == null)
                return null;

            // Film DTO’su oluşturma
            var dto = new MovieDetailDto
            {
                Id = movieEntity.Id,
                TmdbId = movieEntity.TmdbId,
                Title = movieEntity.Title,
                Overview = movieEntity.Overview,
                PosterPath = movieEntity.PosterPath,
                ReleaseDate = movieEntity.ReleaseDate,
                Runtime = movieEntity.Runtime,
                VoteAverage = movieEntity.VoteAverage,
                WatchCount = movieEntity.WatchCount,
                LikeCount = movieEntity.LikeCount,
                Reviews = movieEntity.Reviews
                    .OrderByDescending(r => r.CreatedDate)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        Username = r.User.UserName!,
                        UserProfileImage = r.User.ProfilePictureUrl,
                        MovieId = r.MovieId,
                        Content = r.Content,
                        LikeCount = r.LikeCount,
                        CreatedDate = r.CreatedDate
                    })
                    .ToList()
            };
            // Eğer kullanıcı giriş yapmamışsa, kişiye özel alanlar false/null döner
            if (currentUserId == null)
                return dto;

            // Kullanıcının ilişki bilgisi
            var userMovie = movieEntity.UserMovies
                .FirstOrDefault(um => um.UserId == currentUserId);

            var userLike = movieEntity.MovieLikes
                .Any(ml => ml.UserId == currentUserId);

            // UserMovie bilgilerini ekliyoruz
            dto.IsWatched = userMovie?.IsWatched ?? false;
            dto.UserRating = userMovie?.Rating;
            dto.IsFavorite = userMovie?.IsFavorite ?? false;

            // Like bilgisi
            dto.IsLikedByUser = userLike;

            return dto;

        }

        public async Task<IEnumerable<MovieDto>> GetPopularMoviesAsync(int count = 20)
        {
            var movies = await _movieRepository.GetPopularMoviesAsync(count);
            return movies.Select(MapToMovieDto).ToList();
        }

        public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm)
        {
            var movies = await _movieRepository.SearchMoviesAsync(searchTerm);
            return movies.Select(MapToMovieDto).ToList();
        }
    }
}
