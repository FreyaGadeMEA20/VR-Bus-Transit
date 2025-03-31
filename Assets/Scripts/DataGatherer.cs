using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGatherer : MonoBehaviour
{
    public static DataGatherer Instance { get; internal set; }
    string fileName = "";
    string[] rowDataTemp = new string[6];
    string rowData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }   
    }
    // Start is called before the first frame update
    void Start()
    {
        string folderPath = Application.dataPath;
        string baseFileName = "Data/TestData";
        string extension = ".csv";
        int fileNumber = 1;

        do
        {
            fileName = $"{folderPath}/{baseFileName}{fileNumber}{extension}";
            fileNumber++;
            Debug.Log(fileName);
        } while (System.IO.File.Exists(fileName));
        rowDataTemp = new string[] { "Time", "Position", "Difficulty", "GameState", "BusEndStation", "BusToTake" };
        rowData = string.Join(",", rowDataTemp);
        System.IO.File.AppendAllText(fileName, rowData + "\n");
    }

    public void WriteToCSV(string time, string position, string gameState, string busEndStation, string busToTake){
        rowDataTemp[0] = time;
        rowDataTemp[1] = position;
        rowDataTemp[2] = "1";//GameSettings.Instance.LevelSelected.ToString();
        rowDataTemp[3] = gameState;
        rowDataTemp[4] = busEndStation;
        rowDataTemp[5] = busToTake;

        rowData = string.Join(",", rowDataTemp);
        Debug.Log(fileName);
        System.IO.File.AppendAllText(fileName, rowData + "\n");
    }
}
