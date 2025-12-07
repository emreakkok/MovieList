using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Auth;
using System.Security.Claims;

namespace MovieList.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthService authService,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
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
    }
}