using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Movement;
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
    [SerializeField] Screen[] sceens = new Screen[2];
    [SerializeField] Sprite stopScreen;
    [SerializeField] Sprite dotScreen;

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
        foreach(Screen screen in sceens){
            screen.nextStation[0].text = routeManager.busStops[0].busStop.BusStopName;
            screen.nextStation[1].text = routeManager.busStops[0].busStop.BusStopName;
            screen.nextNextStation.text = routeManager.busStops[1].busStop.BusStopName;
            screen.nextNextNextStation.text = routeManager.busStops[2].busStop.BusStopName;
            screen.line.text = routeManager.busLine.BusLineID.v1.ToString();
            screen.image.sprite = dotScreen;
        }

        // Apply the new image to the material
        //newMat.mainTexture = stopList[currentIndex].nextStopDot;
    }

    // Function does as name suggests
    public void ApplyStopTexture(){
        foreach(Screen screen in sceens){
            screen.image.sprite = stopScreen;
        }
    }
}
