using MovieList.Core.DTOs.Review;

namespace MovieList.Business.Interfaces
{
    public interface IReviewService
    {
        Task<bool> CreateReviewAsync(int userId, ReviewCreateDto dto);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
        Task<IEnumerable<ReviewDto>> GetMovieReviewsAsync(int movieId);
        Task<bool> CanUserReviewAsync(int userId, int movieId);
    }
}