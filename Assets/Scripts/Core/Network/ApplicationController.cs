using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Somos un servidor dericado
        bool isDedicateServer = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        LanchInMode(isDedicateServer);
    }

    private void LanchInMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {

        }
        else
        {

        }
    }
}
