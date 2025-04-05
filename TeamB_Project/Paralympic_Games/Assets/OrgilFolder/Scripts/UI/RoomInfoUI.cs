using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrgilFolder.Scripts.UI
{
    public class RoomInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text sessionName;
        [SerializeField] private TMP_Text gameMode;
        [SerializeField] private TMP_Text playerCount;
        [SerializeField] private Button joinButton;
        public event Action OnJoinClicked = delegate {  };
        private void Start()
        {
            joinButton.onClick.AddListener(OnJoinClicked.Invoke);
        }
        public void Initialize(SessionInfo session)
        {
            sessionName.SetText(session.Name);
            gameMode.SetText(session.Properties["gameMode"] ?? "");
            playerCount.SetText($"{session.PlayerCount}/{session.Properties["maxPlayers"].PropertyValue}");
        }
    }
}