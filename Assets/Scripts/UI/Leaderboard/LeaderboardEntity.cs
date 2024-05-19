using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntity : MonoBehaviour
{
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;

    private FixedString32Bytes playerName;
    public ulong ClientId { get; private set; }
    public int Kills { get; private set; }
    public int Deaths { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int kills, int deaths)
    {
        ClientId = clientId;
        this.playerName = playerName;

        UpdateDeaths(deaths);
        UpdateKills(kills);
    }

    public void UpdateDeaths(int deaths)
    {
        Deaths = deaths;
        UpdateText();
    }

    public void UpdateKills(int kills)
    {
        Kills = kills;
        UpdateText();
    }

    public void UpdateText()
    {
        positionText.text = (transform.GetSiblingIndex() + 1).ToString();
        playerNameText.text = playerName.Value;
        killsText.text = Kills.ToString();
        deathsText.text = Deaths.ToString();
    }
}
