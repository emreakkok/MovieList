using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.Tmdb;
using System.Text.Json;

namespace MovieList.Business.Services
{
    public class TmdbApiService : ITmdbApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken;
        private readonly ILogger<TmdbApiService> _logger;

        public TmdbApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TmdbApiService> logger)
        {
            _httpClient = httpClient;

            // Bearer Token'ı al
            _bearerToken = configuration["TmdbApi:BearerToken"]
                ?? throw new Exception("TMDB Bearer Token bulunamadı!");

            _logger = logger;

            // Bearer Token'ı header'a ekle
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");

            _logger.LogInformation($"TMDB API Token ilk 10 karakter: {_bearerToken.Substring(0, 10)}...");
        }

        public async Task<TmdbMovieResponse?> GetMovieDetailsAsync(int tmdbId)
        {
            try
            {
                var url = $"movie/{tmdbId}?language=tr-TR";

                _logger.LogInformation($"TMDB API isteği: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"TMDB API hatası: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Film detayı başarıyla alındı: {tmdbId}");

                return JsonSerializer.Deserialize<TmdbMovieResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Film detayları çekilirken hata: {tmdbId}");
                return null;
            }
        }

        public async Task<TmdbSearchResponse?> SearchMoviesAsync(string query, int page = 1)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var url = $"search/movie?query={encodedQuery}&language=tr-TR&page={page}";

                _logger.LogInformation($"TMDB Arama: {query}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"TMDB API hatası: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json);

                _logger.LogInformation($"Arama sonucu: {result?.Results.Count ?? 0} film bulundu");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Film arama hatası: {query}");
                return null;
            }
        }

        public async Task<TmdbSearchResponse?> GetPopularMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"movie/popular?language=tr-TR&page={page}";

                _logger.LogInformation($"TMDB Popüler filmler isteniyor...");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"TMDB API hatası: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json);

                _logger.LogInformation($"Popüler filmler başarıyla alındı: {result?.Results.Count ?? 0} film");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Popüler filmler çekilirken hata");
                return null;
            }
        }

        public async Task<TmdbSearchResponse?> GetNowPlayingMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"movie/now_playing?language=tr-TR&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TmdbSearchResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vizyondaki filmler çekilirken hata");
                return null;
            }
        }

        public async Task<TmdbSearchResponse?> GetUpcomingMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"movie/upcoming?language=tr-TR&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TmdbSearchResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yakında çıkacak filmler çekilirken hata");
                return null;
            }
        }
    }
}