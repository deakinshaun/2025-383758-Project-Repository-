using System.Collections.Generic;
using Fusion;
using OrgilFolder.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class JoinSessionUI : MonoBehaviour
{
    public UnityEvent OnJoinSession;
    public RectTransform roomListContent;
    [FormerlySerializedAs("roomItemPrefab")] public SessionInfoUI sessionItemPrefab;
    private void Start()
    {
        Matchmaker.Instance.onSessionListUpdated += UpdateRoomList;
    }

    private void OnDestroy()
    {
        Matchmaker.Instance.onSessionListUpdated -= UpdateRoomList;
    }

    private void UpdateRoomList(List<SessionInfo> sessions)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessions)
        {
            var roomInfo = Instantiate(sessionItemPrefab, roomListContent);
            roomInfo.Initialize(session);
            roomInfo.OnJoinClicked += () =>
            {
                Matchmaker.Instance.TryJoinSession(session.Name, () =>
                {
                    OnJoinSession?.Invoke();
                });
            };
        }
    }

}