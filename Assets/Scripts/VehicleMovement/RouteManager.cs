using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for the Route Manager, which tells the bus(VehicleMovement.cs) where to go
// Only needed to determine SPECIFIC waypoints for the bus to follow 
namespace Movement {
    public class RouteManager : MonoBehaviour
    {
        public BusLineSO busLine; // the bus line of the bus.
        public Waypoint currentWaypoint; // the current waypoint the bus is at
        public Path m_Path; // the path the bus follows
        public List<Waypoint> PATH_FOR_INSPECTOR = new List<Waypoint>(); // path for inspector
        public BusStops endDestination; // the end destination of the bus
        public Waypoints m_Waypoints; // List of possible waypoints the bus traverse

        [Serializable]
        public class BusStops{
            public float timeFromNextStop;
            public Waypoint busStop;
        }
        // List of bus stops the bus will traverse to
        public List<BusStops> busStops;

        // Sets the Waypoints to the Waypoints.cs in the scene, which contains all the waypoints
        private void Awake()
        {
            m_Waypoints = FindObjectOfType<Waypoints>();
        }

        // After it has taken all the waypoitns in awake, it sets the route and sends information to the bus stops
        void Start(){
            SetRoute(); // determines the route

            SendInformationToWaypoint(); //tells the bus stops which busses are coming
        }
        
        // Sends information to the bus stops that the bus is coming
        public void SendInformationToWaypoint(){
            foreach (var _busStop in busStops)
            {
                _busStop.busStop.busStop.AddBusLine(busLine);
            }
        }

        // Sets the route
        public void SetRoute(){
            //controller.SetDestination(currentWaypoint);

            // makes sure there are bus stops for the bus to go to
            if(busStops == null){
                return;
            }

            // Sets the end desitination to be the first bus stop
            endDestination = busStops[0];

            // if it is null, then return
            if(endDestination == null){
                return;
            }

            // Determines the route
            DetermineRoute(currentWaypoint, endDestination);
        }

        // Determines the route for the bus to follow
        void DetermineRoute(Waypoint _start, BusStops _end){
            Debug.Log("Determining Route");

            // It uses Djikstra's Algorithm to determine the path for the bus to follow
            // Creates a new path and uses the algorithm to determine the shortest path
            m_Path = new Path();
            m_Path = m_Waypoints.GetShortestPath(_start, _end.busStop);

            // Sets the path for the inspector, for debugging purposes
            PATH_FOR_INSPECTOR = m_Path.waypoints;
            
            // makes the bus stop cycle around, so it can find the next one. Circular List essentially
            busStops.Add(_end);
            busStops.RemoveAt(0);
        }

        // Gets the next waypoint for the bus to go to
        public Waypoint GetNextWaypoint(Waypoint currentWaypoint){

            // if the path is empty, then return null. Shouldn't happen, but just in case
            if(m_Path.waypoints.Count == 0){
                //Assign new busstop as location
                return null;
            }

            // Gets the current index of the waypoint
            int currentIndex = m_Path.waypoints.IndexOf(currentWaypoint);

            // if the current index is less than the total waypoints, then return the next waypoint
            if (currentIndex < m_Path.waypoints.Count - 1)
            {
                return m_Path.waypoints[currentIndex + 1];
            }

            // if the bus stop is a loop, then return the first waypoint. Shouldn't happen, but just in case
            return null;
        } 

        // Used for the gamemanager to find a bus stop for the player to complete the game at
        public Waypoint ChooseRandomBusStop()
        {
            int randomIndex = UnityEngine.Random.Range(1, busStops.Count-2);
            Waypoint randomWaypoint = busStops[randomIndex].busStop;

            return randomWaypoint;
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