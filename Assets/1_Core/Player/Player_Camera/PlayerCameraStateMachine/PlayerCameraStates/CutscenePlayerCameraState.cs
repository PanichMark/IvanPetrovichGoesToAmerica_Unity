using UnityEngine;

public class CutscenePlayerCameraState : AbstractPlayerCameraState


{
	private Vector3 position;
	private Vector3 eulerAngles;

	public CutscenePlayerCameraState(PlayerCameraController playerCam, Vector3 position, Vector3 eulerAngles)
	{
		playerCamera = playerCam;
		this.position = position;
		this.eulerAngles = eulerAngles;
		playerCamera.CutsceneCameraTransformPosition(this.position);
		var quaternionRotation = Quaternion.Euler(eulerAngles);
		playerCamera.CutsceneCameraTransformRotation(quaternionRotation);
		//	Debug.Log("POSITION "+ this.position);
	}

	
	public override void Update()
	{
		//playerCamera.CutsceneCameraTransform(new Vector3(0, 5, -7));
		//playerCamera.CutsceneCameraTransform(position);
	}
	

}


