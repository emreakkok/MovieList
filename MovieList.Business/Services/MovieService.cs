using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.Review;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;

namespace MovieList.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ITmdbApiService _tmdbApiService;

        public MovieService(
            IMovieRepository movieRepository,
            ITmdbApiService tmdbApiService)
        {
            _movieRepository = movieRepository;
            _tmdbApiService = tmdbApiService;
        }

        private MovieDto MapToMovieDto(Movie m)
        {
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

        /// <summary>
        /// TMDB'den filmi database'e kaydeder veya varsa günceller
        /// </summary>
        public async Task<Movie?> SaveMovieFromTmdbAsync(int tmdbId)
        {
            // Önce database'de var mı kontrol et
            var existingMovie = await _movieRepository.GetByTmdbIdAsync(tmdbId);
            if (existingMovie != null)
                return existingMovie;

            // TMDB'den çek
            var tmdbMovie = await _tmdbApiService.GetMovieDetailsAsync(tmdbId);
            if (tmdbMovie == null)
                return null;

            // Database'e kaydet
            var movie = new Movie
            {
                TmdbId = tmdbMovie.Id,
                Title = tmdbMovie.Title,
                OriginalTitle = tmdbMovie.OriginalTitle,
                Overview = tmdbMovie.Overview,
                PosterPath = tmdbMovie.PosterPath,
                BackdropPath = tmdbMovie.BackdropPath,
                ReleaseDate = DateTime.TryParse(tmdbMovie.ReleaseDate, out var date) ? date : null,
                Runtime = tmdbMovie.Runtime,
                VoteAverage = tmdbMovie.VoteAverage,
                VoteCount = tmdbMovie.VoteCount,
                Genres = tmdbMovie.Genres != null
                    ? string.Join(",", tmdbMovie.Genres.Select(g => g.Name))
                    : null
            };

            return await _movieRepository.AddAsync(movie);
        }

        public async Task<MovieDetailDto?> GetMovieDetailAsync(int movieId, int? currentUserId)
        {
            var movieEntity = await _movieRepository.GetMovieWithDetailsAsync(movieId);
            if (movieEntity == null)
                return null;

            var dto = new MovieDetailDto
            {
                Id = movieEntity.Id,
                TmdbId = movieEntity.TmdbId,
                Title = movieEntity.Title,
                Overview = movieEntity.Overview,
                PosterPath = movieEntity.PosterPath,
                BackdropPath = movieEntity.BackdropPath,
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

            if (currentUserId == null)
                return dto;

            var userMovie = movieEntity.UserMovies
                .FirstOrDefault(um => um.UserId == currentUserId);
            var userLike = movieEntity.MovieLikes
                .Any(ml => ml.UserId == currentUserId);

            dto.IsWatched = userMovie?.IsWatched ?? false;
            dto.UserRating = userMovie?.Rating;
            dto.IsFavorite = userMovie?.IsFavorite ?? false;
            dto.IsLikedByUser = userLike;

            return dto;
        }

        /// <summary>
        /// TMDB'den popüler filmleri çeker ve database'e kaydeder
        /// </summary>
        public async Task<IEnumerable<MovieDto>> GetPopularMoviesAsync(int count = 20)
        {
            // TMDB'den çek
            var tmdbResponse = await _tmdbApiService.GetPopularMoviesAsync();
            if (tmdbResponse == null || !tmdbResponse.Results.Any())
                return Enumerable.Empty<MovieDto>();

            var movies = new List<MovieDto>();

            foreach (var tmdbMovie in tmdbResponse.Results.Take(count))
            {
                // Database'e kaydet
                var movie = await SaveMovieFromTmdbAsync(tmdbMovie.Id);
                if (movie != null)
                {
                    movies.Add(MapToMovieDto(movie));
                }
            }

            return movies;
        }

        /// <summary>
        /// TMDB'de arama yapar
        /// </summary>
        public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<MovieDto>();

            // TMDB'de ara
            var tmdbResponse = await _tmdbApiService.SearchMoviesAsync(searchTerm);
            if (tmdbResponse == null || !tmdbResponse.Results.Any())
                return Enumerable.Empty<MovieDto>();

            var movies = new List<MovieDto>();

            foreach (var tmdbMovie in tmdbResponse.Results.Take(20))
            {
                // Database'e kaydet
                var movie = await SaveMovieFromTmdbAsync(tmdbMovie.Id);
                if (movie != null)
                {
                    movies.Add(MapToMovieDto(movie));
                }
            }

            return movies;
        }
    }
}