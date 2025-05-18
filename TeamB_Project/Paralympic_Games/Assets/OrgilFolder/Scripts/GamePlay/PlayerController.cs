using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay
{
    public class PlayerController : NetworkBehaviour
    {
        private PlayerInput PreviousInput = default;

        private NetworkCharacterController cc;

        [Networked] private Quaternion NetworkRotation { get; set; }

        [SerializeField] private GameObject camera;
        [HideInInspector] public PlayerObject playerObject;

        public override void Spawned()
        {
            cc = GetComponent<NetworkCharacterController>();
            base.Spawned();
            camera.SetActive(HasInputAuthority);
        }

        public override void FixedUpdateNetwork()
        {
            // if (!HasInputAuthority) return;
            if (!Runner.IsForward) return;
            if (!GetInput(out PlayerInput input)) return;

            cc.Velocity = input.velocity;
            cc.Move(input.velocity);

            NetworkRotation = input.rotation;
            cc.transform.rotation = NetworkRotation;
        }
    }
}