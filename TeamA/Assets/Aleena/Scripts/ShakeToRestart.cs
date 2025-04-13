using UnityEngine;

public class ShakeToRestart : MonoBehaviour
{
    public float shakeThreshold = 2.5f;
    public float cooldown = 1.5f;

    private float lastShakeTime = 0f;

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        float accelerationMagnitude = acceleration.sqrMagnitude;

        if (accelerationMagnitude >= shakeThreshold * shakeThreshold)
        {
            if (Time.time - lastShakeTime > cooldown)
            {
                lastShakeTime = Time.time;
                Debug.Log("Shake detected!");
                FindFirstObjectByType<InstructionManager>()?.ResetInstructions();
                Handheld.Vibrate(); // Optional: for feedback
            }
        }
    }
}
