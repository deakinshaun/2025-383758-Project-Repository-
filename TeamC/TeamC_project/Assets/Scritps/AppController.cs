using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ChangeScene()
    {
        SceneManager.LoadScene("VoiceChat");
    }
}
