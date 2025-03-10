using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Movement;
using SerializedTuples;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusScreenController : MonoBehaviour
{
    [Serializable]
    public class Screen{
        public TextMeshProUGUI[] nextStation = new TextMeshProUGUI[2];
        public TextMeshProUGUI nextNextStation;
        public TextMeshProUGUI nextNextNextStation;
        public TextMeshProUGUI line;
        public Image image;
    }

    // Screens for the bus
    [SerializeField] Screen[] screens = new Screen[2];
    [SerializeField] Sprite stopScreen;
    [SerializeField] Sprite dotScreen;

    [SerializedTupleLabels("Next Station", "NextNextStation", "NextNextNextStation", "Line")]
    public SerializedTuple<string, string, string, string> BusLineID = new ("","", "", "");

    [SerializeField] RouteManager routeManager;
    public List<Waypoint> stopList;

    // Start is called before the first frame update
    void Start()
    {
        routeManager = GetComponentInParent<RouteManager>();
        ApplyNextTexture();
    }

    // Fucntion does as name suggests
    public void ApplyNextTexture(){
        foreach(Screen screen in screens){
            screen.nextStation[0].text = BusLineID.v1;
            screen.nextStation[1].text = BusLineID.v1;
            screen.nextNextStation.text = BusLineID.v2;
            screen.nextNextNextStation.text = BusLineID.v3;
            screen.line.text = BusLineID.v4;
            screen.image.sprite = dotScreen;
        }

        // Apply the new image to the material
        //newMat.mainTexture = stopList[currentIndex].nextStopDot;
    }

    // Takes the relevant information from the route manager and applies it to the screens
    public void GiveInformation(){
        BusLineID.v1 = routeManager.busStops[0].busStop.busStop.BusStopName;
        BusLineID.v2 = routeManager.busStops[1].busStop.busStop.BusStopName;
        BusLineID.v3 = routeManager.busStops[2].busStop.busStop.BusStopName;
        BusLineID.v4 = routeManager.busLine.BusLineID.v1.ToString();
    }

    // Function does as name suggests
    public void ApplyStopTexture(){
        foreach(Screen screen in screens){
            screen.image.sprite = stopScreen;
        }
    }
}
