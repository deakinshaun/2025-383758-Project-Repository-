using LudicWorlds;
using Unity.Sentis;
using UnityEngine;

public class LoadEncoderState : SentisWhisperState
{

    public LoadEncoderState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.LoadEncoder, WhisperStateID.LoadSpectro)
    {

    }

    public override void Enter()
    {
        //Debug.Log("-> LoadEncoderState::Enter()");
        DebugPanel.SetStatus("LoadEncoder");
        stage = 0;
    }

    public override void Update()
    {
        switch (stage)
        {
            case 0:
                LoadEncoder();
                stage = 1; 
                break;
            default:
                stateMachine.SetState(nextStateId);
                break;
        }
    }

    private void LoadEncoder()
    {
        Model encoder = ModelLoader.Load( whisper.encoderAsset);
        //whisper.EncoderEngine = WorkerFactory.CreateWorker(backend, encoder); //V1.6
        whisper.EncoderEngine = new Worker(encoder, backend); //V2.0
    }

    public override void Exit()
    {
        base.Exit();
    }
}