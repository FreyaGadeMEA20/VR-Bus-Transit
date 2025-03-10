using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgainCheckIn : MonoBehaviour
{
    public Light Blaatlys; // Reference to the light source on the stander

    public AudioSource audioSource;
    public AudioClip godkendtClip;
    private bool isCoroutineRunning = false; 
    
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        Blaatlys = this.GetComponentInChildren<Light>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HI");
        if(!other.gameObject.CompareTag("Skolekort"))
            return; 

        ReturnToGame();
    }

    
    private void ReturnToGame()
    {
        audioSource.clip = godkendtClip;
        Blaatlys.enabled = false;
        audioSource.Play();

        StartCoroutine(ResetLightAndAudio());
    }

    private void OnTriggerExit(Collider other)
    {
      StartCoroutine(ResetLightAndAudio()); // wait x seconds before resetting the light and audio
    }

    //Coroutine to reset the stander
    private IEnumerator ResetLightAndAudio()
    {
        if (!isCoroutineRunning)
        {
            isCoroutineRunning = true; // Set the flag to indicate the coroutine is running

            yield return new WaitForSeconds(2.0f);

            StartCoroutine(FadeToBlack.Instance.FadeOutAndLoadScene(1));

            yield return new WaitForSeconds(1.0f);

            Blaatlys.enabled = true;

            isCoroutineRunning = false; // Reset the flag when the coroutine is done
        }

    }

}