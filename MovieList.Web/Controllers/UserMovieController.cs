using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    [Authorize]
    public class UserMovieController : Controller
    {
        private readonly IUserMovieService _userMovieService;

        public UserMovieController(IUserMovieService userMovieService)
        {
            _userMovieService = userMovieService;
        }

        // POST: /UserMovie/MarkAsWatched/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsWatched(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _userMovieService.MarkAsWatchedAsync(userId.Value, id);

            return Json(new { success, message = success ? "İzlendi olarak işaretlendi" : "Hata oluştu" });
        }

        // POST: /UserMovie/UnmarkAsWatched/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnmarkAsWatched(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _userMovieService.UnmarkAsWatchedAsync(userId.Value, id);

            return Json(new { success, message = success ? "İzlenmedi olarak işaretlendi" : "Hata oluştu" });
        }

        // POST: /UserMovie/Rate/5?rating=8
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int id, int rating)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _userMovieService.RateMovieAsync(userId.Value, id, rating);

            return Json(new { success, message = success ? $"{rating}/10 puan verildi" : "Geçersiz puan" });
        }

        // POST: /UserMovie/ToggleLike/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var isLiked = await _userMovieService.ToggleLikeMovieAsync(userId.Value, id);

            return Json(new
            {
                success = true,
                isLiked = isLiked,
                message = isLiked ? "Beğenildi" : "Beğeni kaldırıldı"
            });
        }

        // POST: /UserMovie/AddToFavorites/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _userMovieService.AddToFavoritesAsync(userId.Value, id);

            return Json(new { success, message = success ? "Favorilere eklendi" : "Hata oluştu" });
        }
    }
}