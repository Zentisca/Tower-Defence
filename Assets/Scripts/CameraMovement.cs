using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 10f;
    public float increment = 1f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float verticalAlt = Input.GetAxis("VerticalAlt");

        Vector3 targetPosition = transform.position + new Vector3(horizontal, verticalAlt, vertical) * increment;
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        
        
    }
}
