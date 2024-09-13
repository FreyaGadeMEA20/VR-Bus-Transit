using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public class Waypoint : MonoBehaviour
    {
        public enum WaypointType
        {
            Nothing,
            TrafficLight,
            Turn,
            StopSign,
            BusStop,
        }

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

        public WaypointType waypointType;

        bool stop_vehicle{
            get{return STOP_VEHICLE;}
            set{STOP_VEHICLE = value;}
        }
        public bool STOP_VEHICLE;
    }
}