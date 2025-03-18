using System.Collections;
using System.Collections.Generic;
using Polyperfect.People;
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
            if (Slider_NPCs.Instance != null)
            {
                Slider_NPCs.Instance.SubscribeToNPCAmountChanged(UpdatePedestriansToSpawn);
                pedestriansToSpawn = Slider_NPCs.Instance.NPCAmount;
                Debug.Log("PedestrianSpawner initialized with NPCAmount: " + pedestriansToSpawn);
            }
            else
            {
                pedestriansToSpawn = PlayerPrefs.GetInt("NPCAmount", 0); // Default to 0 if not found
                Debug.LogWarning("Slider_NPCs instance not found. Retrieved NPCAmount from PlayerPrefs: " + pedestriansToSpawn);
            }

            spawnCoroutine = StartCoroutine(SpawnPedestrians());
        }

        void UpdatePedestriansToSpawn(int newAmount)
        {
            pedestriansToSpawn = newAmount;
            Debug.Log("PedestrianSpawner updated NPCAmount to: " + pedestriansToSpawn);

            // Restart the spawning coroutine to handle the updated value
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            spawnCoroutine = StartCoroutine(SpawnPedestrians());
        }

        void OnDestroy()
        {
            // Unsubscribe from the event to avoid memory leaks
            if (Slider_NPCs.Instance != null)
            {
                Slider_NPCs.Instance.OnNPCAmountChanged -= UpdatePedestriansToSpawn;
            }
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

                StartCoroutine(StopAnimation(obj));

                count++;
                yield return new WaitForSeconds(0.1f); // Optional delay between spawns
            }
        }

        IEnumerator StopAnimation(GameObject obj)
        {
            yield return new WaitForSeconds(2);

            obj.GetComponent<People_WanderScript>().enabled = false;
        }
    }
}