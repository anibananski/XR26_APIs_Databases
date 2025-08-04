using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using WeatherApp.Data;
using WeatherApp.Config;

namespace WeatherApp.Services
{
    /// <summary>
    /// Modern API client for fetching weather data
    /// Students will complete the implementation following async/await patterns
    /// </summary>
    public class WeatherApiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        /// <summary>
        /// Fetch weather data for a specific city using async/await pattern
        /// </summary>
        /// <param name="city">City name to get weather for</param>
        /// <returns>WeatherData object or null if failed</returns>
        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(city))
            {
                Debug.LogError("City name cannot be empty");
                return null;
            }

            // Check if API key is configured
            if (!ApiConfig.IsApiKeyConfigured())
            {
                Debug.LogError("API key not configured. Please set up your config.json file in StreamingAssets folder.");
                return null;
            }

            // Build the complete URL with city and API key
            string url = $"{baseUrl}?q={UnityWebRequest.EscapeURL(city)}&appid={ApiConfig.OpenWeatherMapApiKey}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                // Handle different result types
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        return ParseWeatherData(request.downloadHandler.text);

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError($"Network connection error: {request.error}");
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"HTTP error {request.responseCode}: {request.error}");
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"Data processing error: {request.error}");
                        break;

                    default:
                        Debug.LogError("Unknown error occurred");
                        break;
                }

                return null;
            }
        }

        /// <summary>
        /// Helper method to safely parse weather data JSON into a WeatherData object
        /// </summary>
        /// <param name="jsonString">Raw JSON string from API</param>
        /// <returns>Parsed WeatherData or null if parsing fails</returns>
        private WeatherData ParseWeatherData(string jsonString)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                return JsonConvert.DeserializeObject<WeatherData>(jsonString, settings);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON parsing failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Example usage method - students can use this as reference
        /// </summary>
        private async void Start()
        {
            // Example: Get weather for London
            var weatherData = await GetWeatherDataAsync("London");

            if (weatherData != null && weatherData.IsValid)
            {
                Debug.Log($"Weather in {weatherData.CityName}: {weatherData.TemperatureInCelsius:F1}°C");
                Debug.Log($"Description: {weatherData.PrimaryDescription}");
            }
            else
            {
                Debug.LogError("Failed to get weather data");
            }
        }
    }
}
