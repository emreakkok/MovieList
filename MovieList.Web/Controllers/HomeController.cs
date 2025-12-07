using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;

namespace MovieList.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IMovieService movieService,
            ILogger<HomeController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("=== ANA SAYFA AÇILDI ===");

            try
            {
                _logger.LogInformation("Popüler filmler çekiliyor...");

                var popularMovies = await _movieService.GetPopularMoviesAsync(20);

                _logger.LogInformation($"Toplam {popularMovies.Count()} film geldi");

                return View(popularMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ANA SAYFA HATASI");
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}