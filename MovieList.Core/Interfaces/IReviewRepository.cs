using MovieList.Core.Entities;

namespace MovieList.Core.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetMovieReviewsAsync(int movieId);
        Task<Review?> GetUserReviewForMovieAsync(int userId, int movieId);
        Task<bool> HasUserReviewedAsync(int userId, int movieId);
    }
}