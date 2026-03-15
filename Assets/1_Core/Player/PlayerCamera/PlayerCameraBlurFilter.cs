using UnityEngine;
using UnityEngine.Rendering;


public class PlayerCameraBlurFilter : MonoBehaviour
{
	private MenuManager menuManager;
	private Volume volumeMainCamera;
	private Volume volumeFirstPersonCamera;

	public void Initialize(MenuManager manager)
	{
		menuManager = manager;

		volumeMainCamera = GetComponent<Volume>();
		Transform firstPersonCameraTransform = transform.Find("FirstPerson Camera");
		volumeFirstPersonCamera = firstPersonCameraTransform.GetComponent<Volume>();
		Debug.Log("CameraBlurFilter initialized.");

		menuManager.OnOpenAnyMenu += ActivateCameraBlur;
		menuManager.OnCloseAnyMenu += DeactivateCameraBlur;
		menuManager.OnOpenPauseMenu += ActivateCameraBlur;
		menuManager.OnClosePauseMenuDuringOpenedDialogueMenu += DeactivateCameraBlur;
	}


	public void ActivateCameraBlur()
	{
		volumeMainCamera.enabled = true;
		volumeFirstPersonCamera.enabled = true;
		Debug.Log("Active CameraBlur");
	}
	public void DeactivateCameraBlur()
	{
		volumeMainCamera.enabled = false;
		volumeFirstPersonCamera.enabled = false;
		Debug.Log("Deactive CameraBlur");
	}
}
