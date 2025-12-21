using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraBlurFilter : MonoBehaviour
{
	private MenuManager menuManager;

	public void Initialize(MenuManager menuManager)
	{
		this.menuManager = menuManager;
	}

	public Volume volumeMainCamera;
	public Volume volumeFirstPersonCamera;

	private void Start()
	{
		volumeMainCamera = GetComponent<Volume>();
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

