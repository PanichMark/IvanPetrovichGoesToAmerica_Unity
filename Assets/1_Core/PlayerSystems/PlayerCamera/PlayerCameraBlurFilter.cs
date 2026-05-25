using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraBlurFilter : MonoBehaviour
{
	private MenuManager _menuManager;
	private Volume _thirdPersonCameraVolume;
	private Volume _firstPersonCameraVolume;
	private GameObject _playerCameraFirstPerson;

	public void Initialize(MenuManager manager, GameObject playerCameraFirstPerson)
	{
		_menuManager = manager;
		_playerCameraFirstPerson = playerCameraFirstPerson;

		_thirdPersonCameraVolume = GetComponent<Volume>();
		_firstPersonCameraVolume = _playerCameraFirstPerson.GetComponent<Volume>();
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