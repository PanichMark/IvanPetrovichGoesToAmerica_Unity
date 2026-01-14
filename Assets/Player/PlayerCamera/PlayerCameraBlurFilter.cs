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

		Debug.Log("PlayerCameraBlurFilter initialized.");
	}

	private void Update()
	{
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