using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.User;
using MovieList.Core.Interfaces;

namespace MovieList.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMovieService _movieService;
        public SearchController(IUserRepository userRepository, IMovieService movieService)
        {
            _userRepository = userRepository;
            _movieService = movieService;
        }

        // Kullanıcı arama
        [HttpGet]
        public async Task<IActionResult> Users(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new List<UserDto>());

            var users = await _userRepository.SearchUsersAsync(q);

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName ?? u.Email,
                Bio = u.Bio,
                ProfileImageUrl = u.ProfilePictureUrl
            }).ToList();

            ViewBag.SearchTerm = q;
            return View(userDtos);
        }

        // Film arama (zaten var ama burada da olabilir)
        [HttpGet]
        public async Task<IActionResult> Movies(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View("Movies", new List<MovieList.Core.DTOs.Movie.MovieDto>());
            }

            var movies = await _movieService.SearchMoviesAsync(q);
            ViewBag.SearchTerm = q;

            return View("Movies", movies);
        }

        // AJAX için kullanıcı arama
        [HttpGet]
        public async Task<IActionResult> SearchUsersJson(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(new List<object>());

            var users = await _userRepository.SearchUsersAsync(q);

            var result = users.Select(u => new
            {
                id = u.Id,
                username = u.UserName ?? u.Email,
                bio = u.Bio,
                profileImage = u.ProfilePictureUrl ?? "/images/default-avatar.png"
            });

            return Json(result);
        }
    }
}