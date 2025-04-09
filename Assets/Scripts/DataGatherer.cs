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

    // Initiatlize the singleton instance of the DataGatherer
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
        // Generate a unique file name
        string folderPath = Application.persistentDataPath; // Use persistentDataPath for Meta Quest compatibility
        string baseFileName = "TestData"; // Name of file
        string extension = ".csv"; // File extension
        int fileNumber = 1; // File number to append to the file name, to seperate the files

        // Check if the file already exists and increment the file number until a unique name is found
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

    // Write data to CSV file
    // The parameters are the data to be written to the CSV file
    public void WriteToCSV(string time, string position, string gameState, string busEndStation, string busToTake)
    {
        // save as a temporary array and then writes it to the file
        rowDataTemp[0] = time;
        rowDataTemp[1] = position;
        rowDataTemp[2] = GameSettings.Instance.LevelSelected.ToString();
        rowDataTemp[3] = gameState;
        rowDataTemp[4] = busEndStation;
        rowDataTemp[5] = busToTake;

        // Write the data to the CSV file
        rowData = string.Join(",", rowDataTemp);
        streamWriter.WriteLine(rowData);
        streamWriter.Flush(); // Ensure data is written immediately
    }

    // Makes sure the device doesn't die by killing the StreamWriter when the application closes
    private void OnDestroy()
    {
        // Ensure the StreamWriter is properly closed when the object is destroyed
        if (streamWriter != null)
        {
            streamWriter.Close();
        }
    }
}
