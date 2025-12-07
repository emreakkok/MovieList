using System.Security.Claims;

namespace MovieList.Web.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Giriş yapmış kullanıcının ID'sini döndürür
        /// </summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Giriş yapmış kullanıcının username'ini döndürür
        /// </summary>
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Giriş yapmış kullanıcının email'ini döndürür
        /// </summary>
        public static string? GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}