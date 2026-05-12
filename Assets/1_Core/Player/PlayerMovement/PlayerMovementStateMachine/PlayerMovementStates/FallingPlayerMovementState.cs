public class FallingPlayerMovementState : AbstractPlayerMovementState
{
	private IInputDevice _inputDevice;

	public FallingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
	}

	public override void Update()
	{
		if (_playerMovementController.IsPlayerFalling == false)
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}
		if (_playerMovementController.IsPlayerFalling == false && _inputDevice.GetKeyRun())
		{
		_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerRunning);
		}
		if (_playerMovementController.IsPlayerFalling == false)
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerWalking);
		}
	}
}