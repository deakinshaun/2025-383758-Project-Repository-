using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Matchmaker : MonoBehaviour, INetworkRunnerCallbacks
{
    public static Matchmaker Instance;
    public NetworkRunner runnerPrefab;
    public NetworkObject managerPrefab;
    public UnityEvent onTryJoinLobby;
    public UnityEvent onLobbyConnected;
    public UnityEvent onCloseLobby_Before;
    public UnityEvent onCloseLobby_After;

    public NetworkRunner Runner { get; private set; }
    public event Action<List<SessionInfo>> onSessionListUpdated;
    
    bool _private = false;
    string _roomCode = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    
    public void TryHostSession(GameProperty property,System.Action successCallback = null)
    {
        StartCoroutine(HostSessionRoutine(property,successCallback));
    }

    IEnumerator HostSessionRoutine(GameProperty property,System.Action successCallback)
    {
        if (!Runner)
        {
            Runner = Instantiate(runnerPrefab);
            Runner.GetComponent<NetworkEvents>().PlayerJoined.AddListener((runner, player) =>
            {
                if (runner.IsServer && runner.LocalPlayer == player)
                {
                    runner.Spawn(managerPrefab);
                }
            });
            Runner.AddCallbacks(this);
        }
		
        string code = string.IsNullOrWhiteSpace(_roomCode) ? RoomCode.Create(6) : _roomCode;

        Task<StartGameResult> task = Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = code,
            SessionProperties = property.GetSessionProperties(),
            SceneManager = Runner.GetComponent<INetworkSceneManager>(),
        });
        while (!task.IsCompleted)
        {
            yield return null;
        }
        StartGameResult result = task.Result;

        if (result.Ok)
        {
            if (successCallback != null)
                successCallback.Invoke();
            else
                Runner.LoadScene(SceneRef.FromPath(property.scenePath));

        }
        else
        {
            DisconnectUI.OnShutdown(result.ShutdownReason);
        }
    }
    public void TryJoinSession(string sessionCode, System.Action successCallback = null)
    {
        StartCoroutine(JoinSessionRoutine(sessionCode, successCallback));
    }

    IEnumerator JoinSessionRoutine(string sessionCode, System.Action successCallback)
    {
        if (Runner) Runner.Shutdown();
        Runner = Instantiate(runnerPrefab);

        Task<StartGameResult> task = Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionCode,
            SceneManager = Runner.GetComponent<INetworkSceneManager>(),
            EnableClientSessionCreation = false,
            //DisableClientSessionCreation = true (Assuming this has been swapped?)
        });
        while (!task.IsCompleted)
        {
            yield return null;
        }
        StartGameResult result = task.Result;

        if (result.Ok)
        {
            if (successCallback != null)
                successCallback.Invoke();
        }
        else
        {
            DisconnectUI.OnShutdown(result.ShutdownReason);
        }
    }

    public void TryJoinLobby()
    {
        StartCoroutine(JoinLobbyRoutine());
    }

    IEnumerator JoinLobbyRoutine()
    {
        onTryJoinLobby?.Invoke();
        Runner = Instantiate(runnerPrefab);
        Runner.AddCallbacks(this);
        Task<StartGameResult> task = Runner.JoinSessionLobby(SessionLobby.ClientServer);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        StartGameResult result = task.Result;
		
        if (result.Ok)
        {
            Debug.Log("Connected to lobby.");
            onLobbyConnected?.Invoke();
        }
        else
        {
            DisconnectUI.OnShutdown(result.ShutdownReason);
        }
    }

    public void CloseLobby()
    {
        StartCoroutine(CloseLobbyRoutine());
    }

    IEnumerator CloseLobbyRoutine()
    {
        onCloseLobby_Before?.Invoke();
        Task task = Runner.Shutdown();
        while (!task.IsCompleted)
        {
            yield return null;
        }
        onCloseLobby_After?.Invoke();
        Runner = null;
    }
    
    public void SetPrivate(bool value)
    {
        _private = value;
    }

    public void SetRoomCode(string code)
    {
        _roomCode = code;
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Runner = null;
        if (shutdownReason != ShutdownReason.Ok)
        {
            DisconnectUI.OnShutdown(shutdownReason);
        }

    }

    #region Unused Callbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
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