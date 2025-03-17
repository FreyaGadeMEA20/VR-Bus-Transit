using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera birdPOVCam;
    public CinemachineVirtualCamera followPlayerCam;

    public void SwitchToPlayerCam()
    {
        playerCam.Priority = 1;
        birdPOVCam.Priority = 0;
        followPlayerCam.Priority = 0;
        Debug.Log("Switched to player cam");
    }

    public void SwitchToBirdPOVCam()
    {
        playerCam.Priority = 0;
        birdPOVCam.Priority = 1;
        followPlayerCam.Priority = 0;
        Debug.Log("Switched to bird POV cam");
    }

    public void SwitchToFollowPlayerCam()
    {
        playerCam.Priority = 0;
        birdPOVCam.Priority = 0;
        followPlayerCam.Priority = 1;
        Debug.Log("Switched to follow player cam");
    }

    public void Test()
    {
        Debug.Log("Test");
    }

    // Start is called before the first frame update
    void Start() => SwitchToPlayerCam();
}
