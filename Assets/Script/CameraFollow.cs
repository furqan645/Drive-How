using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Windows.WebCam;

public class CameraFollow : MonoBehaviour
{
    private Transform cameraPoint;
    private Transform car;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] float delayRotation = 0.1f;
    [SerializeField] float delayInFollow = 0.2f;

    void Start()
        {
            car = GameObject.FindGameObjectWithTag("Car").GetComponent<Transform>();
            cameraPoint = car.Find("CameraPoint").GetComponent<Transform>();
        }

    void FixedUpdate()
    {
        FollowCar();
    }

    void FollowCar()
    {
        // Smoothly follow the car's position
        transform.position = Vector3.SmoothDamp(transform.position, cameraPoint.position, ref velocity, delayInFollow);

        // Smoothly align the camera's rotation
        Quaternion targetRotation = cameraPoint.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delayRotation);
    }

}
