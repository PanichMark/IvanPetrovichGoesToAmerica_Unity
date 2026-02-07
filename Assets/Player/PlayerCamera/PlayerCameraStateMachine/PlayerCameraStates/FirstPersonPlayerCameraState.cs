using UnityEngine;
public class FirstPersonPlayerCameraState : PlayerCameraState
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
			playerCamera.SetPlayerCameraState(PlayerCameraStateType.ThirdPerson);
		}

		//playerCamera.SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
	}

	
}


