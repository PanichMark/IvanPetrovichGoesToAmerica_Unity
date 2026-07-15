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

		ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Standing_Type1.ToString());

		Debug.Log("PlayerMovementAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Standing_Type1.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
		{
			if (_playerBehaviour.IsPlayerArmed == true || (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson))
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
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_RunningForward.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Jumping.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Falling.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Crouching_Type1.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Crouching.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerSliding)
		{

			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Sliding.ToString());
		}
		else if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbing)
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