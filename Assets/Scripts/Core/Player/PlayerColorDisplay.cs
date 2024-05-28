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
        player.TeamIndex.OnValueChanged += HandleTeamChanged;
        HandleTeamChanged(0, player.TeamIndex.Value);
    }

    private void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColor = teamColorLookup.GetColor(newTeamIndex);
        playerName.color = teamColor;
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandleTeamChanged;
    }
}
