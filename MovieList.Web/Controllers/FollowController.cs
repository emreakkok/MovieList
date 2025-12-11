using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        // POST: /Follow/Follow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int id)
        {
            var currentUserId = User.GetUserId();

            if (currentUserId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _followService.FollowUserAsync(currentUserId.Value, id);

            if (!success)
                return Json(new { success = false, message = "Takip edilemedi" });

            return Json(new { success = true, message = "Takip edildi" });
        }

        // POST: /Follow/Unfollow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(int id)
        {
            var currentUserId = User.GetUserId();

            if (currentUserId == null)
                return Json(new { success = false, message = "Giriş yapmalısınız" });

            var success = await _followService.UnfollowUserAsync(currentUserId.Value, id);

            if (!success)
                return Json(new { success = false, message = "Takipten çıkılamadı" });

            return Json(new { success = true, message = "Takipten çıkıldı" });
        }
    }
}