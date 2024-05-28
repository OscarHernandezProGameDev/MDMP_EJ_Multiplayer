using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeamColorLookup", menuName = "Team Color Lookup")]
public class TeamColorLookup : ScriptableObject
{
    [SerializeField] private Color[] teamColors;

    public Color GetColor(int teamIndex)
    {
        return (teamIndex < 0 || teamIndex >= teamColors.Length) ? Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) : teamColors[teamIndex];
    }
}
