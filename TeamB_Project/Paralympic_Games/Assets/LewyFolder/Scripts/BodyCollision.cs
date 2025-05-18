using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BodyCollision : MonoBehaviour
{
    public Transform head;
    public Transform feet;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(head.position.x, head.position.y, head.position.z); 
    }
}
