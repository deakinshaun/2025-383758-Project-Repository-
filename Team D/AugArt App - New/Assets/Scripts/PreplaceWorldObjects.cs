using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.WorldPositioning;
using UnityEngine;
using Niantic.Lightship.AR.XRSubsystems;

public class PreplaceWorldObjects : MonoBehaviour
{

    // [SerializeField] private List<Material> _materials = new();
    [SerializeField] private List<GameObject> _possibleObjectsToPlace = new();
    [SerializeField] private List<LatLong> _latLongs = new();
    [SerializeField] private ARWorldPositioningManager _positioningManager;
    [SerializeField] private ARWorldPositioningObjectHelper _objectHelper;


    private List<GameObject> instantiatedObjects = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var gpsCoord in _latLongs)
        {
            GameObject newObject =
                Instantiate(_possibleObjectsToPlace[_latLongs.IndexOf(gpsCoord) % _possibleObjectsToPlace.Count]);

            _objectHelper.AddOrUpdateObject(newObject, gpsCoord.latitude, gpsCoord.longitude, 0, Quaternion.identity);

            Debug.Log($"Added {newObject.name} with latitude {gpsCoord.latitude} and longitude {gpsCoord.longitude}.");
        }

        _positioningManager.OnStatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(WorldPositioningStatus status)
    {
        Debug.Log("Status changed to " + status);

    }

}


[System.Serializable]
public struct LatLong
{
    public double latitude;
    public double longitude;
}