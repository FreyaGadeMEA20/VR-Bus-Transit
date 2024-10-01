using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement {
    public class RouteManager : MonoBehaviour
    {
        VehicleMovement controller;
        public Waypoint currentWaypoint;
        public List<Waypoint> busStops = new List<Waypoint>();
        public Path m_Path = new Path();
        public List<Waypoint> PATH_FOR_INSPECTOR = new List<Waypoint>();

        public Waypoint endDestination;

        public Waypoints m_Waypoints;

        private void Awake()
        {
            controller = GetComponent<VehicleMovement>();
            m_Waypoints = FindObjectOfType<Waypoints>();
        }

        void Start(){
            SetRoute();
        }

        public void SetRoute(){
            controller.SetDestination(currentWaypoint);

            if(busStops == null){
                return;
            }

            endDestination = busStops[0];

            if(endDestination == null){
                return;
            }
            Debug.Log(currentWaypoint +" to "+ endDestination);
            DetermineRoute(currentWaypoint, endDestination);
        }

        void DetermineRoute(Waypoint _start, Waypoint _end){
            // Djikstra's Algorithm to determine the path for the bus to follow
            m_Path = m_Waypoints.GetShortestPath(_start, _end);
            Debug.Log(m_Path.ToString());
            PATH_FOR_INSPECTOR = m_Path.waypoints;
            busStops.Add(_end);
            busStops.Remove(_end);
        }

        /* void Update(){
            if(controller.ReachedDestination){
                currentWaypoint = currentWaypoint.nextWaypoint;
                controller.SetDestination(currentWaypoint.GetPosition());
            }
        } */
        /* private void OnDrawGizmos()
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

                //if (route.doLoop) Gizmos.DrawLine(route.waypoints[route.waypoints.Count - 1].transform.position, route.waypoints[0].transform.position);
            }
        } */

        /* public Waypoint GetNextWaypoint(Waypoint currentWaypoint, int routeIndex)
        {
            Route route = routes[routeIndex];

            if (currentWaypoint == null)
            {
                //return route.waypoints[0];
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
        } */
    }
}