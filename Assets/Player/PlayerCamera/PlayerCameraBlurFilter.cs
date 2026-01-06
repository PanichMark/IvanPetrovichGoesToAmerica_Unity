using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraBlurFilter : MonoBehaviour
{
	private MenuManager menuManager;
	public Volume volumeMainCamera;
	public Volume volumeFirstPersonCamera;

	public void Initialize(MenuManager manager)
	{
		menuManager = manager;
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