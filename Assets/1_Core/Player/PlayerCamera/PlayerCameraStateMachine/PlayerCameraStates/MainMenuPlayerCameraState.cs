using UnityEngine;

public class MainMenuPlayerCameraState : AbstractPlayerCameraState
{
	private Vector3 position;
	private Vector3 eulerAngles;

	public MainMenuPlayerCameraState(PlayerCameraController playerCam, Vector3 position, Vector3 eulerAngles)
	{
		playerCamera = playerCam;
		this.position = position;
		this.eulerAngles = eulerAngles;
		playerCamera.SetCameraMainMenuPosition(this.position);
		var quaternionRotation = Quaternion.Euler(eulerAngles);
		playerCamera.SetCameraMainMenuRotation(quaternionRotation);
	}

	public override void Update()
	{

	}
}