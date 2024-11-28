using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody busRigb;
    private AudioSource busAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromBus;

    private void Start()
    {
        busAudio = GetComponent<AudioSource>();
        busRigb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        EngineSound();
    }

    void EngineSound()
    {
        currentSpeed = busRigb.velocity.magnitude;
        pitchFromBus = busRigb.velocity.magnitude / 50f;

        if(currentSpeed < minSpeed)
        {
            busAudio.pitch = minPitch;
        }

        if(currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            busAudio.pitch = minPitch + pitchFromBus;
        }

        if(currentSpeed > maxSpeed)
        {
            busAudio.pitch = maxPitch;
        }
    }
}
