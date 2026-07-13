using UnityEngine;

public class PlayerMovementAnimationController: MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private string _currentPlayerMovementAnimation;
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

		ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Idle_Standing_Type1.ToString());

		Debug.Log("PlayerMovementAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdle.ToString())
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Idle_Standing_Type1.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalking.ToString())
		{
			if (_playerBehaviour.IsPlayerArmed == true || (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString()))
			{
				if (_inputDevice.GetKeyUp())
				{
					ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingForward.ToString());
				}
				else if (_inputDevice.GetKeyDown())
				{
					ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingBackward.ToString());
				}
				if (_inputDevice.GetKeyRight())
				{
					ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingRight.ToString());
				}
				else if (_inputDevice.GetKeyLeft())
				{
					ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingLeft.ToString());
				}
			}
			else ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingForward.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerRunning.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_RunningForward.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerJumping.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Jumping.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerFalling.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Falling.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Idle_Crouching_Type1.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Crouching.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerSliding.ToString())
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Sliding.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbing.ToString())
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_LedgeClimbing.ToString());
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