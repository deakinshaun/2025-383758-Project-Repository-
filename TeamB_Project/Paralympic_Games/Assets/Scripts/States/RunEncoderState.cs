using LudicWorlds;
using Unity.Sentis;
using UnityEngine;
using System.Collections;

public class RunEncoderState : SentisWhisperState
{
    //ref: https://docs.unity3d.com/Packages/com.unity.sentis@2.0/manual/split-inference-over-multiple-frames.html

    IEnumerator m_Schedule;
    private int layerCount = 0;

    Tensor<float> encodedAudio;

    // Start is called before the first frame update
    public RunEncoderState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.RunEncoder, WhisperStateID.RunDecoder)
    {
    }

    public override void Enter()
    {
        Debug.Log("-> RunEncoderState::Enter()");
        DebugPanel.SetStatus("RunEncoder");
        stage = 0;
        m_Schedule = null;
        layerCount = 0;
    }

    public override void Update()
    {
        switch (stage)
        {
            case 0:
                StartModel();
                break;
            case 1:
                ExecuteLayer();
                break;
            case 2:
                ReadOutput();
                break;
            default:
                stateMachine.SetState(nextStateId);
                break;
        }
    }

    private void StartModel()
    {
        m_Schedule = whisper.EncoderEngine.ScheduleIterable( whisper.SpectroOutput );
        stage = 1;
    }

    private void ExecuteLayer()
    {
        layerCount++;

        if (!m_Schedule.MoveNext())
        {
            stage = 2;
        }
    }

    private void ReadOutput()
    {
        Debug.Log("-> ReadOutput() - Number of layers: " + layerCount);
        encodedAudio = whisper.EncoderEngine.PeekOutput() as Tensor<float>;
        whisper.EncodedAudio = encodedAudio.ReadbackAndClone();
        stage = 3;
    }

    public override void Exit()
    {
        encodedAudio?.Dispose();
        whisper.SpectroOutput?.Dispose();
        base.Exit();
    }
}