using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Review;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: /Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateDto dto)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Giriş yapmalısınız";
                return RedirectToAction("Login", "Account");
            }

            // Trim işlemi
            if (!string.IsNullOrWhiteSpace(dto.Content))
            {
                dto.Content = dto.Content.Trim();
            }

            // Manuel karakter kontrolü
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                TempData["ErrorMessage"] = "Yorum boş olamaz";
                return RedirectToAction("Details", "Movie", new { id = dto.MovieId });
            }

            if (dto.Content.Length < 3)
            {
                TempData["ErrorMessage"] = "Yorum en az 3 karakter olmalıdır";
                return RedirectToAction("Details", "Movie", new { id = dto.MovieId });
            }

            if (dto.Content.Length > 1000)
            {
                TempData["ErrorMessage"] = "Yorum en fazla 1000 karakter olmalıdır";
                return RedirectToAction("Details", "Movie", new { id = dto.MovieId });
            }

            // Kullanıcı yorum yapabilir mi?
            var canReview = await _reviewService.CanUserReviewAsync(userId.Value, dto.MovieId);
            if (!canReview)
            {
                TempData["ErrorMessage"] = "Bu filme yorum yapamazsınız. Filmi izlemeniz ve daha önce yorum yapmamış olmanız gerekir.";
                return RedirectToAction("Details", "Movie", new { id = dto.MovieId });
            }

            var success = await _reviewService.CreateReviewAsync(userId.Value, dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Yorum başarıyla eklendi";
            }
            else
            {
                TempData["ErrorMessage"] = "Yorum eklenirken bir hata oluştu";
            }

            return RedirectToAction("Details", "Movie", new { id = dto.MovieId });
        }

        // POST: /Review/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _reviewService.DeleteReviewAsync(id, userId.Value);

            if (success)
                return Json(new { success = true, message = "Yorum silindi" });

            return Json(new { success = false, message = "Yorum silinirken bir hata oluştu veya bu yorumu silme yetkiniz yok" });
        }
    }
}