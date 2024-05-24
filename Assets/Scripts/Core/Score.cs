using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private NetworkVariable<int> kills = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> deaths = new NetworkVariable<int>();

    public void AddKill()
    {
        kills.Value++;
    }

    public void AddDeath()
    {
        deaths.Value++;
    }
}
