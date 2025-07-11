using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSupportBot
{
    public class WeatherService
    {

        private static readonly string apiKey = "e267334b3fddd34838ac9e85f2a0bad4"; // Replace with your API key
        private static readonly string geoApiUrl = "https://api.openweathermap.org/geo/1.0/direct?q={0}&limit=1&appid={1}";
        private static readonly string weatherApiUrl = "https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}";

        public static async Task<string> GetWeatherAsync(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                // Step 1: Get Latitude & Longitude
                string geoUrl = string.Format(geoApiUrl, city, apiKey);
                HttpResponseMessage geoResponse = await client.GetAsync(geoUrl);
                if (!geoResponse.IsSuccessStatusCode) return "Error fetching location data.";

                string geoJson = await geoResponse.Content.ReadAsStringAsync();
                JArray geoData = JArray.Parse(geoJson);
                if (geoData.Count == 0) return "Location not found.";

                double latitude = (double)geoData[0]["lat"];
                double longitude = (double)geoData[0]["lon"];

                // Step 2: Get Weather Data
                string weatherUrl = string.Format(weatherApiUrl, latitude, longitude, apiKey);
                HttpResponseMessage weatherResponse = await client.GetAsync(weatherUrl);
                if (!weatherResponse.IsSuccessStatusCode) return "Error fetching weather data.";

                string weatherJson = await weatherResponse.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(weatherJson);

                string city1 = obj["name"]?.ToString() ?? "Unknown";
                string country = obj["sys"]?["country"]?.ToString() ?? "N/A";
                string main = obj["weather"]?[0]?["main"]?.ToString() ?? "N/A";
                string desc = obj["weather"]?[0]?["description"]?.ToString() ?? "N/A";

                double tempKelvin = double.Parse(obj["main"]?["temp"]?.ToString() ?? "0");
                double tempCelsius = tempKelvin - 273.15;

                string humidity = obj["main"]?["humidity"]?.ToString() ?? "N/A";
                string pressure = obj["main"]?["pressure"]?.ToString() ?? "N/A";
                string windSpeed = obj["wind"]?["speed"]?.ToString() ?? "N/A";

                long sunriseUnix = obj["sys"]?["sunrise"]?.Value<long>() ?? 0;
                long sunsetUnix = obj["sys"]?["sunset"]?.Value<long>() ?? 0;

                DateTime sunriseTime = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix).ToLocalTime().DateTime;
                DateTime sunsetTime = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix).ToLocalTime().DateTime;

                string sunriseStr = sunriseTime.ToString("hh:mm tt"); // e.g., "05:30 AM"
                string sunsetStr = sunsetTime.ToString("hh:mm tt");   // e.g., "06:45 PM"

                string weatherInfo = $@"
📍 Location: {city}, {country}
🌤️ Weather: {main} ({desc})
🌡️ Temperature: {tempCelsius:F1}°C
💧 Humidity: {humidity}%
🌀 Wind Speed: {windSpeed} m/s
🎯 Pressure: {pressure} hPa
🌅 Sunrise: {sunriseStr}
🌇 Sunset: {sunsetStr}
";

                return weatherInfo;
            }
        }
    }
}

