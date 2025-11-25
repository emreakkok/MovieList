using MovieList.Core.DTOs.Movie;
using MovieList.Core.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId);
        Task<IEnumerable<MovieDto>> GetFavoriteMoviesAsync(int userId, int count = 4);
        Task<IEnumerable<MovieDto>> GetRecentWatchedMoviesAsync(int userId, int count = 4);

    }
}
