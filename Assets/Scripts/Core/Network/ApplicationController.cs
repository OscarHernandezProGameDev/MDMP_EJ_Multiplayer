using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Somos un servidor dericado
        bool isDedicateServer = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        await LanchInModeAsync(isDedicateServer);
    }

    private async Task LanchInModeAsync(bool isDedicateServer)
    {
        if (isDedicateServer)
        {

        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(clientPrefab);

            await clientSingleton.CreateClientAsync();

            HostSingleton hostSingleton = Instantiate(hostPrefab);

            hostSingleton.CreateHost();
        }
    }
}
