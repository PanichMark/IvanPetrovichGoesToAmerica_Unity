using UnityEngine;

public class CutscenePlayerCameraState : PlayerCameraState
{
	public CutscenePlayerCameraState(PlayerCameraController playerCam)
	{
		playerCamera = playerCam;
		Debug.Log("Entered Cutscene Camera");
	}
	

	public override void Update()
	{
		playerCamera.CutsceneCameraTransform();
	}

	
}


