public class PlayerCameraStateFirstPerson : PlayerCameraStateAbstract
{
	private PlayerMovementController _movementController;
	private IInputDevice _inputDevice;
	public PlayerCameraStateFirstPerson(PlayerCameraController playerCam, PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerCamera = playerCam;
		_movementController = playerMovementController;

		_inputDevice = inputDevice;	
	}

	public override void Update()
	{
		_playerCamera.RotateCamera();
		_playerCamera.FirstPersonCameraTransform();

		if (
			_movementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			_movementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking" ||
			_movementController.CurrentPlayerMovementStateType == "PlayerSliding"
		)
		{
			_playerCamera.CameraCrouching();
		}
		else _playerCamera.CameraStanding();

		if (_inputDevice.GetKeyChangeCameraView())
		{
			_playerCamera.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		}
	}
}