using System.Collections;
using System.Collections.Generic;
using Movement;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Bus Line")]
public class BusLineSO : ScriptableObject
{
    [SerializedTupleLabels("Number", "End Station", "ID")]
    public SerializedTuple<int, string, int> BusLineID = new (18, "0", 0);
}
