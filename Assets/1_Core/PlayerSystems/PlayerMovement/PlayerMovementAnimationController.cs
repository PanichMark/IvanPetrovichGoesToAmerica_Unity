using UnityEngine;

public class PlayerMovementAnimationController: MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private string _currentPlayerMovementAnimation = "";
	private Animator _playerAnimator;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		PlayerBehaviourController playerBehaviour,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject player)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_playerAnimator = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		
		ChangePlayerMovementAnimation("Idle");

		Debug.Log("PlayerMovementAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerIdle")
		{
			
			ChangePlayerMovementAnimation("Idle");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerWalking")
		{
			if (_playerBehaviour.IsPlayerArmed == true || (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "FirstPerson"))
			{
				if (_inputDevice.GetKeyUp())
				{
					ChangePlayerMovementAnimation("Walking Forward");
				}
				else if (_inputDevice.GetKeyDown())
				{
					ChangePlayerMovementAnimation("Walking Backwards");
				}
				if (_inputDevice.GetKeyRight())
				{
					ChangePlayerMovementAnimation("Walking Right");
				}
				else if (_inputDevice.GetKeyLeft())
				{
					ChangePlayerMovementAnimation("Walking Left");
				}
			}
			else ChangePlayerMovementAnimation("Walking Forward");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerRunning")
		{

			ChangePlayerMovementAnimation("Running");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerJumping")
		{

			ChangePlayerMovementAnimation("Jumping");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerFalling")
		{

			ChangePlayerMovementAnimation("Falling");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle")
		{

			ChangePlayerMovementAnimation("CrouchingIdle");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{

			ChangePlayerMovementAnimation("CrouchingWalking");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerSliding")
		{

			ChangePlayerMovementAnimation("Sliding");
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
		{
			ChangePlayerMovementAnimation("Ledge Climbing");
		}
	}

	private void ChangePlayerMovementAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerMovementAnimation != animation)
		{
			_currentPlayerMovementAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}
}