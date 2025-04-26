using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class Ball : NetworkBehaviour
    {
        [Networked] public int PossessingTeam { get; private set; }
        [Networked] public Vector3 ShotOrigin { get; private set; }
        
        //TODO: Handle Team assignemnt

        public void ResetNeutral()
        {
            if (Runner.IsServer)
            {
                PossessingTeam = -1;
            }
        }
        public void InitializePossession(int team)
        {
            if (Runner.IsServer)
            {
                PossessingTeam = team;
            }
        }
        /// <summary>
        /// Call this right before you actually launch/throw the ball.
        /// </summary>
        public void StampShotOrigin(Vector3 origin) {
            if (Runner.IsServer) {
                ShotOrigin = origin;
            }
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCAssignPossession(int team, RpcInfo info = default)
        {
            PossessingTeam = team;
        }
    }
    
}