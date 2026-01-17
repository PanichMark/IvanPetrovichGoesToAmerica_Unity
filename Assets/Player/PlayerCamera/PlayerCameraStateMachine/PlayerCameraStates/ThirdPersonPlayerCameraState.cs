using UnityEngine;
public class ThirdPersonPlayerCameraState : PlayerCameraState
{
	public static ThirdPersonPlayerCameraState Instance { get; private set; }
	public ThirdPersonPlayerCameraState(PlayerCameraController playerCam)
	{
		playerCamera = playerCam;
		Debug.Log("Entered 3rd Person Camera");
		playerCamera.SetPlayerCameraType(PlayerCameraStateType.ThirdPerson);

		// Установим экземпляр
		Instance = this;
	}

	// Деструктор, который очищает экземпляр
	~ThirdPersonPlayerCameraState()
	{
		Instance = null;
	}

	public override void ChangePlayerCameraView()
	{
		playerCamera.SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
		//Instance = null;
	}
	public override void PlayerCameraPosition()
	{
		playerCamera.ThirdPersonCameraTransform();
	}




	public override void EnterCutscene()
	{
		playerCamera.SetPlayerCameraState(PlayerCameraStateType.Cutscene);
		playerCamera.SetPlayerCameraType(PlayerCameraStateType.Cutscene);
	}
}


