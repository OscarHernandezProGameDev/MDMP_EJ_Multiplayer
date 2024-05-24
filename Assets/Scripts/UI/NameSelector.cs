using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 3;
    [SerializeField] private int maxNameLength = 15;

    public const string PlayerNameKey = "PlayerName";

    void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            LoadNextScene();
            return;
        }

        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);

        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
