public class PlayerMovementStateFalling : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;

	public PlayerMovementStateFalling(PlayerMovementStateMachineController playerMovementStateMachineController, PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;

		_playerMovementController.SetPlayerFloorDetectionRayCastLengthToDefault();
	}

	public override void Update()
	{
		if (_playerMovementController.IsPlayerFalling == false)
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStandingIdle);
		}
		if (_playerMovementController.IsPlayerFalling == false && _inputDevice.GetKeyRun())
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerRunning);
		}
		if (_playerMovementController.IsPlayerFalling == false)
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStandingWalking);
		}
	}
}