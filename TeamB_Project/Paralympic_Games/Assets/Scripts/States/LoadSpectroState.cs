using LudicWorlds;
using Unity.Sentis;
using UnityEngine;

public class LoadSpectroState : SentisWhisperState
{

    public LoadSpectroState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.LoadSpectro, WhisperStateID.Ready)
    {

    }

    public override void Enter()
    {
        //Debug.Log("-> LoadSpectroState::Enter()");
        DebugPanel.SetStatus("LoadSpectro");
        stage = 0;
    }

    public override void Update()
    {
        switch (stage)
        {
            case 0:
                LoadSpectro();
                stage = 1;
                break;
            default:
                stateMachine.SetState(nextStateId);
                break;
        }
    }


    private void LoadSpectro()
    {
        Model spectro = ModelLoader.Load(whisper.spectroAsset);
        //whisper.SpectroEngine = WorkerFactory.CreateWorker(backend, spectro);
        whisper.SpectroEngine = new Worker(spectro, backend);
    }


    public override void Exit()
    {
        base.Exit();
    }
}