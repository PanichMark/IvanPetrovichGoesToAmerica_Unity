using UnityEngine;
public class FirstPersonPlayerCameraState : PlayerCameraState
{
	public FirstPersonPlayerCameraState(PlayerCameraController playerCam)
	{
		playerCamera = playerCam;
		Debug.Log("Entered 1st Person Camera");
		playerCamera.SetPlayerCameraType(PlayerCameraStateType.FirstPerson);
	}
	public override void ChangePlayerCameraView()
	{
		playerCamera.SetPlayerCameraState(PlayerCameraStateType.ThirdPerson);
	}
	public override void PlayerCameraPosition()
	{
		playerCamera.FirstPersonCameraTransform();
	}

	public override void EnterCutscene()
	{
		playerCamera.SetPlayerCameraState(PlayerCameraStateType.Cutscene);
		playerCamera.SetPlayerCameraType(PlayerCameraStateType.Cutscene);
	}
}

