using System;
using System.Collections;
using System.Collections.Generic;
using Movement;
using MyBox;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;

public class BusStop : MonoBehaviour {
    public GameObject sign;

    string stopName{ get{return BusStopName;} set{BusStopName = value;}}

    public string BusStopName;

    GameObject player;
    public GameObject deathZone;

    public List<BusLineSO> associatedLines;
    
    bool playerInProx { get; set; }

    public int busStopIndex;

    [Separator("Timetable")]
    
    public List<TTElement> timeTable;
    
    [Serializable]
    public class TTElement{
        public GameObject parent;
        public TextMeshProUGUI number;
        public TextMeshProUGUI timeTableE;
        public TextMeshProUGUI time;
    }
    // Start is called before the first frame update
    void Start() {
        sign = this.gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            player = other.gameObject;
            // Perform actions when player enters proximity
            Debug.Log("Player is within proximity");
            playerInProx = CheckValidStop();
            if(playerInProx){
                GameManager.Instance.inCorrectStopZone = true;
                GameManager.Instance.busStopSign = sign;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")){
            playerInProx = false;
            GameManager.Instance.inCorrectStopZone = false;
            GameManager.Instance.busStopSign = null;
            Debug.Log("Player exits proximity");
        }
    }

    public bool CheckValidStop(){
        foreach (var line in associatedLines)
        {
            if(GameManager.Instance.BusLine.BusLineID.v3 == line.BusLineID.v3){
                return true;
            } 
        }
        return false;
    }

    private Coroutine updateTimeTableCoroutine;


    public void AddBusLine(BusLineSO line){
        associatedLines.Add(line);

        UpdateTime(false);
    }

    public void UpdateTime(bool addTime){
        if(updateTimeTableCoroutine != null){
            StopCoroutine(updateTimeTableCoroutine);
        }

        updateTimeTableCoroutine = StartCoroutine(UpdateTimeTable(addTime));
    }

    int timeTillBusArrives = 0;

    // TODO: work to match multiple busses at inconsistent times
    IEnumerator UpdateTimeTable(bool addTime){
        yield return new WaitForSeconds(1);
        foreach(var time in timeTable){
            time.parent.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        int lineIndex = 0;

        // Time between the busses arriving to the same stop
        int timeBetween = (Mathf.RoundToInt(associatedLines[lineIndex].totalTravelTime) / 60) - 1;
        
        int index = 0; // TEMPORARY
        foreach(var time in timeTable){
            time.parent.SetActive(true);

            if(lineIndex > associatedLines.Count-1){
                lineIndex = 0;
                index++;
            }

            time.number.text = associatedLines[lineIndex].BusLineID.v1.ToString();
            time.timeTableE.text = associatedLines[lineIndex].BusLineID.v3;
            int timeToAdd = timeBetween * index;
            int busStopTime = (int)(associatedLines[lineIndex].travelTimes[busStopIndex] / 60) < 0 ? 0 : (int)(associatedLines[lineIndex].travelTimes[busStopIndex] / 60);
                        
            time.time.text = (busStopTime + timeToAdd - timeTillBusArrives).ToString();

            lineIndex++;
        }

        if(addTime)
            timeTillBusArrives++;
    }

    public void BusPassedStop(BusLineSO line){
        if(busStopIndex == 0){
            busStopIndex = line.travelTimes.Count-1;
        }
        else{
            busStopIndex--;
        }

        timeTillBusArrives = 0;
    }

    public bool CheckPlayerProximity(){
        return playerInProx;
    }
}