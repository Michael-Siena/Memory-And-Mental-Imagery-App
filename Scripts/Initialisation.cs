using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Initialisation : MonoBehaviour
{
    public GameObject compatibleDeviceCanvas, incompatibleDeviceCanvas;
    private void Start() 
    {
        if (!Application.isMobilePlatform)
        {
            compatibleDeviceCanvas.SetActive(true);
            incompatibleDeviceCanvas.SetActive(false);

            Application.targetFrameRate = 60;
        }
        else
        {
            compatibleDeviceCanvas.SetActive(false);
            incompatibleDeviceCanvas.SetActive(true);
        }
    }
}
