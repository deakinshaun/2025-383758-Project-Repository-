using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace OrgilFolder.Scripts
{
    public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public static GameState State => Instance._gameState;
        [SerializeField] private GameState _gameState;
        public static GameManager Instance { get; private set; }

        public float MaxTime => Runner.SessionInfo.Properties["MatchDuration"];

        private void Awake()
        {
            Instance = this;
        }

        public static float Time
        {
            get
            {
                if (Instance?.Object?.IsValid == true)
                {
                    if (Instance.TickStarted != 0)
                    {
                        return (Instance.Runner.Tick - Instance.TickStarted) * Instance.Runner.DeltaTime;
                    }
                }

                return 0;
            }
        }

        public int TickStarted { get; set; }

        public override void Spawned()
        {
            Instance = this;
            Runner.AddCallbacks(this);
            if (Runner.IsServer)
            {
                // SessionSetup.
            }

            if (State.Current < GameState.EGameState.Loading)
            {
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Instance = null;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void Rpc_LoadDone(RpcInfo info = default)
        {
            PlayerRegistry.GetPlayer(info.Source).IsLoaded = true;
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason != ShutdownReason.Ok)
                DisconnectUI.OnShutdown(shutdownReason);
        }

        #region UnusedCallbacks

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }


        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
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

        #endregion
    }
}