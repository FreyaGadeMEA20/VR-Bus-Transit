using System.Collections.Generic;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Bus Line")]
public class BusLineSO : ScriptableObject
{
    [SerializedTupleLabels("Number", "Starting Zone", "End Station")]
    public SerializedTuple<int, string, string> BusLineID = new (18, "0", "1");
    public List<float> travelTimes;
    public float totalTravelTime;

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
