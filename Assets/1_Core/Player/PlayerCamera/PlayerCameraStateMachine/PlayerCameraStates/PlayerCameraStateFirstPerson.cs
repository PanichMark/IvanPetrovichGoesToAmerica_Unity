public class PlayerCameraStateFirstPerson : PlayerCameraStateAbstract
{
	private PlayerMovementController _movementController;
	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	public PlayerCameraStateFirstPerson(PlayerCameraController playerCam, PlayerCameraStateMachineController playerCameraStateMachineController, PlayerMovementController playerMovementController, PlayerMovementStateMachineController playerMovementStateMachineController, IInputDevice inputDevice)
	{
		_playerCamera = playerCam;
		_movementController = playerMovementController;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_inputDevice = inputDevice;	
	}

	public override void Update()
	{
		_playerCamera.RotateCamera();
		_playerCamera.FirstPersonCameraTransform();

		if (
			_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking" ||
			_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerSliding"
		)
		{
			_playerCamera.CameraCrouching();
		}
		else _playerCamera.CameraStanding();

		if (_inputDevice.GetKeyChangeCameraView())
		{
			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		}
	}
}