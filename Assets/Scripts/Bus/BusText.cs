using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BusText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] busNumber;
    [SerializeField] TextMeshProUGUI[] endStation;

    [SerializeField] string busNumberText;
    [SerializeField] string endStationText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (TextMeshProUGUI text in busNumber)
        {
            text.text = busNumberText;
        }

        foreach (TextMeshProUGUI text in endStation)
        {
            text.text = endStationText;
        }
    }
}
