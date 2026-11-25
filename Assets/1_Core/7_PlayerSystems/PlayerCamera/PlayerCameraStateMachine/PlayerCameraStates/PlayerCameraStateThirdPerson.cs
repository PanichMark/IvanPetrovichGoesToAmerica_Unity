public class PlayerCameraStateThirdPerson : PlayerCameraStateAbstract
{
	private IInputDevice _inputDevice;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	public PlayerCameraStateThirdPerson(PlayerCameraController playerCam, PlayerCameraStateMachineController playerCameraStateMachineController, IInputDevice inputDevice)
	{
		_playerCamera = playerCam;
		_inputDevice = inputDevice;
		_playerCameraStateMachineController = playerCameraStateMachineController;
	}

	public override void Update()
	{
		_playerCamera.RotateCamera();
		_playerCamera.ThirdPersonCameraTransform();
		
		if (_inputDevice.GetKeyChangeCameraView())
		{
			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		}
	}
}