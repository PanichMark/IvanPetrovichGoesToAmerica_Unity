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
		_isInitialized = true;
		Debug.Log("CameraBlurFilter initialized.");
	}
	private bool _isInitialized = false;
	private void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
		if (menuManager.IsAnyMenuOpened)
		{
			volumeMainCamera.enabled = true;
			volumeFirstPersonCamera.enabled = true;
		}
		else
		{
			volumeMainCamera.enabled = false;
			volumeFirstPersonCamera.enabled = false;
		}
	}
}
