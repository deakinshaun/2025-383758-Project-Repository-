using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameProperty", fileName = "GameProperty")]
public class GameProperty : ScriptableObject
{
    public string gameModeName;
    public Sport sport;
    public int matchDuration;
    public int teams = 2;
    public int playersPerTeam = 5;
    public int MaxPlayerCount => teams * playersPerTeam;

    public Dictionary<string, SessionProperty> GetSessionProperties()
    {
        var props = new Dictionary<string, SessionProperty>()
        {
            ["Sport"] = (int)sport,
            ["Teams"] = teams,
            ["PlayersPerTeam"] = playersPerTeam,
            ["MatchDuration"] = matchDuration,
            ["MaxPlayers"] = MaxPlayerCount,
            ["GameMode"] = gameModeName,
            ["Scene"] = scenePath
        };
        return props;
    }

    private int _nextTeamIndex = 0;
    /// <summary>
    /// Assigns team in round robin
    /// </summary>
    public int AssignTeam(PlayerRef player)
    {
        int team = (_nextTeamIndex % teams) + 1;
        _nextTeamIndex++;
        return team;
    }

    [ScenePath]
    public string scenePath;
    public Texture2D previewImage;
    [TextArea(5, 20)] public string description;
}

public enum Sport
{
    Basketball,
    Volleyball,
    Fencing
}