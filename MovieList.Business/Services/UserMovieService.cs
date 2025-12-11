using MovieList.Business.Interfaces;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;

namespace MovieList.Business.Services
{
    public class UserMovieService : IUserMovieService
    {
        private readonly IUserMovieRepository _userMovieRepository;
        private readonly IMovieLikeRepository _movieLikeRepository;
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IMovieRepository _movieRepository;

        public UserMovieService(
            IUserMovieRepository userMovieRepository,
            IMovieLikeRepository movieLikeRepository,
            IWatchlistRepository watchlistRepository,
            IMovieRepository movieRepository)
        {
            _userMovieRepository = userMovieRepository;
            _movieLikeRepository = movieLikeRepository;
            _watchlistRepository = watchlistRepository;
            _movieRepository = movieRepository;
        }

        public async Task<bool> MarkAsWatchedAsync(int userId, int movieId)
        {
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
            {
                userMovie = new UserMovie
                {
                    UserId = userId,
                    MovieId = movieId,
                    IsWatched = true,
                    WatchedDate = DateTime.UtcNow
                };
                await _userMovieRepository.AddAsync(userMovie);
            }
            else
            {
                userMovie.IsWatched = true;
                userMovie.WatchedDate = DateTime.UtcNow;
                await _userMovieRepository.UpdateAsync(userMovie);
            }

            // Watchlist'ten çıkar
            var watchlistItem = await _watchlistRepository.GetWatchlistItemAsync(userId, movieId);
            if (watchlistItem != null)
            {
                await _watchlistRepository.DeleteAsync(watchlistItem);
            }

            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }

        public async Task<bool> UnmarkAsWatchedAsync(int userId, int movieId)
        {
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
                return false;

            // İzlenmedi olarak işaretle ve puanı kaldır
            userMovie.IsWatched = false;
            userMovie.WatchedDate = null;
            userMovie.Rating = null;  // Puan da kaldırılır
            await _userMovieRepository.UpdateAsync(userMovie);

            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }

        public async Task<bool> RateMovieAsync(int userId, int movieId, int rating)
        {
            if (rating < 1 || rating > 10)
                return false;

            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
            {
                // İlk kez puan veriliyor
                userMovie = new UserMovie
                {
                    UserId = userId,
                    MovieId = movieId,
                    IsWatched = true,
                    WatchedDate = DateTime.UtcNow,
                    Rating = rating
                };
                await _userMovieRepository.AddAsync(userMovie);
            }
            else
            {
                // ✅ Aynı puana tekrar tıklandıysa puanı kaldır
                if (userMovie.Rating == rating)
                {
                    userMovie.Rating = null;
                }
                else
                {
                    userMovie.Rating = rating;
                }

                userMovie.IsWatched = true;
                if (!userMovie.WatchedDate.HasValue)
                {
                    userMovie.WatchedDate = DateTime.UtcNow;
                }
                await _userMovieRepository.UpdateAsync(userMovie);
            }

            // Watchlist'ten çıkar (puan verildiyse)
            var watchlistItem = await _watchlistRepository.GetWatchlistItemAsync(userId, movieId);
            if (watchlistItem != null)
            {
                await _watchlistRepository.DeleteAsync(watchlistItem);
            }

            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }

        public async Task<bool> RemoveRatingAsync(int userId, int movieId)
        {
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
                return false;

            userMovie.Rating = null;
            await _userMovieRepository.UpdateAsync(userMovie);

            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int movieId)
        {
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
            {
                userMovie = new UserMovie
                {
                    UserId = userId,
                    MovieId = movieId,
                    IsWatched = true,
                    WatchedDate = DateTime.UtcNow,
                    IsFavorite = true
                };
                await _userMovieRepository.AddAsync(userMovie);
            }
            else
            {
                userMovie.IsFavorite = true;
                await _userMovieRepository.UpdateAsync(userMovie);
            }

            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int movieId)
        {
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

            if (userMovie == null)
                return false;

            userMovie.IsFavorite = false;
            await _userMovieRepository.UpdateAsync(userMovie);
            return true;
        }

        public async Task<bool> ToggleLikeMovieAsync(int userId, int movieId)
        {
            var existingLike = await _movieLikeRepository.GetMovieLikeAsync(userId, movieId);

            if (existingLike != null)
            {
                // Zaten beğenilmiş, kaldır
                await _movieLikeRepository.DeleteAsync(existingLike);
                await _movieRepository.UpdateMovieStatsAsync(movieId);
                return false; // false = beğeni kaldırıldı
            }
            else
            {
                // Beğenilmemiş, ekle
                var movieLike = new MovieLike
                {
                    UserId = userId,
                    MovieId = movieId,
                    CreatedDate = DateTime.UtcNow
                };
                await _movieLikeRepository.AddAsync(movieLike);
                await _movieRepository.UpdateMovieStatsAsync(movieId);
                return true; // true = beğenildi
            }
        }

        public async Task<bool> LikeMovieAsync(int userId, int movieId)
        {
            var isLiked = await _movieLikeRepository.IsLikedAsync(userId, movieId);
            if (isLiked)
                return false;

            var movieLike = new MovieLike
            {
                UserId = userId,
                MovieId = movieId,
                CreatedDate = DateTime.UtcNow
            };

            await _movieLikeRepository.AddAsync(movieLike);
            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }

        public async Task<bool> UnlikeMovieAsync(int userId, int movieId)
        {
            var movieLike = await _movieLikeRepository.GetMovieLikeAsync(userId, movieId);

            if (movieLike == null)
                return false;

            await _movieLikeRepository.DeleteAsync(movieLike);
            await _movieRepository.UpdateMovieStatsAsync(movieId);
            return true;
        }
        public async Task<bool> SetFavoriteMoviesAsync(int userId, List<int> movieIds)
        {
            if (movieIds.Count > 4)
                return false;

            // Önce tüm favorileri kaldır
            var allUserMovies = await _userMovieRepository.GetUserWatchedMoviesAsync(userId);
            foreach (var um in allUserMovies.Where(um => um.IsFavorite))
            {
                um.IsFavorite = false;
                await _userMovieRepository.UpdateAsync(um);
            }

            // Yeni favorileri ayarla
            foreach (var movieId in movieIds)
            {
                var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);

                if (userMovie == null)
                {
                    userMovie = new UserMovie
                    {
                        UserId = userId,
                        MovieId = movieId,
                        IsWatched = true,
                        WatchedDate = DateTime.UtcNow,
                        IsFavorite = true
                    };
                    await _userMovieRepository.AddAsync(userMovie);
                }
                else
                {
                    userMovie.IsFavorite = true;
                    await _userMovieRepository.UpdateAsync(userMovie);
                }
            }

            return true;
        }

        // Favori film ID'lerini getir
        public async Task<List<int>> GetFavoriteMovieIdsAsync(int userId)
        {
            var favorites = await _userMovieRepository.GetUserFavoriteMoviesAsync(userId, 4);
            return favorites.Select(um => um.MovieId).ToList();
        }
    }
}