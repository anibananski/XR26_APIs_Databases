using UnityEngine;
using System;
using WeatherApp.Data;

namespace WeatherApp.Events
{
    /// <summary>
    /// Global event manager for broadcasting weather data and status events.
    /// Acts as a central hub between API and UI without direct coupling.
    /// </summary>
    public class WeatherEventManager : MonoBehaviour
    {
        // Singleton instance
        public static WeatherEventManager Instance { get; private set; }

        private void Awake()
        {
            // Ensure there's only one instance
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Event triggered when valid weather data is received.
        /// </summary>
        public event Action<WeatherData> OnWeatherDataReceived;

        /// <summary>
        /// Event triggered when a weather data request fails.
        /// Sends an error message string.
        /// </summary>
        public event Action<string> OnWeatherRequestFailed;

        /// <summary>
        /// Call this to notify listeners that weather data has been received.
        /// </summary>
        /// <param name="data">The valid WeatherData object</param>
        public void WeatherDataReceived(WeatherData data)
        {
            OnWeatherDataReceived?.Invoke(data);
        }

        /// <summary>
        /// Call this to notify listeners of a failed API request.
        /// </summary>
        /// <param name="errorMessage">A human-readable error message</param>
        public void WeatherRequestFailed(string errorMessage)
        {
            OnWeatherRequestFailed?.Invoke(errorMessage);
        }
    }
}

