using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraBlurFilter : MonoBehaviour
{
	private MenuManager _menuManager;
	private Volume _thirdPersonCameraVolume;
	private Volume _firstPersonCameraVolume;

	public void Initialize(MenuManager manager)
	{
		_menuManager = manager;

		_thirdPersonCameraVolume = GetComponent<Volume>();
		Transform firstPersonCameraTransform = transform.Find("FirstPerson Camera");
		_firstPersonCameraVolume = firstPersonCameraTransform.GetComponent<Volume>();
		Debug.Log("CameraBlurFilter initialized.");

		_menuManager.OnOpenAnyMenu += ActivateCameraBlur;
		_menuManager.OnCloseAnyMenu += DeactivateCameraBlur;
		_menuManager.OnOpenPauseMenu += ActivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedDialogueMenu += DeactivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedCutsceneMenu += DeactivateCameraBlur;
	}

	public void ActivateCameraBlur()
	{
		_thirdPersonCameraVolume.enabled = true;
		_firstPersonCameraVolume.enabled = true;
		Debug.Log("Active CameraBlur");
	}
	public void DeactivateCameraBlur()
	{
		_thirdPersonCameraVolume.enabled = false;
		_firstPersonCameraVolume.enabled = false;
		Debug.Log("Deactive CameraBlur");
	}
}