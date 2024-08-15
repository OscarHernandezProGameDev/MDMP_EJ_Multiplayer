using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CaracterSelect : MonoBehaviour
{
    public static CaracterSelect Instance;

    private NetworkObject SelectedCharacterPrefab;

    [SerializeField] private GameObject[] characters;
    [SerializeField] private int selectedCharacterIndex = 0;

    public NetworkObject[] charactersPrefabs;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectNextCharacter()
    {
        characters[selectedCharacterIndex].gameObject.SetActive(false);
        selectedCharacterIndex = (selectedCharacterIndex + 1) % characters.Length;
        characters[selectedCharacterIndex].gameObject.SetActive(true);
    }

    public void SelectPreviousCharacter()
    {
        characters[selectedCharacterIndex].gameObject.SetActive(false);
        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
            selectedCharacterIndex = characters.Length - 1;
        characters[selectedCharacterIndex].gameObject.SetActive(true);
    }

    public void ConfirmCharacter()
    {
        SelectedCharacterPrefab = charactersPrefabs[selectedCharacterIndex];
    }

    public string GetSelectdPrefabName()
    {
        return SelectedCharacterPrefab.name;
    }
}
