using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    [Authorize]
    public class WatchlistController : Controller
    {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        // GET: /Watchlist
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var watchlist = await _watchlistService.GetUserWatchlistAsync(userId.Value);
            return View(watchlist);
        }

        // POST: /Watchlist/Add/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _watchlistService.AddToWatchlistAsync(userId.Value, id);

            return Json(new
            {
                success,
                message = success ? "Watchlist'e eklendi" : "Film zaten watchlist'te"
            });
        }

        // POST: /Watchlist/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _watchlistService.RemoveFromWatchlistAsync(userId.Value, id);

            return Json(new
            {
                success,
                message = success ? "Watchlist'ten çıkarıldı" : "Film watchlist'te değil"
            });
        }
    }
}