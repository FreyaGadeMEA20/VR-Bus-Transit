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

        if(updateTimeTableCoroutine != null){
            StopCoroutine(updateTimeTableCoroutine);
        }

        updateTimeTableCoroutine = StartCoroutine(UpdateTimeTable());
    }

    IEnumerator UpdateTimeTable(){
        yield return new WaitForSeconds(1);
        foreach(var time in timeTable){
            time.parent.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        int index = 0;
        int TIME = 2; // TEMPORARY
        foreach(var time in timeTable){
            time.parent.SetActive(true);

            if(index > associatedLines.Count-1){
                index = 0;
            }

            time.number.text = associatedLines[index].BusLineID.v1.ToString();
            time.timeTableE.text = associatedLines[index].BusLineID.v3;
            time.time.text = TIME.ToString();
            TIME += 5; // TEMPORARY
            Debug.Log(index);
            index++;
        }
    }

    public bool CheckPlayerProximity(){
        return playerInProx;
    }
}

internal class SerializedTuplesLabelsAttribute : Attribute
{
    private string v1;
    private string v2;

    public SerializedTuplesLabelsAttribute(string v1, string v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }
}