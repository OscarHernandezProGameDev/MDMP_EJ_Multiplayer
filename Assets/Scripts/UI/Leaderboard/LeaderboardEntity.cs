using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntity : MonoBehaviour
{
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text deathsText;
    [SerializeField] private TMP_Text killsText;

    private FixedString32Bytes playerName;
    public ulong ClientId { get; private set; }
    public int Deaths { get; private set; }
    public int Kills { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int deaths, int kills)
    {
        this.ClientId = clientId;
        this.playerName = playerName;

        UpdateDeaths(deaths);
        UpdateKills(kills);
    }

    public void UpdateDeaths(int death)
    {
        Deaths = death;

        UpdateText();
    }

    public void UpdateKills(int kill)
    {
        Kills = kill;

        UpdateText();
    }

    public void UpdateText()
    {
        positionText.text = (transform.GetSiblingIndex() + 1).ToString();
        playerNameText.text = playerName.Value;
        deathsText.text = Deaths.ToString();
        killsText.text = Kills.ToString();
    }
}
