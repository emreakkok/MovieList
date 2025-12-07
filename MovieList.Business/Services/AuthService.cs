using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieList.Business.Helpers;
using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Auth;
using MovieList.Core.Entities;

namespace MovieList.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtHelper jwtHelper,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                // Email kontrolü
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bu email adresi zaten kullanılıyor"
                    };
                }

                // Username kontrolü
                if (await _userManager.FindByNameAsync(dto.Username) != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bu kullanıcı adı zaten kullanılıyor"
                    };
                }

                // Kullanıcı oluştur
                var user = new User
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    ProfilePictureUrl = GetRandomProfilePicture(), // Otomatik profil fotoğrafı
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Kayıt başarısız: {errors}"
                    };
                }

                _logger.LogInformation($"Yeni kullanıcı kaydedildi: {user.UserName}");

                // JWT Token oluştur
                var token = _jwtHelper.GenerateToken(user);
                var expiration = DateTime.UtcNow.AddDays(30);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Kayıt başarılı",
                    Token = token,
                    Expiration = expiration,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Username = user.UserName!,
                        Email = user.Email,
                        ProfilePictureUrl = user.ProfilePictureUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kayıt sırasında hata");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Bir hata oluştu, lütfen tekrar deneyin"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                // Email veya username ile kullanıcıyı bul
                var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername)
                    ?? await _userManager.FindByNameAsync(dto.EmailOrUsername);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email/Kullanıcı adı veya şifre hatalı"
                    };
                }

                // Şifre kontrolü
                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email/Kullanıcı adı veya şifre hatalı"
                    };
                }

                _logger.LogInformation($"Kullanıcı giriş yaptı: {user.UserName}");

                // JWT Token oluştur
                var token = _jwtHelper.GenerateToken(user);
                var expiration = DateTime.UtcNow.AddDays(30);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Giriş başarılı",
                    Token = token,
                    Expiration = expiration,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Username = user.UserName!,
                        Email = user.Email,
                        ProfilePictureUrl = user.ProfilePictureUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş sırasında hata");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Bir hata oluştu, lütfen tekrar deneyin"
                };
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user == null;
        }

        /// <summary>
        /// Otomatik profil fotoğrafı oluştur (UI Avatars API)
        /// </summary>
        private string GetRandomProfilePicture()
        {
            var colors = new[] { "00897B", "43A047", "5E35B1", "E53935", "FB8C00", "3949AB" };
            var randomColor = colors[Random.Shared.Next(colors.Length)];
            return $"https://ui-avatars.com/api/?background={randomColor}&color=fff&size=200&name=User";
        }
    }
}