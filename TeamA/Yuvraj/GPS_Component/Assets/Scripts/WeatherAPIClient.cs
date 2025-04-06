using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherAPIClient : MonoBehaviour
{
    [SerializeField] private float latitude = 52.52f;
    [SerializeField] private float longitude = 13.41f;

    private string baseUrl = "https://api.open-meteo.com/v1/forecast";

    void Start()
    {
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        string url = $"{baseUrl}?latitude={latitude}&longitude={longitude}" +
                     "&current=temperature_2m,relative_humidity_2m,precipitation,rain,apparent_temperature,is_day";

        Debug.Log("Requesting weather data from: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching weather data: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Received weather data: " + jsonResponse);

            WeatherResponse weatherData = JsonUtility.FromJson<WeatherResponse>(jsonResponse);

            if (weatherData != null && weatherData.current != null)
            {
                Debug.Log("Temperature: " + weatherData.current.temperature_2m + "°C");
                Debug.Log("Humidity: " + weatherData.current.relative_humidity_2m + "%");
                Debug.Log("Precipitation: " + weatherData.current.precipitation + "mm");
                Debug.Log("Rain: " + weatherData.current.rain + "mm");
                Debug.Log("Feels Like: " + weatherData.current.apparent_temperature + "°C");
                Debug.Log("Daytime: " + (weatherData.current.is_day == 1 ? "Yes" : "No"));
            }
            else
            {
                Debug.LogError("Failed to parse weather data.");
            }
        }
    }
}

[System.Serializable]
public class WeatherResponse
{
    public CurrentWeather current;
}

[System.Serializable]
public class CurrentWeather
{
    public float temperature_2m;
    public float relative_humidity_2m;
    public float precipitation;
    public float rain;
    public float apparent_temperature;
    public int is_day;
}
