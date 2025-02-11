using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Movement{
    public class Waypoint : MonoBehaviour
    {
        [Separator("Connection Waypoints")]
        public Waypoint nextWaypoint;
        public Waypoint previousWaypoint;
        
        public List<Waypoint> branches;
        [Range(0f,1f)]
        public float branchRatio = 0.5f;

        // Just the above variables connected for pathfinding purposes
        [HideInInspector] public List<Waypoint> connections = new List<Waypoint>();

        // What type of waypoint it is
        public enum WaypointType
        {
            Nothing,
            TrafficLight,
            BusStop,
        }
        [Separator("Waypoint Type")]
        public WaypointType waypointType;

        [Range(0f,5f)]
        public float width = 1f;

        public enum TrafficLightState
        {
            Red,
            Green,
        }
        TrafficLightState trafficState{
            get{return TrafficState;}
            set{TrafficState = value;}
        }

        [Separator("Waypoint Type Variables (empty = none)")]
        [ConditionalField("waypointType", false, WaypointType.TrafficLight)] public TrafficLightState TrafficState;
        [ConditionalField("waypointType", false, WaypointType.TrafficLight)] public bool TrafficLightClear = true;
        [ConditionalField("waypointType", false, WaypointType.BusStop)] public BusStop busStop;
        

        public Vector3 GetPosition()
        {
            Vector3 minBound = transform.position + transform.right * width / 2f;
            Vector3 maxBound = transform.position - transform.right * width / 2f;

            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));  
        }

        public bool EvaluateTrafficLight()
        {
            return TrafficLightClear;
        }

        void Awake(){
            if(nextWaypoint != null){
                connections.Add(nextWaypoint);
            }
            
            for(int i = 0; i < branches.Count; i++){
                connections.Add(branches[i]);
            }

            // We don't want it to drive backwards, so it does not add the previous waypoint as a connection
            /* if(previousWaypoint != null){
                connections.Add(previousWaypoint);
            } */
        }
    }
}