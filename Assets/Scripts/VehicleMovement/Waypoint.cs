using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint nextWaypoint;
        public Waypoint previousWaypoint;

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


        bool stop_vehicle{
            get{return STOP_VEHICLE;}
            set{STOP_VEHICLE = value;}
        }
        public bool STOP_VEHICLE;

        [Range(0f,5f)]
        public float width = 1f;

        public List<Waypoint> branches;
        [Range(0f,1f)]
        public float branchRatio = 0.5f;

        public Vector3 GetPosition()
        {
            Vector3 minBound = transform.position + transform.right * width / 2f;
            Vector3 maxBound = transform.position - transform.right * width / 2f;

            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));  
        }
    }
}