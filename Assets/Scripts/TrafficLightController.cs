using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum LightStates{
        Red,
        Green,
    }

    public enum Direction{
        NS,
        EW,
    }
    public Direction trafficLightState = Direction.NS;

    [System.Serializable]
    public class TrafficLight{
        public float timeBeforeSwitch;
        public LightStates state;
        public Direction direction;
        public GameObject[] redLights;
        public GameObject[] yellowLights;
        public GameObject[] greenLights;

        public GameObject[] deathBoxes;
        public GameObject[] wayPointsToStop;
    }

    public TrafficLight[] lights;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var light in lights){
            TurnOffLights(light.redLights);
            TurnOffLights(light.yellowLights);
            TurnOffLights(light.greenLights);
        }
        StartTrafficLight();
    }
    void StartTrafficLight() {
        foreach (var light in lights){
            if(light.direction == trafficLightState){
                StartCoroutine(SwitchToLight(light, LightStates.Green));
            } else {
                StartCoroutine(SwitchToLight(light, LightStates.Red));
            }
        }
    }

    IEnumerator SwitchToLight(TrafficLight light, LightStates state){
        light.state = state;

        switch(light.state){
            case LightStates.Red:
                yield return new WaitForSeconds(1f);
                TurnOffLights(light.greenLights);
                TurnOnLights(light.yellowLights);
                if (light.wayPointsToStop != null)
                {
                    foreach (var wayPoint in light.wayPointsToStop)
                    {
                        wayPoint.GetComponent<Waypoint>().TrafficState = Waypoint.TrafficLightState.Red;
                    }
                }
                yield return new WaitForSeconds(4f);
                TurnOffLights(light.yellowLights);
                TurnOnLights(light.redLights);

                yield return new WaitForSeconds(4f);

                if (light.deathBoxes != null){
                    foreach (var deathBox in light.deathBoxes){
                        deathBox.SetActive(true);
                    }
                }

                yield return new WaitForSeconds(light.timeBeforeSwitch);
                SwitchTrafficLightState();
                StartTrafficLight();

                break;
            case LightStates.Green:
                yield return new WaitForSeconds(5.5f);
                TurnOnLights(light.yellowLights);

                yield return new WaitForSeconds(4f);
                if (light.wayPointsToStop != null)
                {
                    foreach (var wayPoint in light.wayPointsToStop)
                    {
                        wayPoint.GetComponent<Waypoint>().TrafficState = Waypoint.TrafficLightState.Green;
                    }
                }
                if (light.deathBoxes != null){
                    foreach (var deathBox in light.deathBoxes){
                        deathBox.SetActive(false);
                    }
                }

                TurnOffLights(light.redLights);
                TurnOffLights(light.yellowLights);
                TurnOnLights(light.greenLights);

                //yield return new WaitForSeconds(light.timeBeforeSwitch);
                break;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.CompareTag("Bus") || other.gameObject.CompareTag("Car")){
            foreach(var light in lights){
                foreach (var wayPoint in light.wayPointsToStop) {
                    wayPoint.GetComponent<Waypoint>().TrafficLightClear = false;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Bus") || other.gameObject.CompareTag("Car")){
            foreach(var light in lights){
                foreach (var wayPoint in light.wayPointsToStop) {
                    wayPoint.GetComponent<Waypoint>().TrafficLightClear = true;
                }
            }
        }
    }

    void SwitchTrafficLightState(){
        switch(trafficLightState){
            case Direction.NS:
                trafficLightState = Direction.EW;
                break;
            case Direction.EW:
                trafficLightState = Direction.NS;
                break;
        }
    }

    void TurnOffLights(GameObject[] lightS){
        foreach (var light in lightS){
            light.SetActive(false);
        }
    }

    void TurnOnLights(GameObject[] lightS){
        foreach (var light in lightS){
            light.SetActive(true);
        }
    }
}
