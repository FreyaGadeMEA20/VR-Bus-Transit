using System.Collections;
using System.Collections.Generic;
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
        public GameObject wayPointWait;
    }

    public TrafficLight[] lights;
    
    // Start is called before the first frame update
    void Start()
    {
        StartTrafficLight();
    }

    IEnumerator SwitchToLight(TrafficLight light, LightStates state){
        light.state = state;

        switch(light.state){
            case LightStates.Red:
                TurnOffLights(light.greenLights);
                TurnOnLights(light.yellowLights);
                if (light.wayPointWait != null)
                {
                    light.wayPointWait.GetComponent<WaypointClass>().isWaitingPoint = true;
                }
                yield return new WaitForSeconds(2.5f);
                TurnOffLights(light.yellowLights);
                TurnOnLights(light.redLights);


                yield return new WaitForSeconds(2f);

                if (light.deathBoxes != null){
                    foreach (var deathBox in light.deathBoxes){
                        deathBox.SetActive(true);
                    }
                }

                yield return new WaitForSeconds(light.timeBeforeSwitch);
                StartTrafficLight();
                SwitchTrafficLightState();

                break;
            case LightStates.Green:
                TurnOnLights(light.yellowLights);

                yield return new WaitForSeconds(2f);
                if (light.wayPointWait != null)
                {
                    light.wayPointWait.GetComponent<WaypointClass>().isWaitingPoint = false;
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

    void StartTrafficLight(){
        foreach (var light in lights){
            if(light.direction == trafficLightState){
                StartCoroutine(SwitchToLight(light, LightStates.Green));
            } else {
                StartCoroutine(SwitchToLight(light, LightStates.Red));
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
