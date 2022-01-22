using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CameraPlayerInteraction : MonoBehaviour
{
	public static CameraPlayerInteraction Instance;

    private void Awake()
	{		
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

	}

	Camera cam;
	Animator animator;
	CharacterController characterController;

	public float playerForwardspeed = 40f;
	public float playerLeftRightspeed = 10f;
	public float jumpForce = 50;
	public float gravity = 15f;

	float moveDirectionForTilt;    // when tilts the headset..
	float xRotationEularAngle;    // when looks in up or down gets the eular angle..
	float verticalVelocity;
	float moveDirectionForSideLooking;


	bool canJump = true;
	bool canSlide = true;
	bool canRun = true;

	bool isGameReadyToRun = false;

	private IEnumerator Start()
	{
		StartCoroutine(SwitchToVR());
		cam = Camera.main;
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();

		playerForwardspeed = PlayerPrefs.GetFloat("Speed", 0);
		playerLeftRightspeed = PlayerPrefs.GetFloat("Sensitivity", 0);

		yield return new WaitForSeconds(4f);
		isGameReadyToRun = true;
	}
	private void Update()
	{

		#region Experiment
		/*		if (cam.transform.eulerAngles.z < lastZVal)
		{
			Debug.Log("Decreased!");

			horizontal = -1;

			//Update lastZVal
			lastZVal = cam.transform.eulerAngles.z;
		}

		else if (cam.transform.eulerAngles.z > lastZVal)
		{
			Debug.Log("Increased");

			horizontal = 1;

			//Update lastZVal
			lastZVal = cam.transform.eulerAngles.z;
		}
		else if (cam.transform.eulerAngles.z >= 359.5f && cam.transform.eulerAngles.z <= 0.5f)
		{
			horizontal = 0;
		}

		characterController.Move(new Vector3(horizontal * playerLeftRightspeed, 0, 0) * Time.deltaTime);*/


		//Debug.LogError(gvrEditorEmulator.HeadRotation.eulerAngles);
		/*xRotationValue = gvrEditorEmulator.HeadRotation.x;
		yRotationValue = Mathf.Clamp(gvrEditorEmulator.HeadRotation.y, -0.5f, 0.5f);
		zRotationValue = gvrEditorEmulator.HeadRotation.z;
		xRotationEularAngle = gvrEditorEmulator.HeadRotation.eulerAngles.x;*/

		//xRotationValue = cam.transform.rotation.x;
		//yRotationValue = Mathf.Clamp(cam.transform.rotation.y, -0.5f, 0.5f); 
		#endregion

		if (isGameReadyToRun)
		{
			MovePlayerForward();

			//moveDirectionForTilt = cam.transform.rotation.z; // used for movement during tilt
			xRotationEularAngle = cam.transform.eulerAngles.x; // used for jump.

			moveDirectionForSideLooking = CalculateRotation_XZ(cam.transform.eulerAngles.y);

			moveDirectionForTilt = CalculateRotation_XZ(cam.transform.eulerAngles.z);

			HandleLeftRightWhenTilted();

			HandleLeftRightWhenRotate();

			HandleJumpAndSlide();
		}


	}

	private float CalculateRotation_XZ(float rotationalAngle)
	{
		if (rotationalAngle >= 0 && rotationalAngle <= 180)
		{
			Mathf.Clamp(rotationalAngle, 0f, 85f);
		}
		else if (rotationalAngle > 180 && rotationalAngle <= 360)
		{
			Mathf.Clamp(rotationalAngle, 270f, 360f);
		}

		return Mathf.Sin(rotationalAngle * Mathf.Deg2Rad); // used for movement during lokking on side..
	}


	private void HandleJumpAndSlide()
	{
		if (characterController.isGrounded)
		{
			verticalVelocity -= gravity * Time.deltaTime;

			if (xRotationEularAngle >= 275f && xRotationEularAngle <= 350f && canJump)
			{
				animator.Play("Jump");
				verticalVelocity = jumpForce;
				canJump = false;
			}
			else if (xRotationEularAngle >= 10f && xRotationEularAngle <= 80f && canSlide)
			{
				animator.Play("Slide");

				canSlide = false;
				canRun = false;
				canJump = false;
			}
			else if(canRun)
			{
				animator.Play("Run");
				canJump = true;
				canSlide = true;
			}
		}
		else
		{
			if(xRotationEularAngle >= 5f && xRotationEularAngle <= 80f)
				verticalVelocity -= gravity * Time.deltaTime * 5f;
			else
				verticalVelocity -= gravity * Time.deltaTime;
		}
	}


	private void HandleLeftRightWhenTilted()
	{

		characterController.Move(new Vector3(moveDirectionForTilt * playerLeftRightspeed * -1, 0, 0) * Time.deltaTime);

	}
	private void HandleLeftRightWhenRotate()
	{

		characterController.Move(new Vector3(moveDirectionForSideLooking * playerLeftRightspeed , 0, 0) * Time.deltaTime);

	}

	private void MovePlayerForward()
	{
		Vector3 moveVector = Vector3.zero;
		moveVector.x = 0;
		moveVector.y = verticalVelocity;
		moveVector.z = playerForwardspeed;
		characterController.Move(moveVector * Time.deltaTime);
		
	}

	public void ActivateRun()
	{
		
		canRun = true;
	}

	public void PlayerSpeed(float val)
	{
		playerForwardspeed = val;
	}
	public void PlayerSensitivity(float val)
	{
		playerLeftRightspeed = val;
	}

	public void OnTriggerEnter(Collider other)
	{
		SceneManager.LoadScene(0);
	}

	

	// Call via `StartCoroutine(SwitchToVR())` from your code. Or, use
	// `yield SwitchToVR()` if calling from inside another coroutine.
	IEnumerator SwitchToVR()
	{
		// Device names are lowercase, as returned by `XRSettings.supportedDevices`.
		string desiredDevice = "cardboard"; // Or "daydream".

		// Some VR Devices do not support reloading when already active, see
		// https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
		if (String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
		{
			XRSettings.LoadDeviceByName(desiredDevice);

			// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
			yield return null;
		}

		// Now it's ok to enable VR mode.
		XRSettings.enabled = true;
	}
}