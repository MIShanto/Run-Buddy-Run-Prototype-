using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
    public Slider playerSpeed;
    public Slider playerSensitivity;
    public TextMeshProUGUI playerSpeedTXT;
    public TextMeshProUGUI playerSensitivityTXT;

    public float forwardSpeed = 60f;
    public float sensitivity = 50f;

    private void Start()
    {
        playerSpeed.value = forwardSpeed;
        playerSensitivity.value = sensitivity;

        playerSpeedTXT.text = playerSpeed.value.ToString();
        playerSensitivityTXT.text = playerSensitivity.value.ToString();

        StartCoroutine(SwitchTo2D());
    }
    // Update is called once per frame
    void Update()
    {

        //values for tweaking
        //----------start------------
        forwardSpeed = playerSpeed.value;
        playerSpeedTXT.text = forwardSpeed.ToString();

        sensitivity = playerSensitivity.value;
        playerSensitivityTXT.text = sensitivity.ToString();
        //----------end------------
    }

    

    public void Play()
    {
        PlayerPrefs.SetFloat("Speed", forwardSpeed);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);

        SceneManager.LoadScene(1);
    }

    // Call via `StartCoroutine(SwitchTo2D())` from your code. Or, use
    // `yield SwitchTo2D()` if calling from inside another coroutine.
    public IEnumerator SwitchTo2D()
    {
        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName("");

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        // Not needed, since loading the None (`""`) device takes care of this.
        // XRSettings.enabled = false;

        // Restore 2D camera settings.
        ResetCameras();
    }

    // Resets camera transform and settings on all enabled eye cameras.
    void ResetCameras()
    {
        // Camera looping logic copied from GvrEditorEmulator.cs
        for (int i = 0; i < Camera.allCameras.Length; i++)
        {
            Camera cam = Camera.allCameras[i];
            if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {

                // Reset local position.
                // Only required if you change the camera's local position while in 2D mode.
                cam.transform.localPosition = Vector3.zero;

                // Reset local rotation.
                // Only required if you change the camera's local rotation while in 2D mode.
                cam.transform.localRotation = Quaternion.identity;

                // No longer needed, see issue github.com/googlevr/gvr-unity-sdk/issues/628.
                // cam.ResetAspect();

                // No need to reset `fieldOfView`, since it's reset automatically.
            }
        }
    }
}
