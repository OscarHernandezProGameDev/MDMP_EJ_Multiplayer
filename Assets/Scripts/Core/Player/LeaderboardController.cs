using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    private GameObject leaderboard;
    private StarterAssetsInputs _input;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        _input = GetComponent<StarterAssetsInputs>();

        //inputReader.OnLeaderboardEvent += HandleLeaderboard;

        leaderboard = FindObjectOfType<Leaderboard>().gameObject.transform.GetChild(0).gameObject;
        leaderboard.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        //inputReader.OnLeaderboardEvent -= HandleLeaderboard;
    }

    private void Update()
    {
        if (_input.leaderboard)
        {
            leaderboard.SetActive(true);
        }
        else
        {
            leaderboard.SetActive(false);
            //_input.leaderboard = false;
        }
    }
}
