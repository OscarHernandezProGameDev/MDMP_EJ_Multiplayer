using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class LeaderboardControler : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    private GameObject leaderboard;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        starterAssetsInputs.OnLeaderboardEvent += HandleLeaderboard;
        leaderboard = FindAnyObjectByType<Leaderboard>().transform.GetChild(0).gameObject;
        leaderboard.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        starterAssetsInputs.OnLeaderboardEvent -= HandleLeaderboard;
    }

    private void HandleLeaderboard(bool isBottonPressed)
    {
        if (isBottonPressed)
        {
            leaderboard.SetActive(true);
        }
        else
        {
            leaderboard.SetActive(false);
        }
    }
}
