using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint nextWaypoint;
        public Waypoint previousWaypoint;
        
        public List<Waypoint> branches;
        [Range(0f,1f)]
        public float branchRatio = 0.5f;

        public List<Waypoint> connections = new List<Waypoint>();

        void Awake(){
            if(nextWaypoint != null){
                connections.Add(nextWaypoint);
            }
            //connections.Insert(0, nextWaypoint);
            for(int i = 0; i < branches.Count; i++){
                connections.Add(branches[i]);
            }
            if(previousWaypoint != null){
                connections.Add(previousWaypoint);
            }
            //connections.Add(previousWaypoint);
        }

        public enum WaypointType
        {
            Nothing,
            TrafficLight,
            BusStop,
        }
        public WaypointType waypointType;

        public enum TrafficLightState
        {
            Red,
            Green,
        }
        TrafficLightState trafficState{
            get{return TrafficState;}
            set{TrafficState = value;}
        }

        public TrafficLightState TrafficState;

        [Range(0f,5f)]
        public float width = 1f;


        public Vector3 GetPosition()
        {
            Vector3 minBound = transform.position + transform.right * width / 2f;
            Vector3 maxBound = transform.position - transform.right * width / 2f;

            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));  
        }
    }
}