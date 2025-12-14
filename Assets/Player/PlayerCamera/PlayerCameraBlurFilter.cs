using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraBlurFilter : MonoBehaviour
{
	public Volume volumeMainCamera;
	public Volume volumeFirstPersonCamera;

	private void Start()
	{
		volumeMainCamera = GetComponent<Volume>();
	}

	private void Update()
	{
		if (MenuManager.IsAnyMenuOpened)
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

