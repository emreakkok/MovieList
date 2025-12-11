using MovieList.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Interfaces
{
    public interface IMovieLikeRepository : IRepository<MovieLike>
    {
        Task<MovieLike?> GetMovieLikeAsync(int userId, int movieId);
        Task<bool> IsLikedAsync(int userId, int movieId);
    }
}
