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

    public void ActiveVirtualJoystick(StarterAssetsInputs starterAssetsInputs)
    {
        StartCoroutine(ActiveVirtualJoystickCoroutine(starterAssetsInputs));
    }

    IEnumerator ActiveVirtualJoystickCoroutine(StarterAssetsInputs starterAssetsInputs)
    {
        yield return new WaitForEndOfFrame();

        if (!isModeMobile)
            yield break;

        this.starterAssetsInputs = joystickCanvas.starterAssetsInputs = starterAssetsInputs;
        starterAssetsInputs.cursorLocked = false;
        starterAssetsInputs.cursorInputForLook = false;
        joystickCanvas.gameObject.SetActive(true);
    }
}
