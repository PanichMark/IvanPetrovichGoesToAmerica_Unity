using UnityEngine;
public class ThirdPersonPlayerCameraState : PlayerCameraState
{
	private IInputDevice inputDevice;
	public ThirdPersonPlayerCameraState(PlayerCameraController playerCam, IInputDevice inputDevice)
	{
		playerCamera = playerCam;
		this.inputDevice = inputDevice;
	
	}

	// Деструктор, который очищает экземпляр
	

	public override void Update()
	{
		this.playerCamera.ThirdPersonCameraTransform();

		if (inputDevice.GetKeyChangeCameraView())
		{
			playerCamera.SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
		}
		//playerCamera.ThirdPersonCameraTransform();
	}

}


	


