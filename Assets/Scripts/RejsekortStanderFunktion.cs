using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RejskortStanderFunctions : MonoBehaviour
{
    public Light Blaatlys; // Reference to the light source on the stander

    public AudioSource audioSource;
    public AudioClip godkendtClip;
    public AudioClip afvistClip;

    public Material checkedIn_GodRejse; // Material til Godkendt
    public Material checkedIn_alreadyCheckedIn; // Material Til afvist
    public Material checkedOut_GodRejse; // Material til Godkendt
    public Material checkedOut_alreadyCheckedOut; // Material Til afvist

    public Renderer ScreenText; // Reference til det object der skal have �ndret materiale

    private Material originalMaterial;
   
    private bool canInteract; // Can the player interact with the stander?
    private bool isCoroutineRunning = false; 
    public bool checkIndStander; // If this is a checkInd stander, then this is true. If it is a checkUd stander, then this is false.

    [SerializeField] BusController Bus;
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        Blaatlys = this.GetComponentInChildren<Light>();

        ScreenText = transform.parent.Find("ScreenText").GetComponent<Renderer>();

        canInteract = true;

        // Store original material into originalMaterial variable
        if (ScreenText != null)
        {
            originalMaterial = ScreenText.material;
        }

        //Bus = GameObject.FindGameObjectWithTag("Bus").GetComponent<BusController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(checkIndStander)
        {
            if (canInteract)
            {
                if (other.gameObject.CompareTag("Skolekort"))
                {
                    if (Bus.HasCheckedIn)
                    {
                        // Already checked in
                        HandleAlreadyCheckedIn();
                    }
                    else
                    {
                        // Perform check-in
                        HandleCheckIn();

                        /* if(!Bus.CorrectBus){
                            Bus.WrongBus();
                        } */
                    }
                }
            }
            else
            {
                Debug.Log("Cannot interact");
            }
        }
        else
        {
            if (canInteract)
            {
                if (other.CompareTag("Skolekort"))
                {
                    if (Bus.HasCheckedIn)
                    {
                        // Already checked in
                        HandleCheckOut();
                    }
                    else
                    {
                        HandleAlreadyCheckedOut();
                        // Perform check-in

                    }
                }
            }
            else
            {
                Debug.Log("Cannot interact");
            }
        }
    }

    private void HandleAlreadyCheckedIn()
    {
        Blaatlys.enabled = false;
        audioSource.clip = afvistClip;
        audioSource.Play();
        canInteract = false;
        Debug.Log("Already checked in");

        // Change material on ScreenText to AfvistText
        if (ScreenText != null && checkedIn_alreadyCheckedIn != null)
        {
            ScreenText.material = checkedIn_alreadyCheckedIn;
        }
        StartCoroutine(ResetLightAndAudio());
    }
    private void HandleCheckIn()
    {
        audioSource.clip = godkendtClip;
        Blaatlys.enabled = false;
        audioSource.Play();
        canInteract = false;
        Bus.HasCheckedIn = true;

        // Change material on ScreenText to OkText
        if (ScreenText != null && checkedIn_GodRejse != null)
        {
            Debug.Log("Checked in");
            ScreenText.material = checkedIn_GodRejse;
        }

        StartCoroutine(ResetLightAndAudio());
    }

    private void HandleAlreadyCheckedOut()
    {
        Blaatlys.enabled = false;
        audioSource.clip = afvistClip;
        audioSource.Play();
        canInteract = false;
        Debug.Log("Already checked out");
        // Change material on ScreenText to AfvistText
        if (ScreenText != null && checkedIn_alreadyCheckedIn != null)
        {
            ScreenText.material = checkedOut_alreadyCheckedOut;
        }
        StartCoroutine(ResetLightAndAudio());
    }
    private void HandleCheckOut()
    {
        audioSource.clip = godkendtClip;
        Blaatlys.enabled = false;
        audioSource.Play();
        canInteract = false;
        Bus.HasCheckedIn = false;
        Debug.Log("Checked out");

        // Change material on ScreenText to OkText
        if (ScreenText != null && checkedIn_GodRejse != null)
        {
            ScreenText.material = checkedOut_GodRejse;
        }

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

            yield return new WaitForSeconds(4.0f);

            Blaatlys.enabled = true;
            
            canInteract = true;

            if (ScreenText != null)
            {
                ScreenText.material = originalMaterial;
            }

            isCoroutineRunning = false; // Reset the flag when the coroutine is done
        }

    }

}