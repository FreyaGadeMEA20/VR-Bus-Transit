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
        if(birdPOVCam != null) birdPOVCam.Priority = 0;
        if(followPlayerCam != null) followPlayerCam.Priority = 0;
    }

    public void SwitchToBirdPOVCam()
    {
        playerCam.Priority = 0;
        if(birdPOVCam != null) birdPOVCam.Priority = 1;
        if(followPlayerCam != null) followPlayerCam.Priority = 0;
    }

    public void SwitchToFollowPlayerCam()
    {
        playerCam.Priority = 0;
        if(birdPOVCam != null) birdPOVCam.Priority = 0;
        if(followPlayerCam != null) followPlayerCam.Priority = 1;
    }

    // Start is called before the first frame update
    void Start() => SwitchToPlayerCam();
}
