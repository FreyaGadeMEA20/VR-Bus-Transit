using System.Collections;
using UnityEngine;
using Polyperfect.People;

namespace Movement
{
    public class PedestrianSpawner : MonoBehaviour
    {
        public GameObject[] pedestrianPrefabs;
        public int pedestriansToSpawn;

        [SerializeField] GameObject Container;
        void Start()
        {
            if (NPCManager.Instance != null)
            {
                pedestriansToSpawn = NPCManager.Instance.NPCAmount;
                Debug.Log($"PedestrianSpawner initialized with NPCAmount: {pedestriansToSpawn}");
            }
            else
            {
                pedestriansToSpawn = 100; // Default to 0 if NPCManager is not found
                Debug.LogWarning("NPCManager instance not found. Defaulting pedestriansToSpawn to 100.");
            }

            StartCoroutine(SpawnPedestrians());
        }

        IEnumerator SpawnPedestrians()
        {
            int count = 0;
            while (count < pedestriansToSpawn)
            {
                GameObject obj = Instantiate(pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)]);
                Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
                obj.GetComponent<PedestrianWaypointNavigator>().currentWaypoint = child.GetComponent<PedestrianWaypoint>();
                obj.transform.position = child.position;
                obj.transform.parent = Container.transform;

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