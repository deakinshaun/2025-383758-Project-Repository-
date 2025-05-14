using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroTrackWithPause : MonoBehaviour
{


    public float spinThreshold = 360f;

    private Gyroscope gyro;
    private bool gyroSupported;

    private float lastYaw;
    private float accumulatedYaw;
    private bool isPaused = false;
    public GameObject pauseMenu;
    void Start()
    {
        // cache initial yaw
        lastYaw = transform.rotation.eulerAngles.y;

        Debug.Log("Gyro supported: " + SystemInfo.supportsGyroscope);
        Debug.Log("Gyro enabled: " + Input.gyro.enabled);


        // gyro setup
        gyroSupported = SystemInfo.supportsGyroscope;
        if (gyroSupported)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
    }

    void Update()
    {
        if (!gyroSupported) return;

        // apply gyro rotation
        transform.rotation = Quaternion.Euler(90, 0, 90)
                             * gyro.attitude
                             * Quaternion.Euler(180, 180, 0);

        // accumulate yaw delta
        float currentYaw = transform.rotation.eulerAngles.y;
        float delta = Mathf.DeltaAngle(lastYaw, currentYaw);
        accumulatedYaw += Mathf.Abs(delta);
        lastYaw = currentYaw;

        // on each full spin…
        if (accumulatedYaw >= spinThreshold)
        {
            TogglePause();
            accumulatedYaw -= spinThreshold;
        }
    }

    private void TogglePause()
    {
        pauseMenu.SetActive(true);
        isPaused = !isPaused;



        // freeze/unfreeze game
        Time.timeScale = isPaused ? 0f : 1f;

    }
}
