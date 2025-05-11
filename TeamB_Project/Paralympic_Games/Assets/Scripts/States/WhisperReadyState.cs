using LudicWorlds;
using UnityEngine;

public class WhisperReadyState : SentisWhisperState
{
    // Start is called before the first frame update
    public WhisperReadyState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.Ready, WhisperStateID.Ready)
    {      
    }

    public override void Enter()
    {
        Debug.Log("-> WhisperReadyState::Enter()");
        DebugPanel.SetStatus("Ready");
        whisper.IsReady = true;
    }
}
