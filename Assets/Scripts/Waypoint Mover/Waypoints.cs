using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Route
{
    public List<Transform> waypoints = new List<Transform>();
}

public class Waypoints : MonoBehaviour
{
    public List<Route> routes = new List<Route>();

    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    public bool doLoop = true;

    private void OnDrawGizmos()
    {
        foreach (var route in routes)
        {
            if (route.waypoints.Count == 0) continue;

            for (int i = 0; i < route.waypoints.Count - 1; i++)
            {
                //Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(route.waypoints[i].position, waypointSize);
                Gizmos.DrawLine(route.waypoints[i].position, route.waypoints[i + 1].position);
            }

            if (doLoop) Gizmos.DrawLine(route.waypoints[route.waypoints.Count - 1].position, route.waypoints[0].position);
        }
    }

    public Color RandomColor(){
        return new Color(Random.value, Random.value, Random.value);
    }

    public Transform GetNextWaypoint(Transform currentWaypoint, int routeIndex)
    {
        Route route = routes[routeIndex];

        if (currentWaypoint == null)
        {
            return route.waypoints[0];
        }

        int currentIndex = route.waypoints.IndexOf(currentWaypoint);

        if (currentIndex < route.waypoints.Count - 1)
        {
            return route.waypoints[currentIndex + 1];
        }

        if (doLoop)
        {
            return route.waypoints[0];
        }

        return null;
    }
}