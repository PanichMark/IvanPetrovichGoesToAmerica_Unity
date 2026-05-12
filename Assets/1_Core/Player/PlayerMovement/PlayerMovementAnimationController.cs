using UnityEngine;

public class PlayerMovementAnimationController: MonoBehaviour
{
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerMovementController _playerMovementController;
	private PlayerCameraController _playerCameraController;
	private string _currentPlayerMovementAnimation = "";
	private Animator _playerAnimator;
	private bool _isInitialized = false;

	public void Initialize(
		IInputDevice inputDevice,
		PlayerBehaviourController playerBehaviour,
		PlayerMovementController playerMovementController,
		PlayerCameraController playerCameraController,
		GameObject player)
	{
		_inputDevice = inputDevice;
		_playerAnimator = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
		_playerMovementController = playerMovementController;
		_playerCameraController = playerCameraController;
		
		ChangePlayerMovementAnimation("Idle");

		_isInitialized = true;
		Debug.Log("PlayerAnimationController Initialized");
	}

	private void Update()
	{
		if (!_isInitialized)
			return;

		if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerIdle")
		{
			
			ChangePlayerMovementAnimation("Idle");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerWalking")
		{
			if (_playerBehaviour.IsPlayerArmed == true || (_playerCameraController.CurrentPlayerCameraStateType == "FirstPerson"))
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
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerRunning")
		{

			ChangePlayerMovementAnimation("Running");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerJumping")
		{

			ChangePlayerMovementAnimation("Jumping");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerFalling")
		{

			ChangePlayerMovementAnimation("Falling");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle")
		{

			ChangePlayerMovementAnimation("CrouchingIdle");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{

			ChangePlayerMovementAnimation("CrouchingWalking");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerSliding")
		{

			ChangePlayerMovementAnimation("Sliding");
		}
		else if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
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