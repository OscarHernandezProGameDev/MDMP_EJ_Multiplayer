using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}

public static class AuthenticationManager
{
    public static AuthState State { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuthAsync(int maxTries = 5)
    {
        if (State == AuthState.Authenticated)
        {
            return State;
        }

        State = AuthState.Authenticating;

        int tries = 0;

        while (State == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                State = AuthState.Authenticated;

                break;
            }

            tries++;

            await Task.Delay(1000);
        }

        return State;
    }
}
