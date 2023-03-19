using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints; // array of waypoints for the enemies to move through

    public Transform[] GetWaypoints()
    {
        return waypoints;
    }
}