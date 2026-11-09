using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Gyroscope : MonoBehaviour
{
    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        Vector3 gyroRate = Input.gyro.rotationRateUnbiased * Mathf.Rad2Deg;

        // 1) gyro space (left-handed) -> Unity space (right-handed)
        Vector3 unityRate = new Vector3(-gyroRate.x, -gyroRate.y, gyroRate.z);

        // 2) rotate the vector into the camera's frame (camera looks along y:180)
        Quaternion cam = Quaternion.Euler(0, 180, 0);
        Vector3 rot = cam * unityRate * Time.deltaTime;

        // deltas are now in world axes, so rotate in world space
        transform.Rotate(rot, Space.World);
    }
}