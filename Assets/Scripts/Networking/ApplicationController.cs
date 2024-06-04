using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private ApplicationData applicationData;
    [SerializeField] private NetworkObject playerHostServerPrefab;
    [SerializeField] private NetworkObject playerDedicateServerPrefab;

    private const string GameScene = "Game";

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

#if DEDICATESERVER_ENABLED
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
#else
        await LaunchInMode();
#endif
    }

#if DEDICATESERVER_ENABLED
    private async Task LaunchInMode(bool isDedicateServer)
#else
    private async Task LaunchInMode()
#endif
    {
#if DEDICATESERVER_ENABLED
        if (isDedicateServer)
        {
            Application.targetFrameRate = 60;

            applicationData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
#endif
        HostSingleton hostSingleton = Instantiate(hostPrefab);

        hostSingleton.CreateHost(playerHostServerPrefab);

        ClientSingleton clientSingleton = Instantiate(clientPrefab);

        bool isAuthenticated = await clientSingleton.CreateClient();

        if (isAuthenticated)
        {
            clientSingleton.GameManager.GoToMainMenu();
        }

#if DEDICATESERVER_ENABLED
        }
#endif
    }

#if DEDICATESERVER_ENABLED
    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(playerDedicateServerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
#endif
}
