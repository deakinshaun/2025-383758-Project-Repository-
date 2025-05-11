using UnityEngine;
using Unity.Sentis;
using TMPro;
using System.Collections.Generic;
using LudicWorlds;

// https://huggingface.co/unity/sentis-whisper-tiny/blob/main/RunWhisper.cs

public enum WhisperStateID
{
    LoadDecoder,
    LoadEncoder,
    LoadSpectro,
    Ready,
    StartTranscription,
    RunSpectro,
    RunEncoder,
    RunDecoder
}


public class RunWhisper : GameObjectStateMachine<WhisperStateID>
{
    public ModelAsset decoderAsset;
    public ModelAsset encoderAsset;
    public ModelAsset spectroAsset;
    public TextAsset vocabJson;

    public Worker DecoderEngine { get; set; }
    public Worker EncoderEngine { get; set; }
    public Worker SpectroEngine { get; set; }

    // Link your audioclip here. Format must be 16Hz mono non-compressed.
    private AudioClip audioClip;
    public AudioClip AudioClip { get { return audioClip; } }

    public int NumSamples { get; set; }
    public float[] Data { get; set; }
    public string[] Tokens { get; set; }

    // Used for special character decoding;
    public int[] WhiteSpaceCharacters { get; set; }

    public Tensor<float> SpectroOutput { get; set; } //CPU Tensor
    public Tensor<float> EncodedAudio { get; set; }  //CPU Tensor

    public TMP_Text SpeechText;

    public bool IsReady { get; set; }


    protected override void Awake()
    {
        IsReady = false;
        WhiteSpaceCharacters = new int[256];
        SetupWhiteSpaceShifts();
        SetTokens();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start(); // <- Init States
    }

    protected override void InitStates()
    {
        base.InitStates();
        AddState(new LoadDecoderState(this));
        AddState(new LoadEncoderState(this));
        AddState(new LoadSpectroState(this));
        AddState(new WhisperReadyState(this));

        AddState(new StartTranscriptionState(this));
        AddState(new RunSpectroState(this));
        AddState(new RunEncoderState(this));
        AddState(new RunDecoderState(this));

        SetState(WhisperStateID.LoadDecoder);
    }


    private void SetTokens()
    {
        var vocab = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabJson.text);
        Tokens = new string[vocab.Count];

        foreach (var item in vocab)
        {
            Tokens[item.Value] = item.Key;
        }
    }


    public void Transcribe( AudioClip clip )
    {
        Debug.Log("-> SentisWisper::Transcribe() ...");

        IsReady = false;
        audioClip = clip;

        SetState( WhisperStateID.StartTranscription );
    }


    protected override void Update()
    {
        base.Update();
    }


    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) WhiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('¡' <= c && c <= '¬') || ('®' <= c && c <= 'ÿ'));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        DecoderEngine?.Dispose();
        EncoderEngine?.Dispose();
        SpectroEngine?.Dispose();
    }
}
