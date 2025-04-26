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


    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            Server_SetState(EGameState.Pregame);
        }

        StateMachine[EGameState.Pregame].onEnter = prev =>
        {
            if (prev == EGameState.Postgame)
            {
                if (Runner.IsServer)
                {
                    Runner.LoadScene("Menu");
                    if (!Runner.SessionInfo.IsOpen) Runner.SessionInfo.IsOpen = true;
                }
            }
        };

        StateMachine[EGameState.Pregame].onExit = next =>
        {
            if (Runner.SessionInfo.IsOpen) Runner.SessionInfo.IsOpen = false;
        };

        StateMachine[EGameState.Loading].onEnter = prev =>
        {
            if (prev == EGameState.Pregame)
            {
                if (Runner.IsServer)
                {
                    Runner.LoadScene(Runner.SessionInfo.Properties["Scene"]);
                }
            }
        };

        StateMachine[EGameState.Loading].onUpdate = () =>
        {
            if (Runner.IsServer)
            {
                if (PlayerRegistry.All(p => p.IsLoaded, true))
                {
                    Server_SetState(EGameState.Intro);
                }
            }
        };

        StateMachine[EGameState.Loading].onExit = next =>
        {
            if (Runner.IsServer)
            {
                PlayerRegistry.ForEach(p => p.IsLoaded = false, true);
                PlayerRegistry.ForEach((p, i) =>
                {
                    Runner.Spawn(ResourcesManager.Instance.playerControllerPrefab, Level.Current.GetSpawnPosition(i,p.Team),
                        inputAuthority: p.Ref);
                    //TODO:Spawn each player on avialable spawn locations
                });
            }
        };


        StateMachine[EGameState.Intro].onEnter = prev =>
        {
            if (Runner.IsServer)
            {
                PlayerRegistry.ForEach(p => { });
            }
            //Maybe show some cinematic camera movement
            //TODO:Start countdown for game
            Server_DelaySetState(EGameState.Game, 3);
        };

        StateMachine[EGameState.Game].onEnter = prev =>
        {
            GameManager.Instance.TickStarted = Runner.Tick;
            
        };


        StateMachine[EGameState.Game].onUpdate = () =>
        {
            //Broadcast tick timer
            // HUD.SetTimerText(GameManager.Time);
            if (Runner.IsServer && GameManager.Time >= GameManager.Instance.MaxTime)
            {
                Debug.Log("Time's up");
            }
        };

        StateMachine[EGameState.Outro].onEnter = prev =>
        {
            // GameManager.CalculateScores();
            // UIScreen.activeScreen.BackTo(InterfaceManager.Instance.hud);
            // UIScreen.Focus(InterfaceManager.Instance.scoreboard);
            // UIScreen.Focus(InterfaceManager.Instance.performance.screen);
            //
            // GameManager.Instance.TickStarted = 0;
        };

        StateMachine[EGameState.Outro].onExit = next =>
        {
            // UIScreen.activeScreen.Back();
        };


        StateMachine[EGameState.Postgame].onEnter = prev =>
        {
            // Unload Level
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
        //Debug.Log($"Set State to {st}");
        Previous = Current;
        Current = state;
    }

    public void Server_DelaySetState(EGameState newState, float delay)
    {
        Debug.Log($"Delay state change to {newState} for {delay}s");
        Delay = TickTimer.CreateFromSeconds(Runner, delay);
        DelayedState = newState;
    }
}