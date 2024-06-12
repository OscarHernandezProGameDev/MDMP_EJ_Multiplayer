using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualJoysticks : MonoBehaviour
//: NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private bool enabledIfEditorMode;
    [SerializeField] private bool isModeMobile;

    [Header("References")]
    public UICanvasControllerInput joystickCanvas;

    private void Awake()
    {
        joystickCanvas.gameObject.SetActive(false);

        Debug.Log($"Application.isEditor: {Application.isEditor}");
        Debug.Log($"Application.isMobilePlatform: {Application.isMobilePlatform}");

        if (Application.isEditor)
        {
            isModeMobile = enabledIfEditorMode;

            return;
        }
        if (!Application.isMobilePlatform)
        {
            isModeMobile = false;

            return;
        }

        isModeMobile = true;
    }

    public void ActiveVirtualJoystick(StarterAssetsInputs starterAssetsInputs = null)
    {
        StartCoroutine(ActiveVirtualJoystickCoroutine(starterAssetsInputs));
    }

    IEnumerator ActiveVirtualJoystickCoroutine(StarterAssetsInputs starterAssetsInputs = null)
    {
        yield return new WaitForEndOfFrame();

        if (!isModeMobile)
            yield break;

        if (starterAssetsInputs is not null)
            this.starterAssetsInputs = starterAssetsInputs;

        if (this.starterAssetsInputs is not null)
        {
            joystickCanvas.starterAssetsInputs = this.starterAssetsInputs;

            this.starterAssetsInputs.cursorLocked = false;
            this.starterAssetsInputs.cursorInputForLook = false;
            joystickCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("this.starterAssetsInputs is null");
            joystickCanvas.gameObject.SetActive(false);
        }
    }
}
