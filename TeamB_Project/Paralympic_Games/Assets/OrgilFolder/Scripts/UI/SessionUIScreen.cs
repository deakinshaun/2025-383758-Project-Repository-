using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using OrgilFolder.Scripts;
using OrgilFolder.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;


public class SessionUIScreen : MonoBehaviour
{
    public Transform team1PlayerItemHolder;
    public Transform team2PlayerItemHolder;
    public Button team1JoinButton;
    public Button team2JoinButton;
    public Transform spectatorItemHolder;
    public Button startGameButton;

    [SerializeField] private PlayerSessionItemUI playerSessionItemUIPrefab;
    readonly Dictionary<PlayerRef, PlayerSessionItemUI> playerItems = new Dictionary<PlayerRef, PlayerSessionItemUI>();

    public void AddSubscriptions()
    {
        PlayerRegistry.onPlayerJoined += PlayerJoined;
        PlayerRegistry.onPlayerLeft += PlayerLeft;
    }

    private void OnEnable()
    {
        PlayerRegistry.onPlayerJoined -= PlayerJoined;
        PlayerRegistry.onPlayerLeft -= PlayerLeft;
        PlayerRegistry.onPlayerJoined += PlayerJoined;
        PlayerRegistry.onPlayerLeft += PlayerLeft;
        PlayerObject.onPlayerTeamChanged += SpawnPlayerItem;

        Initialization();

        team1JoinButton.onClick.AddListener(() =>
        {
            PlayerObject.Local.Rpc_SetTeam(1);
            team1JoinButton.gameObject.SetActive(false);
            team2JoinButton.gameObject.SetActive(true);
        });
        team2JoinButton.onClick.AddListener(() =>
        {
            PlayerObject.Local.Rpc_SetTeam(2);
            team2JoinButton.gameObject.SetActive(false);
            team1JoinButton.gameObject.SetActive(true);
        });
    }

    private void Initialization()
    {
        
        
        foreach (var player in PlayerRegistry.Players)
        {
            SpawnPlayerItem(PlayerRegistry.Instance.Runner, player.Ref, player.Team);
        }
    }

    private void SpawnPlayerItem(NetworkRunner runner, PlayerRef player, int team)
    {
        var parentHolder = team switch
        {
            1 => team1PlayerItemHolder,
            2 => team2PlayerItemHolder,
            _ => spectatorItemHolder
        };

        if (PlayerRegistry.GetPlayer(player).IsSpectator)
        {
            parentHolder = spectatorItemHolder;
        }

        if (playerItems.TryGetValue(player, out var pobject))
        {
            runner.Despawn(pobject.GetComponent<NetworkObject>());
        }

        if (runner.CanSpawn)
        {
            PlayerSessionItemUI item = runner.Spawn(playerSessionItemUIPrefab, inputAuthority: player);
            playerItems[player] = item;
            item.transform.SetParent(parentHolder, false);

            PlayerRegistry.GetPlayer(player).OnSpectatorChanged += (() =>
            {
                if (PlayerRegistry.GetPlayer(player).IsSpectator)
                {
                    item.transform.SetParent(spectatorItemHolder, false);
                }
            });
        }
    }

    private void OnDisable()
    {
        PlayerRegistry.onPlayerJoined -= PlayerJoined;
        PlayerRegistry.onPlayerLeft -= PlayerLeft;
        PlayerObject.onPlayerTeamChanged -= SpawnPlayerItem;
    }

    private void PlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerItems.TryGetValue(player, out PlayerSessionItemUI item))
        {
            if (item)
            {
                Debug.Log($"Removing {nameof(PlayerSessionItemUI)} for {player}");
                runner.Despawn(item.Object);
            }
            else
            {
                Debug.Log($"{nameof(PlayerSessionItemUI)} for {player} was null.");
            }

            playerItems.Remove(player);
        }
        else
        {
            Debug.LogWarning($"{player} not found");
        }
    }

    private void PlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!playerItems.ContainsKey(player))
        {
            SpawnPlayerItem(runner, player, PlayerRegistry.GetPlayer(player).Team);
        }
        else
        {
            Debug.LogWarning($"{player} already in dictionary");
        }
    }

    public void StartGame()
    {
        if (PlayerRegistry.CountPlayers > 0)
        {
            GameManager.State.Server_SetState(GameState.EGameState.Loading);
        }
    }

    public void ToggleSpectate()
    {
        PlayerObject.Local.Rpc_ToggleSpectate();
    }

    public void Leave()
    {
        StartCoroutine(LeaveRoutine());
    }

    IEnumerator LeaveRoutine()
    {
        Task task = Matchmaker.Instance.Runner.Shutdown();
        while (!task.IsCompleted)
        {
            yield return null;
        }

        UIManager.Instance.Pop();
    }
}