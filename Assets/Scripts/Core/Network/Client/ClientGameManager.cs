using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string MainMenu = "MainMenu";

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationManager.DoAuthAsync();

        if (authState == AuthState.Authenticated)
            return true;

        return false;
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }
}
