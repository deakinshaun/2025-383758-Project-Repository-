using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay
{
    public class PlayerController : NetworkBehaviour
    {
        [Networked] PlayerInput CurrentInput { get; set; }
        private PlayerInput PreviousInput = default;

        [SerializeField] private GameObject camera;
        [HideInInspector] public PlayerObject playerObject;

        public override void Spawned()
        {
            base.Spawned();
            camera.SetActive(HasInputAuthority);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority) return;
            if (!GetInput(out PlayerInput input)) return;
            if (!Runner.IsForward) return;

            CurrentInput = input;
            transform.position += input.velocity * Runner.DeltaTime;
            transform.rotation = input.rotation;
        }
    }
}