using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechConverter : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;
    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
    }

    private void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        recording = false;
        SendRecording();
    }

    // Update is called once per frame
    void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }


    }

    private void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response =>
        {
            text.color = Color.white;
            text.text = response;
        }, error =>
        {
            text.color = Color.red;
            text.text = error;
        });
    }
}
