using UnityEngine;
using UnityEngine.InputSystem;

public class ARObjectInteraction : MonoBehaviour
{
    private float initialDistance;
    private Vector3 initialScale;

    private Vector2 previousTouchPosition;
    private bool isRotating = false;

    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float scaleSpeed = 0.01f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 3f;

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touches = Touchscreen.current.touches;
        int touchCount = 0;

        foreach (var touch in touches)
            if (touch.press.isPressed) touchCount++;

        // One touch - rotate
        if (touchCount == 1)
        {
            var touch = touches[0];
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta.ReadValue();
                transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                isRotating = true;
            }
        }
        // Two touches - scale
        else if (touchCount == 2)
        {
            var touch0 = touches[0];
            var touch1 = touches[1];

            if (touch0.isInProgress && touch1.isInProgress)
            {
                Vector2 pos0 = touch0.position.ReadValue();
                Vector2 pos1 = touch1.position.ReadValue();

                float currentDistance = Vector2.Distance(pos0, pos1);

                if (initialDistance == 0)
                {
                    initialDistance = currentDistance;
                    initialScale = transform.localScale;
                }

                float scaleFactor = (currentDistance / initialDistance);
                Vector3 newScale = initialScale * scaleFactor;

                // Clamp to avoid crazy scaling
                newScale = Vector3.Max(newScale, Vector3.one * minScale);
                newScale = Vector3.Min(newScale, Vector3.one * maxScale);

                transform.localScale = newScale;
            }
        }
        else
        {
            // Reset when fingers are lifted
            initialDistance = 0;
            isRotating = false;
        }
    }
}
