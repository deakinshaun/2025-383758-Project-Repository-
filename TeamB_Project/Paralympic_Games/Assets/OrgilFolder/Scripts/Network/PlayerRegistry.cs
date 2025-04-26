using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using Helpers.Linq;
using UnityEngine;

public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
{
    public const byte CAPACITY = 10;
    public static PlayerRegistry Instance { get; private set; }
    
    
    public static int CountAll => Instance.Object.IsValid ? Instance.ObjectByRef.Count : 0;
    public static int CountPlayers => Instance.Object.IsValid ? CountWhere(p => !p.IsSpectator) : 0;
    public static int CountSpectators => Instance.Object.IsValid ? CountWhere(p => p.IsSpectator) : 0;
    public static event System.Action<NetworkRunner, PlayerRef> onPlayerJoined;
    public static event System.Action<NetworkRunner, PlayerRef> onPlayerLeft;
    
    public static IEnumerable<PlayerObject> Everyone => Instance?.Object?.IsValid == true ? Instance.ObjectByRef.Select(kvp => kvp.Value) : Enumerable.Empty<PlayerObject>();
    public static IEnumerable<PlayerObject> Players => Instance?.Object?.IsValid == true ? Instance.ObjectByRef.Where(kvp => kvp.Value && !kvp.Value.IsSpectator).Select(kvp => kvp.Value) : Enumerable.Empty<PlayerObject>();

    
    [Networked, Capacity(CAPACITY)]
    NetworkDictionary<PlayerRef, PlayerObject> ObjectByRef { get; }

    private void Awake()
    {
	    
        Instance = this;
    }

    public override void Spawned()
    {
        Instance = this;
        Runner.AddCallbacks(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Instance = null;
        runner.RemoveCallbacks(this);
        onPlayerJoined = onPlayerLeft = null;
    }
    
    bool GetAvailable(out byte index)
    {
        if (ObjectByRef.Count == 0)
        {
            index = 0;
            return true;
        }
        else if (ObjectByRef.Count == CAPACITY)
        {
            index = default;
            return false;
        }

        byte[] indices = ObjectByRef.OrderBy(kvp => kvp.Value.Index).Select(kvp => kvp.Value.Index).ToArray();

        for (int i = 0; i < indices.Length - 1; i++)
        {
            if (indices[i + 1] > indices[i] + 1)
            {
                index = (byte)(indices[i] + 1);
                return true;
            }
        }

        index = (byte)(indices[indices.Length - 1] + 1);
        return true;
    }


    public static void Server_Add(NetworkRunner runner, PlayerRef playerRef, PlayerObject playerObject)
    {
        Debug.Assert(runner.IsServer);

        if (Instance.GetAvailable(out byte index))
        {
            Instance.ObjectByRef.Add(playerRef, playerObject);
            DontDestroyOnLoad(playerObject.gameObject);
            playerObject.Server_Init(playerRef,index);
        }
        else
        {
            Debug.LogWarning($"Unable to register player {playerRef}",playerObject);
        }
    }

    public static void Server_Remove(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Assert(runner.IsServer);
        Debug.Assert(playerRef.IsRealPlayer); 
        
        Debug.Log($"Removing Player {playerRef}");

        if (Instance.ObjectByRef.Remove(playerRef) == false)
        {
            Debug.LogWarning("Could not remove player from registry");
        }
    }
    
    public static void PlayerJoined(PlayerRef player)
    {
	    onPlayerJoined?.Invoke(Instance.Runner, player);
    }

    public static bool HasPlayer(PlayerRef pRef)
    {
        return Instance.ObjectByRef.ContainsKey(pRef);
    }

    public static PlayerObject GetPlayer(PlayerRef pRef)
    {
        if (HasPlayer(pRef))
            return Instance.ObjectByRef.Get(pRef);
        return null;
    }
    public static bool IsHost(PlayerRef pRef)
    {
        return GetPlayer(pRef)?.Index == 0;
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
	    if(runner.IsServer) Server_Remove(runner,player);
        onPlayerLeft?.Invoke(Instance.Runner,player);
    }

    #region Utility Methods
    
    public static IEnumerable<PlayerObject> Where(System.Predicate<PlayerObject> match, bool includeSpectators = false)
	{
		return (includeSpectators ? Everyone : Players).Where(p => match.Invoke(p));
		
		//return Instance.ObjectByRef.Where(kvp => match.Invoke(kvp.Value)).Select(kvp => kvp.Value);
	}

	public static PlayerObject First(System.Predicate<PlayerObject> match, bool includeSpectators = false)
	{
		return (includeSpectators ? Everyone : Players).First(p => match.Invoke(p));
	}

	public static void ForEach(System.Action<PlayerObject> action, bool includeSpectators = false)
	{
		foreach (var p in (includeSpectators ? Everyone : Players))
		{
			action.Invoke(p);
		}
	}

	public static void ForEach(System.Action<PlayerObject, int> action, bool includeSpectators = false)
	{
		int i = 0;
		(includeSpectators ? Everyone : Players).ForEach(p => action.Invoke(p, i++));
	}

	public static void ForEachWhere(System.Predicate<PlayerObject> match, System.Action<PlayerObject> action, bool includeSpectators = false)
	{
		(includeSpectators ? Everyone : Players).Where(p => match.Invoke(p)).ForEach(p => action.Invoke(p));
	}

	public static int CountWhere(System.Predicate<PlayerObject> match, bool includeSpectators = false)
	{
		return (includeSpectators ? Everyone : Players).Where(p => match.Invoke(p)).Count();
	}

	public static bool Any(System.Predicate<PlayerObject> match, bool includeSpectators = false)
	{
		if (Instance == null) return false;
		return (includeSpectators ? Everyone : Players).Where(p => match.Invoke(p)).Count() > 0;
	}

	public static bool All(System.Predicate<PlayerObject> match, bool includeSpectators = false)
	{
		return (includeSpectators ? Everyone : Players).Where(p => !match.Invoke(p)).Count() == 0;
	}

	public static IOrderedEnumerable<PlayerObject> OrderAsc<T>(
		System.Func<PlayerObject, T> selector,
		System.Predicate<PlayerObject> match = null,
		bool includeSpectators = false) where T : System.IComparable<T>
	{
		if (match != null) return (includeSpectators ? Everyone : Players).Where(p => match.Invoke(p)).OrderBy(selector);
		return (includeSpectators ? Everyone : Players).OrderBy(selector);
	}

	public static IOrderedEnumerable<PlayerObject> OrderDesc<T>(
		System.Func<PlayerObject, T> selector, 
		System.Predicate<PlayerObject> match = null, 
		bool includeSpectators = false) where T : System.IComparable<T>
	{
		if (match != null) return (includeSpectators ? Everyone : Players).Where(p => match.Invoke(p)).OrderByDescending(selector);
		return (includeSpectators ? Everyone : Players).OrderByDescending(selector);
	}
	
    #endregion
    
    #region UnusedCallbacks
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
	    
    }

    #endregion
}