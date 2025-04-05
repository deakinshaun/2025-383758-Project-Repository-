using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance;
    public NetworkRunner runnerPrefab;
    public NetworkRunner Runner => runner;
    private NetworkRunner runner;

    public event Action<List<SessionInfo>> onSessionListUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        runner = Instantiate(runnerPrefab);
        runner.AddCallbacks(this);
        runner.JoinSessionLobby(SessionLobby.ClientServer);
    }

    public async void JoinLobby(SessionInfo session)
    {
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = session.Name
        });
    }
    public async void StartHost(string sessionName, GameMode mode, GameProperty gameProperty)
    {
        var customProps = new Dictionary<string, SessionProperty>();

        customProps["scene"] = gameProperty.sceneIndex;
        customProps["gameMode"] = gameProperty.gameModeName;
        customProps["maxPlayers"] = gameProperty.maxPlayerCount;
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            SessionProperties = customProps,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            Scene = SceneRef.FromIndex(gameProperty.sceneIndex),
            IsVisible = true
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"SessionUpdated: {sessionList.ToString()}");
        onSessionListUpdated?.Invoke(sessionList);
    }

    #region Unused Callbacks

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

    #endregion
}