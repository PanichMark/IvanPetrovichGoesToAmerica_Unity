using UnityEngine;
public class FirstPersonPlayerCameraState : AbstractPlayerCameraState
{
	private PlayerMovementController movementController;
	private IInputDevice inputDevice;
	public FirstPersonPlayerCameraState(PlayerCameraController playerCam, PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		playerCamera = playerCam;
		movementController = playerMovementController;

		this .inputDevice = inputDevice;	
	}

	public override void Update()
	{
		playerCamera.RotateCamera();
		this.playerCamera.FirstPersonCameraTransform();

		

		// Проверка на специфичные состояния движения
		if (
			movementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			movementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking" ||
			movementController.CurrentPlayerMovementStateType == "PlayerSliding"
		)
		{
			playerCamera.CameraCrouching();
		}
		else playerCamera.CameraStanding();

		if (inputDevice.GetKeyChangeCameraView())
		{
			playerCamera.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		}

		//playerCamera.SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
	}

	
}


