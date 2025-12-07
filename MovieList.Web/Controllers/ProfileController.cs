using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;

namespace MovieList.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        // Kullanıcı profili: /Profile/Index/5
        public async Task<IActionResult> Index(int id)
        {
            var profile = await _userService.GetProfileAsync(id);

            if (profile == null)
                return NotFound();

            // Favori filmler
            var favoriteMovies = await _userService.GetFavoriteMoviesAsync(id, 4);
            ViewBag.FavoriteMovies = favoriteMovies;

            // Son izlenen filmler
            var recentMovies = await _userService.GetRecentWatchedMoviesAsync(id, 4);
            ViewBag.RecentMovies = recentMovies;

            return View(profile);
        }
    }
}