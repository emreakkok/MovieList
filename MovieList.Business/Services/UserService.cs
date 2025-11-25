using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.User;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<IEnumerable<MovieDto>> GetFavoriteMoviesAsync(int userId, int count = 4)
        {
            // Favori filmlerin çekilmesi
            var userEntity = await _userRepository.GetUserWithMoviesAsync(userId);

            if (userEntity == null)
                return Enumerable.Empty<MovieDto>();

            var favoriteMovies = userEntity.UserMovies
                                            .Where(um => um.IsFavorite)
                                            .Select(um => um.Movie)
                                            .Take(count)
                                            .ToList();

            return favoriteMovies.Select(MapToMovieDto).ToList();
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);

            if (userEntity == null)
                return null;

            
            var totalWatchCount = await _userRepository.CountAsync(u => u.UserMovies.Any(um => um.IsWatched));

            return new UserProfileDto
            {
                Id = userEntity.Id,
                Username = userEntity.UserName ?? userEntity.Email,
                Bio = userEntity.Bio,
                ProfileImageUrl = userEntity.ProfilePictureUrl,
                CreatedDate = userEntity.CreatedDate,

                FollowerCount = userEntity.FollowerCount,
                FollowingCount = userEntity.FollowingCount,
                WatchCountTotal = totalWatchCount
               
            };
        }

        public async Task<IEnumerable<MovieDto>> GetRecentWatchedMoviesAsync(int userId, int count = 4)
        {
            var userEntity = await _userRepository.GetUserWithMoviesAsync(userId);

            if (userEntity == null)
                return Enumerable.Empty<MovieDto>();

            var recentMovies = userEntity.UserMovies
                                         .Where(um => um.IsWatched && um.WatchedDate.HasValue)
                                         .OrderByDescending(um => um.WatchedDate)
                                         .Take(count)
                                         .Select(um => um.Movie)
                                         .ToList();

            return recentMovies.Select(MapToMovieDto).ToList();
        }


    }
}
