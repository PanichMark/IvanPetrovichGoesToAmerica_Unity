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

		ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_IdleStanding_Type1.ToString());

		Debug.Log("PlayerMovementAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerIdle")
		{
			ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_IdleStanding_Type1.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerWalking")
		{
			if (_playerBehaviour.IsPlayerArmed == true || (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "FirstPerson"))
			{
				if (_inputDevice.GetKeyUp())
				{
					ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_Movement_WalkingForward.ToString());
				}
				else if (_inputDevice.GetKeyDown())
				{
					ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_Movement_WalkingBackward.ToString());
				}
				if (_inputDevice.GetKeyRight())
				{
					ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_Movement_WalkingRight.ToString());
				}
				else if (_inputDevice.GetKeyLeft())
				{
					ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_Movement_WalkingLeft.ToString());
				}
			}
			else ChangePlayerMovementAnimation(PlayerMovementAnimationsEnum.Animation_Humanoid_Movement_WalkingForward.ToString());
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