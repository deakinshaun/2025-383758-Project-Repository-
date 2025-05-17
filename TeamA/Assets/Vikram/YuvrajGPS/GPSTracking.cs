// using System.Collections;
// using UnityEngine;
// using UnityEngine.Android;

// public class GPSTracking : MonoBehaviour
// {
//     public float latitude = 0.0f;
//     public float longitude = 0.0f;
//     public float altitude = 0.0f;

//     private bool isLocationReady = false;

//     IEnumerator Start()
//     {
//         if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//         {
//             Permission.RequestUserPermission(Permission.FineLocation);
//             yield return new WaitForSeconds(2);
//         }

//         if (!Input.location.isEnabledByUser)
//         {
//             Debug.Log("Location access not enabled by user.");
//             yield break;
//         }

//         Input.location.Start(1f, 1f);

//         int maxWait = 20;
//         while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
//         {
//             yield return new WaitForSeconds(1);
//             maxWait--;
//         }

//         if (Input.location.status == LocationServiceStatus.Failed)
//         {
//             Debug.Log("Unable to determine device location.");
//             yield break;
//         }

//         isLocationReady = true;
//         InvokeRepeating("UpdateLocation", 0f, 5f);
//     }

//     void UpdateLocation()
//     {
//         if (!isLocationReady || Input.location.status != LocationServiceStatus.Running)
//             return;

//         latitude = Input.location.lastData.latitude;
//         longitude = Input.location.lastData.longitude;
//         altitude = Input.location.lastData.altitude;

//         Debug.Log($"Location: {latitude}, {longitude}, Alt: {altitude}m");
//     }
// }




using UnityEngine;
using UnityEngine.Android;

public class GPSTracking : MonoBehaviour
{

    public void retrieveLocation(out float latitude, out float longitude, out float altitude)
    {
        latitude = 0.0f;
        longitude = 0.0f;
        altitude = 0.0f;

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location access has not been enabled");
            return;
        }
        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (Input.location.status == LocationServiceStatus.Stopped)
            {
                Input.location.Start();
            }
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            altitude = Input.location.lastData.altitude;
        }
    }
}