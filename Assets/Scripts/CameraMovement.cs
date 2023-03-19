using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 10f; // the speed at which the camera moves
    public float increment = 1f; // the increment by which the camera moves

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // get the horizontal input axis
        float vertical = Input.GetAxis("Vertical"); // get the vertical input axis
        float verticalAlt = Input.GetAxis("VerticalAlt"); // get the alternative vertical input axis

        Vector3 targetPosition = transform.position + new Vector3(horizontal, verticalAlt, vertical) * increment; // calculate the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime); // interpolate between the current position and the target position
        
        
    }
}
