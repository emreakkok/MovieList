using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Web.Extensions;

namespace MovieList.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // Film detay: /Movie/Details/123 (database ID)
        public async Task<IActionResult> Details(int id)
        {
            // Giriş yapmış kullanıcının ID'sini al (null ise giriş yapılmamış)
            int? currentUserId = User.GetUserId();

            var movie = await _movieService.GetMovieDetailAsync(id, currentUserId);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // TMDB'den film detay: /Movie/Tmdb/550 (TMDB ID)
        public async Task<IActionResult> Tmdb(int id)
        {
            var movie = await _movieService.SaveMovieFromTmdbAsync(id);

            if (movie == null)
                return NotFound();

            return RedirectToAction("Details", new { id = movie.Id });
        }

        // Arama
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View("Search", new List<MovieList.Core.DTOs.Movie.MovieDto>());
            }

            var movies = await _movieService.SearchMoviesAsync(q);
            ViewBag.SearchTerm = q;

            return View("Search", movies);
        }

        // Popüler
        public async Task<IActionResult> Popular()
        {
            var movies = await _movieService.GetPopularMoviesAsync(50);
            return View(movies);
        }
    }
}