using UnityEngine;
using TMPro;

public class GPSWeatherDisplay : MonoBehaviour
{
    public GPSTracking gps;
    public WeatherAPIClient weather;
    public TextMeshProUGUI textElement;       // Main GPS + Weather display
    public TextMeshProUGUI instructionText;   // Dynamic instruction display

    void Start()
    {
        InvokeRepeating("UpdateDisplay", 2f, 5f);
    }

    void UpdateDisplay()
    {
        if (gps == null || weather == null || textElement == null || instructionText == null)
        {
            Debug.LogWarning("GPSWeatherDisplay: Missing references.");
            return;
        }

        // GPS Info
        string gpsText = $"📍 <b>GPS Data</b>\n" +
                         $"Latitude: {gps.latitude:F6}\n" +
                         $"Longitude: {gps.longitude:F6}\n" +
                         $"Altitude: {gps.altitude:F2} m\n\n";

        // Weather Info
        string weatherText = "Weather data loading...";
        string instruction = "Analyzing weather...";

        if (weather.LatestWeather != null)
        {
            var w = weather.LatestWeather;

            weatherText = $"🌦️ <b>Weather Data</b>\n" +
                          $"Temperature: {w.temperature_2m}°C\n" +
                          $"Feels Like: {w.apparent_temperature}°C\n" +
                          $"Humidity: {w.relative_humidity_2m}%\n" +
                          $"Rain: {w.rain} mm\n" +
                          $"Precipitation: {w.precipitation} mm\n" +
                          $"Daytime: {(w.is_day == 1 ? "Yes" : "No")}";

            // ✅ Task 3: Show instruction based on weather
            if (w.rain > 0f)
                instruction = "🌧️ It's raining — take an umbrella!";
            else if (w.is_day == 0)
                instruction = "🌙 It's nighttime — stay safe.";
            else if (w.temperature_2m < 10f)
                instruction = "🧥 It's cold — wear something warm.";
            else
                instruction = "☀️ Weather looks good — enjoy your day!";
        }

        textElement.text = gpsText + weatherText;
        instructionText.text = instruction;
    }
}
