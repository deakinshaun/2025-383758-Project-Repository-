using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherAPIClient : MonoBehaviour
{
    [SerializeField] private GPSTracking gps; // Assign via Inspector
    public CurrentWeather LatestWeather { get; private set; }

    private string baseUrl = "https://api.open-meteo.com/v1/forecast";

    void Start()
    {
        StartCoroutine(DelayedFetch());
    }

    IEnumerator DelayedFetch()
    {
        yield return new WaitForSeconds(6); // wait for GPS
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        float lat = gps.latitude;
        float lon = gps.longitude;

        string url = $"{baseUrl}?latitude={lat}&longitude={lon}" +
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
                LatestWeather = weatherData.current;
                Debug.Log("✅ Weather updated");
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