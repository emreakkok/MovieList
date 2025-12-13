using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Review;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;

namespace MovieList.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserMovieRepository _userMovieRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IUserMovieRepository userMovieRepository)
        {
            _reviewRepository = reviewRepository;
            _userMovieRepository = userMovieRepository;
        }

        public async Task<bool> CreateReviewAsync(int userId, ReviewCreateDto dto)
        {
            // Karakter sınırı kontrolü - trim'den sonra kontrol et
            var trimmedContent = dto.Content.Trim();
            if (string.IsNullOrWhiteSpace(trimmedContent) || trimmedContent.Length < 3 || trimmedContent.Length > 1000)
                return false;

            // Kullanıcı bu filmi izlemiş mi? - ZORUNLU KONTROL
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, dto.MovieId);
            if (userMovie == null || !userMovie.IsWatched)
                return false; // Film izlenmemişse yorum yapılamaz

            // Kullanıcı daha önce yorum yapmış mı?
            var existingReview = await _reviewRepository.GetUserReviewForMovieAsync(userId, dto.MovieId);
            if (existingReview != null)
                return false; // Bir kullanıcı bir filme sadece bir kez yorum yapabilir

            var review = new Review
            {
                UserId = userId,
                MovieId = dto.MovieId,
                Content = trimmedContent,
                CreatedDate = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null || review.UserId != userId)
                return false;

            await _reviewRepository.DeleteAsync(review);
            return true;
        }

        public async Task<IEnumerable<ReviewDto>> GetMovieReviewsAsync(int movieId)
        {
            var reviews = await _reviewRepository.GetMovieReviewsAsync(movieId);

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                Username = r.User.UserName ?? r.User.Email,
                UserProfileImage = r.User.ProfilePictureUrl,
                MovieId = r.MovieId,
                Content = r.Content,
                LikeCount = r.LikeCount,
                CreatedDate = r.CreatedDate
            });
        }

        public async Task<bool> CanUserReviewAsync(int userId, int movieId)
        {
            // Filmi izlemiş mi? - İLK ÖNCE BU KONTROL EDİLMELİ
            var userMovie = await _userMovieRepository.GetUserMovieAsync(userId, movieId);
            if (userMovie == null || !userMovie.IsWatched)
                return false; // Film izlenmemişse yorum yapılamaz

            // Daha önce yorum yapmış mı?
            var hasReviewed = await _reviewRepository.HasUserReviewedAsync(userId, movieId);
            if (hasReviewed)
                return false;

            return true;
        }
    }
}