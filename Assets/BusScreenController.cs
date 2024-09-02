using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusScreenController : MonoBehaviour
{
    public List<BusStop> stopList;
    public Material newMat; // Reference to the material renderer


    private int currentIndex = 0; // Index of the current image

    [System.Serializable]
    public class BusStop {
        public GameObject wayPoint;
        public Texture nextStopDot;
        public Texture nextStopStop;
    }

    // Start is called before the first frame update
    void Start()
    {
        //newMat = Resources.Load("ScreenInfo", typeof(Material)) as Material;
        
        // Check if there are any images in the list
        if (stopList.Count > 0)
        {
            // Set the initial image
            newMat.mainTexture = stopList[currentIndex].nextStopDot;
        }
    }

    // Update is called once per frame
    /* void Update()
    {
        // Check for input to change the image
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyNextTexture();
        }

        if (Input.GetKeyDown(KeyCode.M)){
            ApplyStopTexture();
        }
    } */

    public void ApplyNextTexture(){
        // Increment the index
        currentIndex++;

        // Wrap around to the beginning if the index exceeds the list size
        if (currentIndex >= stopList.Count)
        {
            currentIndex = 0;
        }

        // Apply the new image to the material
        newMat.mainTexture = stopList[currentIndex].nextStopDot;
    }

    public void ApplyStopTexture(){
        newMat.mainTexture = stopList[currentIndex].nextStopStop;
    }
}
