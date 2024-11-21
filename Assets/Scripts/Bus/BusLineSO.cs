using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Bus Line")]
public class BusLineSO : ScriptableObject
{
    [SerializedTupleLabels("Number", "Starting Zone", "End Station")]
    public SerializedTuple<int, string, string> BusLineID = new (18, "0", "1");
}
