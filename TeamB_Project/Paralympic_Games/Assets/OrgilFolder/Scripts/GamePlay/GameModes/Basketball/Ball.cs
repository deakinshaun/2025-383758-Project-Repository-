using Fusion;
using Oculus.Interaction;
using OrgilFolder.Scripts.Utility;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class Ball : NetworkBehaviour
    {
        [Networked] public int PossessingTeam { get; private set; }
        [Networked] public Vector3 ShotOrigin { get; private set; }

        [SerializeField] private AudioSource audioSource;
        //TODO: Handle Team assignemnt

        public override void Spawned()
        {
            base.Spawned();
            var eventWrapper = GetComponentInChildren<InteractableUnityEventWrapper>();

            eventWrapper.WhenSelect.AddListener(HandlePossesion);
            eventWrapper.WhenUnselect.AddListener(() => { StampShotOrigin(transform.position); });
        }

        private void HandlePossesion()
        {
            var playerController = GetComponentInParent<PlayerController>();
            if (playerController)
            {
                int team = playerController.playerObject.Team;
                RPCAssignPossession(team);
            }
        }

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

        private void OnCollisionEnter(Collision other)
        {
            audioSource.PlayOneShot(audioSource.clip, new Vector2(0.9f, 1.1f), 1.0f);
        }


        /// <summary>
        /// Call this right before you actually launch/throw the ball.
        /// </summary>
        public void StampShotOrigin(Vector3 origin)
        {
            if (Runner.IsServer)
            {
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