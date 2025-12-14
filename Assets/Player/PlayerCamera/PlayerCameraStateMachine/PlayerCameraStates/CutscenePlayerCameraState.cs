using UnityEngine;

public class CutscenePlayerCameraState : PlayerCameraState
{
	public CutscenePlayerCameraState(PlayerCameraController playerCam)
	{
		playerCamera = playerCam;
		Debug.Log("Entered Cutscene Camera");
	}
	

	public override void PlayerCameraPosition()
	{
		playerCamera.CutsceneCameraTransform();
	}

	public override void ExitCutscene()
	{
		if (playerCamera.GetPreviousPlayerCameraType() == PlayerCameraStateType.FirstPerson.ToString())
		{
			playerCamera.SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
		}
		else if (playerCamera.GetPreviousPlayerCameraType() == PlayerCameraStateType.ThirdPerson.ToString())
		{
			playerCamera.SetPlayerCameraState(PlayerCameraStateType.ThirdPerson);
		}
	}
}

