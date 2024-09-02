using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PositionTracker : MonoBehaviour
{
    private StreamWriter csvWriter;     // Writing to a CSV File
    [SerializeField] string id;

    private float timer = 0f;
    private float interval = 1f;
    // Start is called before the first frame update
    void Start()
    {

        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        
        // Initialize the StreamWriter to write to a CSV file
        string filePath = @"Assets\CSV_Data\Position_"+id+".csv"; // Set your desired file path
        csvWriter = new StreamWriter(filePath, true);

         // Check if the file is empty (indicating the start of a new session) and write headers
        if (csvWriter.BaseStream.Length == 0)
        {
            csvWriter.WriteLine("PositionX, PositionZ, time"); // Write header
        }

        StartCoroutine(TimerCoroutine());
    }

    // Update is called once per frame

    void Update()
    {
    }
    
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            timer += interval;
            WriteToCSV();
        }
    }

    private void WriteToCSV()
    {
        Vector2 positions = GetPosition();
        
        //"User/ID, SceneNR, Blink, Green/Blue, TotalTime, TimeBeforeBlink, SceneTime, POI, DiffFromPOI, AverageTotalDiff"
        csvWriter.WriteLine($"{positions[0]}, {positions[1]}, {timer}");
        csvWriter.Flush(); // Flush to ensure data is written immediately
    }

    private Vector2 GetPosition()
    {
        Vector3 position = transform.position;
        return new Vector2(position.x, position.z);
    }

    private void OnApplicationQuit()
    {
        // Close the StreamWriter when the application is exiting
        if (csvWriter != null)
        {
            csvWriter.Close();
        }
    }
}
