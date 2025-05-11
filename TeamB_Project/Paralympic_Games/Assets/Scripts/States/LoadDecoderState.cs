using LudicWorlds;
using Unity.Sentis;
using UnityEngine;

public class LoadDecoderState : SentisWhisperState
{

    public LoadDecoderState(IStateMachine<WhisperStateID> stateMachine) : base(stateMachine, WhisperStateID.LoadDecoder, WhisperStateID.LoadEncoder)
    {
        
    }

    public override void Enter()
    {
        //Debug.Log("-> LoadDecoderState::Enter()");
        DebugPanel.SetStatus("LoadDecoder");
        stage = 0;
    }

    public override void Update()
    {
        switch(stage)
        {
            case 0:
                LoadDecoder();
                stage = 1;
                break;
            default:
                stateMachine.SetState( nextStateId );
                break;
        }      
    }

    private void LoadDecoder() 
    {
        Model decoder = ModelLoader.Load(whisper.decoderAsset);

        // Define the functional graph of the model.
        var graph = new FunctionalGraph();

        // Set the inputs of the graph from the original model inputs and return an array of functional tensors
        var inputs = graph.AddInputs(decoder);

        // Apply the model forward function to the inputs to get the source model functional outputs.
        // Sentis will destructively change the loaded source model. To avoid this at the expense of
        // higher memory usage and compile time, use the Functional.ForwardWithCopy method.
        FunctionalTensor[] outputs = Functional.Forward(decoder, inputs);

        // Calculate the argMax of the first output with the functional API.
        FunctionalTensor argmaxOutput = Functional.ArgMax(outputs[0],2);

        // Build the model from the graph using the `Compile` method with the desired outputs.
        Model decoderWithArgMax = graph.Compile(argmaxOutput);

        whisper.DecoderEngine = new Worker(decoderWithArgMax, backend);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
