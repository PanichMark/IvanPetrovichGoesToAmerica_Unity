using UnityEngine;
public class FirstPersonPlayerCameraState : PlayerCameraState
{
	public static FirstPersonPlayerCameraState Instance { get; private set; }

	public FirstPersonPlayerCameraState(PlayerCameraController playerCam)
	{
		playerCamera = playerCam;
		Debug.Log("Entered 1st Person Camera");
		playerCamera.SetPlayerCameraType(PlayerCameraStateType.FirstPerson);

		// Установим экземпляр
		Instance = this;


	}

	// Деструктор, который очищает экземпляр
	~FirstPersonPlayerCameraState()
	{
		Instance = null;
	}

	public override void ChangePlayerCameraView()
	{
		playerCamera.SetPlayerCameraState(PlayerCameraStateType.ThirdPerson);
		//Instance = null;
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

