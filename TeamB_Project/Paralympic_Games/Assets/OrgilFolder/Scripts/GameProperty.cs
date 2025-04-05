using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/GameProperty", fileName = "GameProperty")]
public class GameProperty : ScriptableObject
{
    public string gameModeName;
    public Sport sport;
    public int maxPlayerCount;
    public int sceneIndex;
    public Texture2D previewImage;
    [TextArea(5, 20)] public string description;
}

public enum Sport
{
    Basketball,
    Volleyball,
    Fencing
}