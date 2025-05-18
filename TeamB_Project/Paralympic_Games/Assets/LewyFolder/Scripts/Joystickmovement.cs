using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OVR;
public class Joystickmovement : MonoBehaviour
{

    public Rigidbody player;
    public float speed;


    // Update is called once per frame
    void Update()
    {
        var joystickAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.LTouch);
        float fixedY = player.position.y;

        player.position +=(transform.right * joystickAxis.x + transform.forward * joystickAxis.y) * speed * Time.deltaTime;
        player.position = new Vector3(player.position.x, fixedY, player.position.z);
    }
}
