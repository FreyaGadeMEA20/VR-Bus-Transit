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

        public WaypointType waypointType;
    }
}