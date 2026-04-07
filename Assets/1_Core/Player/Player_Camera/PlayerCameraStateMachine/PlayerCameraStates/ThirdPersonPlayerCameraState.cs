using UnityEngine;
public class ThirdPersonPlayerCameraState : AbstractPlayerCameraState
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
		playerCamera.RotateCamera();
		this.playerCamera.ThirdPersonCameraTransform();
		
		if (inputDevice.GetKeyChangeCameraView())
		{
			playerCamera.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		}
		//playerCamera.ThirdPersonCameraTransform();
	}

}


	


