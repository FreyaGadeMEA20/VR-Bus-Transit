using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Prefab of the car to be spawned
    public GameObject busPrefab;
    //public List<Transform> startWaypoints; // List of start waypoints where cars can spawn
    public Waypoints waypoints; // Reference to the Waypoints script
    public float spawnIntervalMin = 1f; // Minimum interval between car spawns
    public float spawnIntervalMax = 2f; // Maximum interval between car spawns
    public int routeIndex;

    public float busSpawnDelay = 5f;
    public int busRouteIndex;
    public bool hasSpawnedBus = false;
    public bool canSpawnBus = false;

    public bool doSpawnCars = true;

    public int maxActiveCars = 15;
    public int currentActiveCars = 0;

    float timeElapsed = 0f;

    private List<Transform> startWaypoints = new List<Transform>();

    void Start()
    {
        // Get the Waypoints script attached to the Waypoints game object
        waypoints = GameObject.Find("Waypoints").GetComponent<Waypoints>();

        // Populate the list of start waypoints
        PopulateStartWaypoints();

        // Start spawning cars
        StartCoroutine(SpawnEntities());

    }

    void Update(){
        timeElapsed += Time.deltaTime;
    }

    private void PopulateStartWaypoints()
    {
        startWaypoints.Clear();

        foreach (var route in waypoints.routes){
            if (route.waypoints.Count > 0){
                startWaypoints.Add(route.waypoints[0]);
            }

            else{
                Debug.LogWarning("No waypoints found in the route");
            }
        }
    }

    private IEnumerator SpawnEntities(){
        float waitTime;

        while (true){
            waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(waitTime);            

            if (timeElapsed >= busSpawnDelay && !hasSpawnedBus && canSpawnBus){
                hasSpawnedBus = true;
                SpawnBus();
                Debug.Log("Bus Spawned");
                timeElapsed = 0f;
            }

            else {
                if (doSpawnCars){
                    if (currentActiveCars < maxActiveCars){
                        SpawnCar();
                    }
                }
            }

            //Debug.Log("Time Elapsed: " + timeElapsed);

        }
    }

    private void SpawnCar()
    {
        //while (true){
        //yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

        routeIndex = Random.Range(0, waypoints.routes.Count);
        //Debug.Log("Chosen Route Index assigned to car: " + routeIndex);

        Transform startWaypoint = waypoints.routes[routeIndex].waypoints[0];
        //Debug.Log("Start Waypoint: " + startWaypoint.name);

        // Select a random prefab from the carPrefabs array
        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

        GameObject newCar = Instantiate(carPrefab, startWaypoint.position, startWaypoint.rotation);
        //Debug.Log("Car Spawned: " + newCar.name + " at " + startWaypoint.position + " with index " + routeIndex);

        WaypointMover waypointMover = newCar.GetComponent<WaypointMover>();
        waypointMover.entityType = "Car";

        if (waypointMover != null){
            waypointMover.waypoints = waypoints;
            //Debug.Log("waypointMover Waypoints assigned to car: " + waypoints.name);
            //waypointMover.SetRouteIndex(routeIndex);
            //Debug.Log("waypointMover Route index assigned to car: " + routeIndex);
        }

        else{
            Debug.LogWarning("WaypointMover component not found");
        }
        //}

        IncrementActiveCars();
    }

    private void SpawnBus(){
        Transform startWaypoint = waypoints.routes[busRouteIndex].waypoints[0];

        GameObject newBus = Instantiate(busPrefab, startWaypoint.position, startWaypoint.rotation);

        WaypointMover waypointMover = newBus.GetComponent<WaypointMover>();
        waypointMover.entityType = "Bus";
/* 
        TeleportationArea TP = GameObject.FindWithTag("TeleportArea").GetComponent<TeleportationArea>();
        MeshCollider busFloor = GameObject.FindWithTag("BusFloor").GetComponent<MeshCollider>();
        TP.collider[1] = busFloor; */

        if (waypointMover != null){
            waypointMover.waypoints = waypoints;
        }

        else{
            Debug.LogWarning("WaypointMover component not found");
        }
    }

    public void IncrementActiveCars(){
        currentActiveCars++;
    }

    public void DecrementActiveCars(){
        currentActiveCars--;
    }
}

// OLD
        /* // Infinite loop for continuously spawning cars
        while (true)
        {
            // Wait for a random amount of time between spawn intervals
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            // Select a random start waypoint for the car
            Transform startWaypoint = startWaypoints[Random.Range(0, startWaypoints.Count)];

            // Instantiate a car at the selected start waypoint
            GameObject newCar = Instantiate(carPrefab, startWaypoint.position, startWaypoint.rotation);

            // Get the WaypointMover component attached to the spawned car
            WaypointMover carMover = newCar.GetComponent<WaypointMover>();

            // If the WaypointMover component is found, assign a random route index to the car
            if (carMover != null)
            {
                carMover.waypoints = GameObject.Find("Waypoints").GetComponent<Waypoints>();
                carMover.routeIndex = Random.Range(0, carMover.waypoints.routes.Count);
            }
            else
            {
                // Log a warning if the WaypointMover component is not found
                Debug.LogWarning("WaypointMover component not found on the spawned car.");
            }
        } */