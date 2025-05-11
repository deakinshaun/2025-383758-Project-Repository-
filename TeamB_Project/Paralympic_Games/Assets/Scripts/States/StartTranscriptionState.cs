using LudicWorlds;
using UnityEngine;
using UnityEngine.Rendering;

public class StartTranscriptionState : SentisWhisperState
{
    // Maximum size of audioClip (30s at 16kHz)
    const int maxSamples = 30 * 16000;

    // Start is called before the first frame update

    public StartTranscriptionState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.StartTranscription, WhisperStateID.RunSpectro)
    {
    }

    public override void Enter()
    {
        Debug.Log("-> StartTranscriptionState::Enter()");
        DebugPanel.SetStatus("StartTranscription");
        whisper.SpeechText.color = Color.gray;
        whisper.SpeechText.text = "Transcribing ...";
        LoadAudio();
    }

    private void LoadAudio()
    {
        if (whisper.AudioClip.frequency != 16000)
        {
            Debug.Log($"The audio clip should have frequency 16kHz. It has frequency {whisper.AudioClip.frequency / 1000f}kHz");
            return;
        }

        whisper.NumSamples = whisper.AudioClip.samples;

        if (whisper.NumSamples > maxSamples)
        {
            Debug.Log($"The AudioClip is too long. It must be less than 30 seconds. This clip is {whisper.NumSamples / whisper.AudioClip.frequency} seconds.");
            return;
        }

        whisper.Data = new float[maxSamples];
        whisper.NumSamples = maxSamples;

        //We will get a warning here if data.length is larger than audio length but that is OK
        whisper.AudioClip.GetData(whisper.Data, 0);
    }

    public override void Update()
    {
        stateMachine.SetState(nextStateId);
    }
}
