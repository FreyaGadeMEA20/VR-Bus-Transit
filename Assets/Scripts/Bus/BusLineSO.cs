using System.Collections;
using System.Collections.Generic;
using Movement;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Bus Line")]
public class BusLineSO : ScriptableObject
{
    [SerializedTupleLabels("Number", "Starting Zone", "End Station")]
    public SerializedTuple<int, string, string> BusLineID = new (18, "0", "1");
    public List<Waypoint> waypoints;

    public Waypoint ChooseRandomWaypoint()
    {
        int randomIndex = Random.Range(1, waypoints.Count);
        Waypoint randomWaypoint = waypoints[randomIndex];

        return randomWaypoint;
    }
}
