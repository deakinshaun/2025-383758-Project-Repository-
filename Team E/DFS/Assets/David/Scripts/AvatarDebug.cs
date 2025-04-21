using UnityEngine;

public class AvatarDebug : MonoBehaviour
{
    float timer = 0;

void Start()
{
    Debug.Log("Start triggered on Avatar");
}
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            Debug.Log("[AvatarDebug] Still alive at: " + Time.time);
            timer = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[AvatarDebug] Trigger Enter: " + other.name);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[AvatarDebug] Collision Enter: " + collision.gameObject.name);
    }
}
