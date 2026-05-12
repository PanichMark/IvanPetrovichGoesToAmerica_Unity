public class ThirdPersonPlayerCameraState : AbstractPlayerCameraState
{
	private IInputDevice _inputDevice;
	public ThirdPersonPlayerCameraState(PlayerCameraController playerCam, IInputDevice inputDevice)
	{
		_playerCamera = playerCam;
		_inputDevice = inputDevice;
	}

	public override void Update()
	{
		_playerCamera.RotateCamera();
		_playerCamera.ThirdPersonCameraTransform();
		
		if (_inputDevice.GetKeyChangeCameraView())
		{
			_playerCamera.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		}
	}
}