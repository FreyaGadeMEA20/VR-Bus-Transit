using System.Collections.Generic;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

// Scriptable object to control a bus line
// Includes how long it takes for the bus to traverse each stop
// and the total time it takes to traverse the entire line
// and the information relevant to the bus
[CreateAssetMenu(menuName = "Bus Line")]
public class BusLineSO : ScriptableObject
{
    // Tuple to control all of the relevant id information
    [SerializedTupleLabels("Number", "Starting Zone", "End Station")]
    public SerializedTuple<int, string, string> BusLineID = new (18, "0", "1");
    
    // Time
    public List<float> travelTimes;
    public float totalTravelTime;

    // Function to bake the time it takes to traverse each stop
    public void BakeTime(int index){
        if(index == 5){
            totalTravelTime = Bus_Time.Instance.currentTime;
            return;
        } else if (index > 5){
            return;
        }
        travelTimes[index] = Bus_Time.Instance.currentTime;
    }
}
