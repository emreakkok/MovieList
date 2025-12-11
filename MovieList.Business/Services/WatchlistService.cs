using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Movie;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;

namespace MovieList.Business.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IUserMovieRepository _userMovieRepository;

        public WatchlistService(
            IWatchlistRepository watchlistRepository,
            IUserMovieRepository userMovieRepository)
        {
            _watchlistRepository = watchlistRepository;
            _userMovieRepository = userMovieRepository;
        }

        public async Task<bool> AddToWatchlistAsync(int userId, int movieId)
        {
            // İzlenmiş mi kontrol et
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);
            if (userMovie != null && userMovie.IsWatched)
            {
                return false; // İzlenmiş filmi watchlist'e ekleyemezsin
            }

            // Zaten watchlist'te mi?
            var exists = await _watchlistRepository.IsInWatchlistAsync(userId, movieId);
            if (exists)
                return false;

            var watchlistItem = new Watchlist
            {
                UserId = userId,
                MovieId = movieId,
                AddedDate = DateTime.UtcNow
            };

            await _watchlistRepository.AddAsync(watchlistItem);
            return true;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int userId, int movieId)
        {
            var watchlistItem = await _watchlistRepository.GetWatchlistItemAsync(userId, movieId);

            if (watchlistItem == null)
                return false;

            await _watchlistRepository.DeleteAsync(watchlistItem);
            return true;
        }

        public async Task<bool> IsInWatchlistAsync(int userId, int movieId)
        {
            return await _watchlistRepository.IsInWatchlistAsync(userId, movieId);
        }

        public async Task<IEnumerable<MovieDto>> GetUserWatchlistAsync(int userId)
        {
            var watchlistItems = await _watchlistRepository.GetUserWatchlistAsync(userId);

            return watchlistItems.Select(w => new MovieDto
            {
                Id = w.Movie.Id,
                TmdbId = w.Movie.TmdbId,
                Title = w.Movie.Title,
                Overview = w.Movie.Overview,
                PosterPath = w.Movie.PosterPath,
                ReleaseDate = w.Movie.ReleaseDate,
                Runtime = w.Movie.Runtime,
                VoteAverage = w.Movie.VoteAverage,
                WatchCount = w.Movie.WatchCount,
                LikeCount = w.Movie.LikeCount
            });
        }
    }
}