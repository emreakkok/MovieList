using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Business.Services;
using MovieList.Core.DTOs.Auth;
using MovieList.Core.DTOs.User;
using MovieList.Core.Entities;
using MovieList.Web.Extensions;
using System.Security.Claims;

namespace MovieList.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IUserMovieService _userMovieService;
        private readonly IMovieService _movieService;

        public AccountController(
            IAuthService authService,
            UserManager<User> userManager,
            IUserMovieService userMovieService,
            IMovieService movieService)
        {
            _authService = authService;
            _userManager = userManager;
            _userMovieService = userMovieService;
            _movieService = movieService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            // JWT Token'ı cookie'ye kaydet
            Response.Cookies.Append("jwt", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.Expiration
            });

            TempData["SuccessMessage"] = "Kayıt başarılı! Hoş geldiniz.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            // JWT Token'ı cookie'ye kaydet
            Response.Cookies.Append("jwt", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.Expiration
            });

            TempData["SuccessMessage"] = $"Hoş geldin, {result.User?.Username}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            TempData["InfoMessage"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/CheckEmail (AJAX için)
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var available = await _authService.IsEmailAvailableAsync(email);
            return Json(new { available });
        }

        // GET: /Account/CheckUsername (AJAX için)
        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var available = await _authService.IsUsernameAvailableAsync(username);
            return Json(new { available });
        }

        // GET: /Account/EditProfile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
                return NotFound();

            // ✅ Favori film ID'lerini al
            var favoriteMovieIds = await _userMovieService.GetFavoriteMovieIdsAsync(userId.Value);

            var model = new UserUpdateDto
            {
                Username = user.UserName ?? "",
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                FavoriteMovieIds = favoriteMovieIds
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserUpdateDto model, IFormFile? profilePicture)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.GetUserId();
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
                return NotFound();

            // Username değiştirme kontrolü
            if (user.UserName != model.Username)
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor");
                    return View(model);
                }
                user.UserName = model.Username;
            }

            user.Bio = model.Bio;

            // ✅ Profil fotoğrafı yükleme
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Sadece JPEG, PNG ve GIF formatları desteklenir");
                    return View(model);
                }

                // Dosya boyutu kontrolü (5MB)
                if (profilePicture.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Dosya boyutu 5MB'dan küçük olmalıdır");
                    return View(model);
                }

                // Eski profil fotoğrafını sil (varsa)
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) &&
                    user.ProfilePictureUrl.StartsWith("/uploads/"))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // wwwroot/uploads/profiles klasörüne kaydet
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureUrl = $"/uploads/profiles/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Favori filmleri kaydet
            await _userMovieService.SetFavoriteMoviesAsync(userId.Value, model.FavoriteMovieIds);

            TempData["SuccessMessage"] = "Profil başarıyla güncellendi";
            return RedirectToAction("Index", "Profile", new { id = userId });
        }

        // Film arama API (AJAX için)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchMoviesForFavorites(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());

            var movies = await _movieService.SearchMoviesAsync(query);

            var result = movies.Take(10).Select(m => new
            {
                id = m.Id,
                title = m.Title,
                year = m.ReleaseDate?.Year,
                poster = m.FullPosterUrl
            });

            return Json(result);
        }
    }
}