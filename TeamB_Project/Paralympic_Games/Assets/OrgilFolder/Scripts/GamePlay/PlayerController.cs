using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay
{
    public class PlayerController : NetworkBehaviour
    {
        [Networked] PlayerInput CurrentInput { get; set; }
        private PlayerInput PreviousInput = default;
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out PlayerInput input))
            {
                CurrentInput = input;
                if (Runner.IsForward)
                {
                    transform.position += input.velocity * Runner.DeltaTime;
                    transform.rotation = input.rotation;
                }
            }
        }
    }
}