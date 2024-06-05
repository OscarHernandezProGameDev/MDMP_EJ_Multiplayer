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
    [SerializeField] private NetworkObject playerPrefab;

    private const string GameScene = "Game";

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

#if UNITY_SERVER || UNITY_EDITOR
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
#else
        await LaunchInMode();
#endif
    }

#if UNITY_SERVER || UNITY_EDITOR
    private async Task LaunchInMode(bool isDedicateServer)
#else
    private async Task LaunchInMode()
#endif
    {
#if UNITY_SERVER || UNITY_EDITOR
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

        hostSingleton.CreateHost(playerPrefab);

        ClientSingleton clientSingleton = Instantiate(clientPrefab);

        bool isAuthenticated = await clientSingleton.CreateClient();

        if (isAuthenticated)
        {
            clientSingleton.GameManager.GoToMainMenu();
        }

#if UNITY_SERVER || UNITY_EDITOR
        }
#endif
    }

#if UNITY_SERVER || UNITY_EDITOR
    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
#endif
}
