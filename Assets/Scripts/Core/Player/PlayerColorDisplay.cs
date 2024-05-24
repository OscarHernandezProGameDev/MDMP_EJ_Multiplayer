using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TeamColorLookup teamColorLookup;
    [SerializeField] private SetPlayerData player;
    [SerializeField] private TMP_Text playerName;

    private void Start()
    {
        HandleTeamChanged(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandleTeamChanged;
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandleTeamChanged;
    }

    private void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColor = teamColorLookup.GetTeamColor(newTeamIndex);
        playerName.color = teamColor;
    }
}
