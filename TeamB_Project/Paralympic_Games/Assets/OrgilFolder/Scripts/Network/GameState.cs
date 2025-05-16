using System;
using Fusion;
using OrgilFolder.Scripts;
using OrgilFolder.Scripts.GamePlay;
using OrgilFolder.Scripts.GamePlay.GameModes.Basketball;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    public enum EGameState
    {
        Off,
        Pregame,
        Loading,
        Intro,
        Game,
        Outro,
        Postgame
    }

    [Networked] [field: ReadOnly] public EGameState Previous { get; set; }
    [Networked] [field: ReadOnly] public EGameState Current { get; set; }
    [Networked] TickTimer Delay { get; set; }
    [Networked] EGameState DelayedState { get; set; }

    protected StateMachine<EGameState> StateMachine = new();

    public float DelayRemainingTime => Delay.RemainingTime(Runner).Value;
    public event Action<EGameState> onSetState;
    public event Action<EGameState, float> onSetDelaydState;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            Server_SetState(EGameState.Pregame);
        }

        StateMachine[EGameState.Pregame].onEnter = prev =>
        {
            if (prev != EGameState.Postgame) return;
            if (!Runner.IsServer) return;
            Runner.LoadScene("Menu");
            if (!Runner.SessionInfo.IsOpen) Runner.SessionInfo.IsOpen = true;
        };

        StateMachine[EGameState.Pregame].onExit = next =>
        {
            if (Runner.SessionInfo.IsOpen) Runner.SessionInfo.IsOpen = false;
        };

        StateMachine[EGameState.Loading].onEnter = prev =>
        {
            if (prev != EGameState.Pregame) return;
            if (Runner.IsServer)
            {
                Runner.LoadScene(Runner.SessionInfo.Properties["Scene"]);
            }
        };

        StateMachine[EGameState.Loading].onUpdate = () =>
        {
            if (!Runner.IsServer) return;
            if (PlayerRegistry.All(p => p.IsLoaded, true))
            {
                Server_SetState(EGameState.Intro);
            }
        };

        StateMachine[EGameState.Loading].onExit = next =>
        {
            if (!Runner.IsServer) return;
            PlayerRegistry.ForEach(p => p.IsLoaded = false, true);
            PlayerRegistry.ForEach((p, i) =>
            {
                (Vector3, Quaternion) spawnPosRot = Level.Current.GetSpawnPositionAndRotation(i, p.Team);
                var playerController = Runner.Spawn(ResourcesManager.Instance.playerControllerPrefab, position: spawnPosRot.Item1,
                    rotation: spawnPosRot.Item2,
                    inputAuthority: p.Ref);
                playerController.playerObject = p;
            });
        };


        StateMachine[EGameState.Intro].onEnter = prev =>
        {
            if (Runner.IsServer)
            {
                PlayerRegistry.ForEach(p => { });
            }
            Server_DelaySetState(EGameState.Game, 6f);
        };

        StateMachine[EGameState.Game].onEnter = prev =>
        {
            GameManager.Instance.TickStarted = Runner.Tick;
            BasketballGameRule.Instance.SpawnBallNeutral();
        };


        StateMachine[EGameState.Game].onUpdate = () =>
        {
            if (Runner.IsServer && GameManager.Time >= GameManager.Instance.MaxTime)
            {
                Server_SetState(EGameState.Outro);
            }
        };

        StateMachine[EGameState.Outro].onEnter = prev =>
        {
    
        };

        StateMachine[EGameState.Outro].onExit = next =>
        {
        };


        StateMachine[EGameState.Postgame].onEnter = prev =>
        {
            Server_DelaySetState(EGameState.Pregame, 5);
        };

        StateMachine[EGameState.Postgame].onUpdate = () => { };
        StateMachine[EGameState.Postgame].onExit = next => { PlayerRegistry.ForEach(p => p.ClearGameplayData()); };

        Runner.SetIsSimulated(Object, true);
        StateMachine.Update(Current, Previous);
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer)
        {
            if (Delay.Expired(Runner))
            {
                Delay = TickTimer.None;
                Server_SetState(DelayedState);
            }
        }

        if (Runner.IsForward)
            StateMachine.Update(Current, Previous);
    }

    public void Server_SetState(EGameState state)
    {
        if (Current == state) return;
        Previous = Current;
        Current = state;
        onSetState?.Invoke(Current);
    }

    public void Server_DelaySetState(EGameState newState, float delay)
    {
        Delay = TickTimer.CreateFromSeconds(Runner, delay);
        DelayedState = newState;
        onSetDelaydState?.Invoke(DelayedState, delay);
    }
}