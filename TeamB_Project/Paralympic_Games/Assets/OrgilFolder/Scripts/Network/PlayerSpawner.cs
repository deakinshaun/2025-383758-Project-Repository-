using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    public NetworkObject playerObject;

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.CanSpawn)
        {
            var pObject = Runner.Spawn(playerObject, inputAuthority: player);
            Runner.SetPlayerObject(player,pObject);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        bool canDespawn = (Runner.Topology == Topologies.ClientServer && Runner.IsServer) ||
                          (Runner.Topology == Topologies.Shared && Runner.IsSharedModeMasterClient);
        if (canDespawn)
        {
            PlayerObject leavingPlayer = PlayerRegistry.GetPlayer(player);
            Runner.Despawn(leavingPlayer.Object);
        }
    }
}