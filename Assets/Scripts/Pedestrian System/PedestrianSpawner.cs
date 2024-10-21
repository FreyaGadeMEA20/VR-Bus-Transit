using System.Collections;
using System.Collections.Generic;
using Polyperfect.People;
using UnityEngine;

namespace Movement{
    public class PedestrianSpawner : MonoBehaviour
    {
        public GameObject[] pedestrianPrefabs;

        public int pedestriansToSpawn;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SpawnPedestrians());
        }

        IEnumerator SpawnPedestrians()
        {
            int count = 0;
            while (count < pedestriansToSpawn)
            {
                Debug.Log(count);
                GameObject obj = Instantiate(pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)]);
                Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
                obj.GetComponent<PedestrianWaypointNavigator>().currentWaypoint = child.GetComponent<PedestrianWaypoint>();
                obj.transform.position = child.position;
                
                StartCoroutine(StopAnimation(obj));

                count++;
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator StopAnimation(GameObject obj){
            yield return new WaitForSeconds(2);
            
            obj.GetComponent<People_WanderScript>().enabled = false;
        }
    }
}