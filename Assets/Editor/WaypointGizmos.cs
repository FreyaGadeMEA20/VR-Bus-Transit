using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Movement;

// Allows it to be used in the unity editor, so it is visualized
[InitializeOnLoad()]
public class WaypointEditor
{
    // Draws the gizmo in the scene view, no matter if it's selected, not selected or pickable
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        Color color = Color.white;
        switch(waypoint.waypointType)
        {
            case Waypoint.WaypointType.Nothing:
                color = Color.yellow;
                break;
            case Waypoint.WaypointType.TrafficLight:
                if(waypoint.TrafficState == Waypoint.TrafficLightState.Red)
                {
                    color = Color.red;
                }
                else
                {
                    color = Color.green;
                }
                break;
            case Waypoint.WaypointType.BusStop:
                color = Color.black;
                break;
        }
        // If it is selected, it will be coloured a strong yellow
        if((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = color;
        }
        else    // If it is not selected, it will be coloured a soft yellow
        {
            Gizmos.color = color * 0.5f;
        }

        // Draws a sphere in the position of the waypoint
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);

        // Draws a line from the waypoint to the right and left, to show the width of the waypoint
        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f),
                        waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2f));
        
        // If the waypoint has a previous waypoint, it will draw a red line to it
        if(waypoint.previousWaypoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position - offset, waypoint.previousWaypoint.transform.position - offsetTo);
        }

        // If the waypoint has a next waypoint, it will draw a green line to it
        if(waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * waypoint.nextWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
        }

        if(waypoint.branches!=null)
        {
            foreach (var branch in waypoint.branches)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
            }
        }
    }
}
