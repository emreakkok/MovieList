using MovieList.Core.Entities;

namespace MovieList.Core.Interfaces
{
    public interface IFollowRepository : IRepository<Follow>
    {
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<Follow?> GetFollowAsync(int followerId, int followingId);
        Task<int> GetFollowerCountAsync(int userId);
        Task<int> GetFollowingCountAsync(int userId);

        Task<IEnumerable<Follow>> GetFollowersAsync(int userId);
        Task<IEnumerable<Follow>> GetFollowingAsync(int userId);
    }
}