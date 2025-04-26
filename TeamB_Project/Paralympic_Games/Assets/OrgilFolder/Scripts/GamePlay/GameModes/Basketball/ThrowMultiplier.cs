using Oculus.Interaction;
using UnityEngine;

public class ThrowMultiplier : MonoBehaviour
{
    [SerializeField] private float throwVelocityMultiplier = 1f;
    [SerializeField] private float throwAngularVelocityMultiplier = 1f;
    [SerializeField] private InteractableUnityEventWrapper eventWrapper;
    void Start()
    {
        eventWrapper.WhenUnselect.AddListener(ApplyVelocityMultiplier);
    }
    private void ApplyVelocityMultiplier()
    {
        GetComponent<Rigidbody>().linearVelocity *= throwVelocityMultiplier;
        GetComponent<Rigidbody>().angularVelocity *= throwAngularVelocityMultiplier;
    }
}
