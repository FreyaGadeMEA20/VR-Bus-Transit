using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataGatherer : MonoBehaviour
{
    public static DataGatherer Instance { get; internal set; }
    string fileName = "";
    string[] rowDataTemp = new string[6];
    string rowData;
    StreamWriter streamWriter;

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
        string folderPath = Application.persistentDataPath; // Use persistentDataPath for Meta Quest compatibility
        string baseFileName = "TestData";
        string extension = ".csv";
        int fileNumber = 1;

        do
        {
            fileName = $"{folderPath}/{baseFileName}{fileNumber}{extension}";
            fileNumber++;
        } while (File.Exists(fileName));

        // Initialize StreamWriter
        streamWriter = new StreamWriter(fileName, true);
        rowDataTemp = new string[] { "Time", "Position", "Difficulty", "GameState", "BusEndStation", "BusToTake" };
        rowData = string.Join(",", rowDataTemp);
        streamWriter.WriteLine(rowData);
        streamWriter.Flush();
    }

    public void WriteToCSV(string time, string position, string gameState, string busEndStation, string busToTake)
    {
        rowDataTemp[0] = time;
        rowDataTemp[1] = position;
        rowDataTemp[2] = GameSettings.Instance.LevelSelected.ToString();
        rowDataTemp[3] = gameState;
        rowDataTemp[4] = busEndStation;
        rowDataTemp[5] = busToTake;

        rowData = string.Join(",", rowDataTemp);
        streamWriter.WriteLine(rowData);
        streamWriter.Flush();
    }

    private void OnDestroy()
    {
        // Ensure the StreamWriter is properly closed when the object is destroyed
        if (streamWriter != null)
        {
            streamWriter.Close();
        }
    }
}
