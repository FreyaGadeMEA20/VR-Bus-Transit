using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

public class Bus_VariableEngineNoise : MonoBehaviour
{
    public AudioSource engineAudioSource;
    public VehicleMovement vehicleMovement;
    public float maxVolume = 1.0f;
    public float minVolume = 0.1f;
    public float maxPitch = 2.0f;
    public float minPitch = 0.5f;
    public float smoothTime = 0.1f; // Smoothing time for transitions

    private float currentVolume;
    private float currentPitch;
    private float volumeVelocity;
    private float pitchVelocity;

    // Start is called before the first frame update
    void Start()
    {
        if (engineAudioSource == null)
        {
            engineAudioSource = GetComponent<AudioSource>();
        }

        if (vehicleMovement == null)
        {
            vehicleMovement = GetComponent<VehicleMovement>();
        }

        currentVolume = engineAudioSource.volume;
        currentPitch = engineAudioSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        float acceleration = vehicleMovement.Acceleration;
        bool brakes = vehicleMovement.Breaks; // Assuming Brakes is a public boolean property

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, acceleration);
        float targetPitch = brakes ? 1.0f : Mathf.Lerp(minPitch, maxPitch, acceleration);

        currentVolume = Mathf.SmoothDamp(currentVolume, targetVolume, ref volumeVelocity, smoothTime);
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, smoothTime);

        engineAudioSource.volume = currentVolume;
        engineAudioSource.pitch = currentPitch;
    }
}