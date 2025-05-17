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
        string gpsText = $" <b>GPS Data</b>\n" +
                         $"Latitude: {gps.latitude:F1} " +
                         $"Longitude: {gps.longitude:F1} " +
                         $"Altitude: {gps.altitude:F1} ";

        // Weather Info
        string weatherText = "Weather data loading...";
        string instruction = "Analyzing weather...";

        if (weather.LatestWeather != null)
        {
            var w = weather.LatestWeather;

            weatherText = $"<b>Weather:</b> {w.temperature_2m}°C (Feels like {w.apparent_temperature}°C), " +
               $"Humidity {w.relative_humidity_2m}%, Rain {w.rain}mm, Precip {w.precipitation}mm, " +
               $"Daytime: {(w.is_day == 1 ? "Yes" : "No")}";


            // Task 3: Show instruction based on weather
            if (w.rain > 0f)
                instruction = "️ It's raining — take an umbrella!";
            else if (w.is_day == 0)
                instruction = " It's nighttime — stay safe.";
            else if (w.temperature_2m < 10f)
                instruction = " It's cold — wear something warm.";
            else
                instruction = " Weather looks good — enjoy your day!";
        }

        textElement.text = gpsText + weatherText;
        instructionText.text = instruction;
    }
}
