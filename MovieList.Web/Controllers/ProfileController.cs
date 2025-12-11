using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Business.Services;
using MovieList.Core.DTOs.User;
using MovieList.Core.Entities;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWatchlistService _watchlistService;
        private readonly IFollowService _followService;

        public ProfileController(IUserService userService, IWatchlistService watchlistService, IFollowService followService)
        {
            _userService = userService;
            _watchlistService = watchlistService;
            _followService = followService;
        }

        // Kullanıcı profili: /Profile/Index/5
        public async Task<IActionResult> Index(int id)
        {
            int? currentUserId = User.GetUserId();

            var profile = await _userService.GetProfileAsync(id, currentUserId);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        public async Task<IActionResult> RatedMovies(int id)
        {
            var movies = await _userService.GetRatedMoviesAsync(id);
            var profile = await _userService.GetProfileAsync(id);

            ViewBag.Username = profile?.Username;
            ViewBag.UserId = id;

            return View(movies);
        }

        public async Task<IActionResult> MoviesByRating(int id, int rating)
        {
            var movies = await _userService.GetMoviesByRatingAsync(id, rating);
            var profile = await _userService.GetProfileAsync(id);

            ViewBag.Username = profile?.Username;
            ViewBag.UserId = id;
            ViewBag.Rating = rating;

            return View(movies);
        }
        public async Task<IActionResult> Watchlist(int id)
        {
            var movies = await _watchlistService.GetUserWatchlistAsync(id);
            ViewBag.Username = (await _userService.GetProfileAsync(id))?.Username;
            return View(movies);
        }
        // Takipçiler sayfası
        public async Task<IActionResult> Followers(int id)
        {
            var followers = await _followService.GetFollowersAsync(id);
            var profile = await _userService.GetProfileAsync(id);

            ViewBag.Username = profile?.Username;
            ViewBag.UserId = id;

            return View(followers);
        }

        // Takip edilenler sayfası
        public async Task<IActionResult> Following(int id)
        {
            var following = await _followService.GetFollowingAsync(id);
            var profile = await _userService.GetProfileAsync(id);

            ViewBag.Username = profile?.Username;
            ViewBag.UserId = id;

            return View(following);
        }

    }
}