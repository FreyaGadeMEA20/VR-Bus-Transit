using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    [System.Serializable]
    public class Route{
        public List<Waypoint> waypoints = new List<Waypoint>();
        
        public bool doLoop = true;
    }
    
    public class RouteManager : MonoBehaviour
    {
        public List<Route> routes = new List<Route>();
    
        [Range(0f, 2f)]
        [SerializeField] private float waypointSize = 1f;

        
        private void OnDrawGizmos()
        {
            foreach (var route in routes)
            {
                if (route.waypoints.Count == 0) continue;

                for (int i = 0; i < route.waypoints.Count -1 ; i++)
                {
                    //Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(route.waypoints[i].transform.position, waypointSize);
                    Gizmos.DrawLine(route.waypoints[i].transform.position, route.waypoints[i + 1].transform.position);
                }

                Gizmos.DrawWireSphere(route.waypoints[route.waypoints.Count-1].transform.position, waypointSize);

                if (route.doLoop) Gizmos.DrawLine(route.waypoints[route.waypoints.Count - 1].transform.position, route.waypoints[0].transform.position);
            }
        }

        public Waypoint GetNextWaypoint(Waypoint currentWaypoint, int routeIndex)
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

            if (route.doLoop)
            {
                return route.waypoints[0];
            }

            return null;
        }
    }
}