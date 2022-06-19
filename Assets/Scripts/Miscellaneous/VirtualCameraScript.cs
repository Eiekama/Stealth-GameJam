using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraScript : MonoBehaviour
{
    public static CinemachineVirtualCamera virtualCam;

    void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
    }
}
