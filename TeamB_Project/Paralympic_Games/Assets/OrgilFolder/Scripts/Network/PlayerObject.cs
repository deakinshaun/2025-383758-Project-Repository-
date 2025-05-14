using Fusion;
using UnityEngine;

public class PlayerObject : NetworkBehaviour
{
    public static PlayerObject Local { get; private set; }
    
    [Networked]
    public PlayerRef Ref { get; set; }
    [Networked]
    public byte Index { get; set; }
    
    [Networked, OnChangedRender(nameof(StatChanged))]
    public string Nickname { get; set; }
    [Networked, OnChangedRender(nameof(StatChanged))]
    public Color Color { get; set; }
    
    [Networked]
    public bool IsLoaded { get; set; }
    [Networked, OnChangedRender(nameof(SpectatorChanged))]
    public bool IsSpectator { get; set; }

    [Networked, OnChangedRender(nameof(TeamChanged))]
    public int Team { get; set; } = -1;
    
    public event System.Action OnStatChanged;
    public event System.Action OnSpectatorChanged;
    
    public static event System.Action<NetworkRunner,PlayerRef, int> onPlayerTeamChanged;

    private  void TeamChanged() {
   
        onPlayerTeamChanged?.Invoke(Runner,Ref,Team);
    }
    public void Server_Init(PlayerRef pRef, byte index)
    {
        Debug.Assert(Runner.IsServer);

        Ref = pRef;
        Index = index;
        Color = Random.ColorHSV(0, 1, 0.5f, 1, 0.8f, 1);
    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            PlayerRegistry.Server_Add(Runner, Object.InputAuthority, this);
        }
        if (Object.HasInputAuthority)
        {
            Local = this;
            Rpc_SetNickname(!string.IsNullOrWhiteSpace(UserData.Nickname) ? UserData.Nickname : $"Player_{Random.Range(100, 1000)}");
        }
        PlayerRegistry.PlayerJoined(Object.InputAuthority);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Local == this) Local = null;
    }
    public void ClearGameplayData()
    {
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetNickname(string nick)
    {
        Nickname = nick;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetColor(Color color)
    {
        Color = color;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ToggleSpectate()
    {
        IsSpectator = !IsSpectator;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetTeam(int team)
    {
        Team = team;
    }
    void StatChanged()
    {
        OnStatChanged?.Invoke();
    }

    void SpectatorChanged()
    {
        OnSpectatorChanged?.Invoke();
    }
}