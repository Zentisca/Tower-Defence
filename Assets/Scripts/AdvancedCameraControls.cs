using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCameraControls : MonoBehaviour
{
    public float scrollMoveSpeed = 5f;
    public float xzMoveSpeed = 10f;
    public float rotationSpeed = 50f;
    public Vector3 startingPosition = new Vector3(0f, 10f, -10f);
    public Quaternion startingRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
    public float tiltAngle = 30f;
    public float tiltDuration = 1f;
    public float minTiltAngle = 0f;
    public float maxTiltAngle = 90f;

    private float tiltTimer = 0f;
    private bool isTilting = false;

    void Update()
    {
        // Camera move down towards the ground
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            transform.Translate(Vector3.down * scrollMoveSpeed * Time.deltaTime);
        }

        // Camera move up towards the sky
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            transform.Translate(Vector3.up * scrollMoveSpeed * Time.deltaTime);
        }

        // Camera rotate clockwise
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        // Camera rotate anti-clockwise
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }

        // Camera move up screen
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * xzMoveSpeed * Time.deltaTime);
        }

        // Camera move down screen
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * xzMoveSpeed * Time.deltaTime);
        }

        // Camera move left screen
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * xzMoveSpeed * Time.deltaTime);
        }

        // Camera move right screen
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * xzMoveSpeed * Time.deltaTime);
        }

        // Reset camera to starting position
        if (Input.GetKeyDown(KeyCode.Home))
        {
            transform.position = startingPosition;
            transform.rotation = startingRotation;
        }
        
        // Start tilting camera
        if (Input.GetMouseButtonDown(1))
        {
            tiltTimer = 0f;
            isTilting = true;
        }

        // Tilt camera
        if (Input.GetMouseButton(1) && isTilting)
        {
            tiltTimer += Time.deltaTime;

            if (tiltTimer >= tiltDuration)
            {
                isTilting = false;
            }
            else
            {
                float mouseY = Input.GetAxis("Mouse Y");
                float newRotationX = Mathf.Clamp(transform.rotation.eulerAngles.x - mouseY * 
                    tiltAngle * Time.deltaTime, minTiltAngle, maxTiltAngle);
                transform.rotation = Quaternion.Euler(newRotationX, transform.rotation.eulerAngles.y, 
                    transform.rotation.eulerAngles.z);
            }
        }


        // Stop tilting camera
        if (Input.GetMouseButtonUp(1))
        {
            isTilting = false;
        }
    }
}

/*
    Notes:
    maxTilt Angle needs to be changed. This should be max depression & Incline, not angler maxTilt.
    Smooth out camera movements on all the above, it's a little too "zippy."
    
    Find a way to Make the Camera not be able to pass through GameObjects (Maybe look into an invisible box around the 
    pip of the camera to use as a collider?
    
*/