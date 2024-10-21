using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public class PedestrianWaypointNavigator : MonoBehaviour
    {

        PedestrianNavigationController controller;
        public PedestrianWaypoint currentWaypoint;

        int direction;

        private void Awake()
        {
            controller = GetComponent<PedestrianNavigationController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            direction = Mathf.RoundToInt(Random.Range(0f, 1f));
            controller.SetDestination(currentWaypoint.GetPosition());
        }

        // Update is called once per frame
        void Update()
        {
            if(controller.reachedDestination)
            {
                bool shouldBranch = false;

                if(currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
                {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
                }

                if (shouldBranch)
                {
                    PedestrianWaypoint tempWaypoint = (PedestrianWaypoint)currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];

                    if(tempWaypoint.TrafficState == PedestrianWaypoint.TrafficLightState.Green){
                        currentWaypoint = tempWaypoint;
                    } else {
                        shouldBranch = false;
                    }
                }
                if (!shouldBranch)
                {
                    if(direction == 0)
                    {
                        if (currentWaypoint.nextWaypoint != null)
                        {
                            currentWaypoint = (PedestrianWaypoint)currentWaypoint.nextWaypoint;
                        }
                        else
                        {
                            currentWaypoint = (PedestrianWaypoint)currentWaypoint.previousWaypoint;
                            direction = 1;
                        }
                    }
                    else if(direction == 1)
                    {
                        if(currentWaypoint.previousWaypoint != null)
                        {
                            currentWaypoint = (PedestrianWaypoint)currentWaypoint.previousWaypoint;
                        }
                        else
                        {
                            currentWaypoint = (PedestrianWaypoint)currentWaypoint.nextWaypoint;
                            direction = 0;
                        }
                    }
                }
                controller.SetDestination(currentWaypoint.GetPosition());
            }
        }
    }
}