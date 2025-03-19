using System.Collections;
using UnityEngine;

namespace Movement
{
    public class PedestrianSpawner : MonoBehaviour
    {
        public GameObject[] pedestrianPrefabs;
        public int pedestriansToSpawn;

        [SerializeField] GameObject Container;

        private Coroutine spawnCoroutine;

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

            spawnCoroutine = StartCoroutine(SpawnPedestrians());
        }

        IEnumerator SpawnPedestrians()
        {
            // Clear existing NPCs if necessary
            foreach (Transform child in Container.transform)
            {
                Destroy(child.gameObject);
            }

            int count = 0;
            while (count < pedestriansToSpawn)
            {
                GameObject obj = Instantiate(pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)]);
                Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
                obj.GetComponent<PedestrianWaypointNavigator>().currentWaypoint = child.GetComponent<PedestrianWaypoint>();
                obj.transform.position = child.position;
                obj.transform.parent = Container.transform;

                count++;
                yield return new WaitForSeconds(0.1f); // Optional delay between spawns
            }
        }
    }
}