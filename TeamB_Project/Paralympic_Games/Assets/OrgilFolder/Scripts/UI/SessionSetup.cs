using OrgilFolder.Scripts;
using OrgilFolder.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;


public class SessionSetup : MonoBehaviour
{
    public static string sessionName;
    public static bool isPrivate;
    public UnityEvent onTryCreateSession;
    public UnityEvent onSessionCreated;

    public CarouselView carouselView;
    public GameInfoUI gameInfoPrefab;

    [SerializeField] private GameProperty[] gameModes;

    public string SessionName
    {
        get => sessionName;
        set => sessionName = value;
    }

    public bool IsPrivate
    {
        get => isPrivate;
        set
        {
            isPrivate = value;
            if (GameManager.Instance?.Runner != null) ApplyConfig();
        }
    }

    private void Start()
    {
        foreach (var gameMode in gameModes)
        {
            var gameModeInfo = Instantiate(gameInfoPrefab, carouselView.ContentRect);
            gameModeInfo.Initialize(gameMode);
        }
    }

    public void TryCreateSession()
    {
        if (Matchmaker.Instance.Runner == null)
        {
            onTryCreateSession?.Invoke();
            Matchmaker.Instance.SetRoomCode(sessionName);
            Matchmaker.Instance.SetPrivate(IsPrivate);
            Matchmaker.Instance.TryHostSession(gameModes[carouselView.currentIndex], () => onSessionCreated?.Invoke());
        }
        else
        {
            ApplyConfig();
        }
    }

    public void ApplyConfig()
    {
        if (GameManager.Instance.Runner.SessionInfo.IsVisible != !IsPrivate)
            GameManager.Instance.Runner.SessionInfo.IsVisible = !IsPrivate;
    }
}