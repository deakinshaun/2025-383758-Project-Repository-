using UnityEngine;
using TMPro;

public class AutoManager : MonoBehaviour
{
    public GPSTracking gpsTracking;          // Reference to your GPS tracking script
    public AutoLocation[] autolocations;  // List of all auto locations
    public TextMeshProUGUI autoText;     // UI Text to display nearest auto
    public float searchRadius = 10000f;      // 10 km search radius

    void Update()
    {
        float currentLat, currentLon, currentAlt;
        gpsTracking.retrieveLocation(out currentLat, out currentLon, out currentAlt);

        AutoLocation nearestAuto = null;
        float nearestDistance = searchRadius; // Start with the max search radius

        foreach (AutoLocation auto in autolocations)
        {
            float distance = CalculateDistance(currentLat, currentLon, auto.latitude, auto.longitude);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestAuto = auto;
            }
        }

        // Display the nearest auto
        if (nearestAuto != null)
        {
            autoText.text = $"Nearest AutoPart Store:- {nearestAuto.name}\nDistance: {nearestDistance:F2} meters";
        }
        else
        {
            autoText.text = "No nearby auto part store found.";
        }
    }

    // Calculate the distance using Haversine Formula
    float CalculateDistance(float lat1, float lon1, double lat2, double lon2)
    {
        float R = 6371000; // Earth radius in meters
        float dLat = Mathf.Deg2Rad * (float)(lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (float)(lon2 - lon1);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * (float)lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }
}


