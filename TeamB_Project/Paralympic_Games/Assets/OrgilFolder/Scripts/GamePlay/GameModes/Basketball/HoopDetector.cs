using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class HoopDetector : MonoBehaviour
    {
        [Tooltip("Event raised when a ball scores.")]

        public int Team;
        public UnityEvent onScore;

        private bool _enteredFromAbove;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                return;
            }
            _enteredFromAbove = other.transform.position.y > transform.position.y;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Ball") || !_enteredFromAbove) return;
            if (other.transform.position.y > transform.position.y) return;
            if (other.TryGetComponent<NetworkObject>(out var netobj) && netobj.TryGetComponent<Ball>(out var ball))
            {
                BasketballGameRule.Instance.RPCRegisterScore(Team, ball.ShotOrigin,
                    netobj.InputAuthority);
            }

            onScore?.Invoke();
            _enteredFromAbove = false;
        }
    }
}