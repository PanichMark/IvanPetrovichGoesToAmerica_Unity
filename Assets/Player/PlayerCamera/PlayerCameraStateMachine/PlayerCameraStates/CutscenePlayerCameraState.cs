using UnityEngine;

public class CutscenePlayerCameraState : PlayerCameraState


{
	private Vector3 position;

	public CutscenePlayerCameraState(PlayerCameraController playerCam, Vector3 position)
	{
		playerCamera = playerCam;
		this.position = position;
		playerCamera.CutsceneCameraTransform(this.position);
	//	Debug.Log("POSITION "+ this.position);
	}

	
	public override void Update()
	{
		//playerCamera.CutsceneCameraTransform(new Vector3(0, 5, -7));
		//playerCamera.CutsceneCameraTransform(position);
	}
	

}


