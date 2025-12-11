using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.User;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using MovieList.DataAccess.Repositories;

namespace MovieList.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMovieRepository _userMovieRepository;
        private readonly IFollowRepository _followRepository;
        private readonly IWatchlistRepository _watchlistRepository;

        public UserService(
            IUserRepository userRepository,
            IUserMovieRepository userMovieRepository,
            IFollowRepository followRepository,
            IWatchlistRepository watchlistRepository)
        {
            _userRepository = userRepository;
            _userMovieRepository = userMovieRepository;
            _followRepository = followRepository;
            _watchlistRepository = watchlistRepository;
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

        public async Task<UserProfileDto?> GetProfileAsync(int userId, int? currentUserId = null)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);
            if (userEntity == null)
                return null;

            var watchCount = await _userMovieRepository.CountAsync(um =>
                um.UserId == userId && um.IsWatched);

            var favoriteUserMovies = await _userMovieRepository.GetUserFavoriteMoviesAsync(userId, 4);
            var favoriteMovies = favoriteUserMovies.Select(um => MapToMovieDto(um.Movie));

            var recentUserMovies = await _userMovieRepository.GetUserRecentWatchesAsync(userId, 4);
            var recentMovies = recentUserMovies.Select(um => MapToMovieDto(um.Movie));

            // ✅ Watchlist filmlerini al
            var watchlistMovies = await GetWatchlistMoviesAsync(userId, 6);

            bool isFollowing = false;
            if (currentUserId.HasValue && currentUserId.Value != userId)
            {
                isFollowing = await _followRepository.IsFollowingAsync(
                    currentUserId.Value,
                    userId);
            }

            return new UserProfileDto
            {
                Id = userEntity.Id,
                Username = userEntity.UserName ?? userEntity.Email,
                Bio = userEntity.Bio,
                ProfileImageUrl = userEntity.ProfilePictureUrl,
                CreatedDate = userEntity.CreatedDate,
                FollowerCount = userEntity.FollowerCount,
                FollowingCount = userEntity.FollowingCount,
                WatchCountTotal = watchCount,
                FavoriteMovies = favoriteMovies,
                RecentWatchedMovies = recentMovies,
                WatchlistMovies = watchlistMovies, // ✅ Eklendi
                IsCurrentUserFollowing = isFollowing
            };
        }

        public async Task<IEnumerable<MovieDto>> GetFavoriteMoviesAsync(int userId, int count = 4)
        {
            var favoriteUserMovies = await _userMovieRepository.GetUserFavoriteMoviesAsync(userId, count);

            return favoriteUserMovies
                .Select(um => MapToMovieDto(um.Movie))
                .ToList();
        }

        public async Task<IEnumerable<MovieDto>> GetRecentWatchedMoviesAsync(int userId, int count = 4)
        {
            var recentUserMovies = await _userMovieRepository.GetUserRecentWatchesAsync(userId, count);

            return recentUserMovies
                .Select(um => MapToMovieDto(um.Movie))
                .ToList();
        }

        // Overload: UserMovie'den mapping yaparken kullanıcının puanını da al
        private MovieDto MapToMovieDtoWithRating(UserMovie um)
        {
            return new MovieDto
            {
                Id = um.Movie.Id,
                TmdbId = um.Movie.TmdbId,
                Title = um.Movie.Title,
                Overview = um.Movie.Overview,
                PosterPath = um.Movie.PosterPath,
                ReleaseDate = um.Movie.ReleaseDate,
                Runtime = um.Movie.Runtime,
                VoteAverage = um.Movie.VoteAverage,
                WatchCount = um.Movie.WatchCount,
                LikeCount = um.Movie.LikeCount,
                UserRating = um.Rating 
            };
        }

        public async Task<IEnumerable<MovieDto>> GetRatedMoviesAsync(int userId)
        {
            var ratedUserMovies = await _userMovieRepository.GetUserWatchedMoviesAsync(userId);

            return ratedUserMovies
                .Where(um => um.Rating.HasValue)
                .Select(um => MapToMovieDtoWithRating(um))
                .ToList();
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesByRatingAsync(int userId, int rating)
        {
            var userMovies = await _userMovieRepository.GetUserMoviesByRatingAsync(userId, rating);

            return userMovies
                .Select(um => MapToMovieDtoWithRating(um))
                .ToList();
        }

        public async Task<IEnumerable<MovieDto>> GetWatchlistMoviesAsync(int userId, int count = 6)
        {
            var watchlistRepository = _watchlistRepository as WatchlistRepository;
            var watchlistItems = await watchlistRepository.GetUserWatchlistAsync(userId);

            return watchlistItems
                .Take(count)
                .Select(w => MapToMovieDto(w.Movie))
                .ToList();
        }
    }
}