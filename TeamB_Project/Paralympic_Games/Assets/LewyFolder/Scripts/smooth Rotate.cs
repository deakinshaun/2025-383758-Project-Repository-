using UnityEngine;

public class smoothRotate : MonoBehaviour
{
    public Rigidbody player;
    public float speed;
    public Transform rotator;

    // Update is called once per frame
    void Update()
    {
        var joystickAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        if (joystickAxis.x >= .8f)
        {
            player.transform.RotateAround(rotator.position, rotator.up, speed * .1f);

        }
        if (joystickAxis.x <= -.8f)
        {
            player.transform.RotateAround(rotator.position, rotator.up, -speed * .1f);
        }
    }
}
