using System;
using System.Collections.Generic;
using Fusion;
using OrgilFolder.Scripts.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class LobbyUI : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public Button hostButton;
    public Button refreshButton;
    public RectTransform roomListContent;
    public RectTransform gameModeCarouselContent;
    public CarouselView carouselView;
    public RoomInfoUI roomItemPrefab;
    public GameInfoUI gameInfoPrefab;

    [SerializeField] private GameProperty[] gameModes;

    private void Start()
    {
        NetworkManager.Instance.onSessionListUpdated += UpdateRoomList;

        hostButton.onClick.AddListener(HostGame);

        foreach (Transform child in gameModeCarouselContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var gameMode in gameModes)
        {
            var gameModeInfo = Instantiate(gameInfoPrefab, gameModeCarouselContent);

            gameModeInfo.Initialize(gameMode);
        }
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.onSessionListUpdated -= UpdateRoomList;
    }

    private void UpdateRoomList(List<SessionInfo> sessions)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessions)
        {
            var roomInfo = Instantiate(roomItemPrefab, roomListContent);

            roomInfo.Initialize(session);

            roomInfo.OnJoinClicked += () =>
            {
                NetworkManager.Instance.JoinLobby(session);
            };
        }
    }

    void HostGame()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName)) roomName = "Room_" + Random.Range(1000, 9999);
        NetworkManager.Instance.StartHost(roomName, GameMode.Host,gameModes[carouselView.currentIndex]);
    }

    void RefreshRooms()
    {
    }
}