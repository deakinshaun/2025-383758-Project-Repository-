using System;
using System.Threading.Tasks;
using Oculus.Interaction;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class BallMount : MonoBehaviour
    {
        [SerializeField] private GameObject preview;
        private Collider ballCollider;
        private InteractableUnityEventWrapper ballEventWrapper;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                ballCollider = other;
                ballEventWrapper = other.GetComponentInChildren<InteractableUnityEventWrapper>();
                ballEventWrapper.WhenUnselect.AddListener(HandleUnselect);
                ballEventWrapper.WhenSelect.AddListener(Reset);
                preview.gameObject.SetActive(true);
            }
        }

        private async void Reset()
        {
            ballCollider.transform.parent = null;
            await Task.Yield();
            ballCollider.attachedRigidbody.isKinematic = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == ballCollider)
            {
                ballEventWrapper.WhenUnselect.RemoveListener(HandleUnselect);
                ballEventWrapper.WhenSelect.RemoveListener(Reset);
                ballCollider = null;
                preview.SetActive(false);
            }
        }
        private async void HandleUnselect()
        {
            await Task.Yield();
            preview.SetActive(false);
            ballCollider.transform.parent = transform;
            ballCollider.transform.position = transform.position;
            ballCollider.attachedRigidbody.isKinematic = false;
        }
    }
}