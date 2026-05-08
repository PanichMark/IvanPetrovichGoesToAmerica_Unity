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
		//	Debug.Log("POSITION "+ this.position);
	}


	public override void Update()
	{
		//playerCamera.CutsceneCameraTransform(new Vector3(0, 5, -7));
		//playerCamera.CutsceneCameraTransform(position);
	}


}


