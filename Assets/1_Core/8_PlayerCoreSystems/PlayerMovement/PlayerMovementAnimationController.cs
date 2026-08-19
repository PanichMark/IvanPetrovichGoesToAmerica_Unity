using UnityEngine;

public class PlayerMovementAnimationController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private string _currentPlayerMovementAnimation;
	private Animator _playerAnimator;

	public void Initialize(
		IInputDevice inputDevice,
		PlayerBehaviourController playerBehaviour,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject player)
	{
		_inputDevice = inputDevice;
		_playerBehaviour = playerBehaviour;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_playerAnimator = player.GetComponent<Animator>();

		_playerMovementStateMachineController.OnChangeMovementState += HandleMovementStateChanged;

		ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Standing_Type1.ToString());
	}

	private void HandleMovementStateChanged(PlayerMovementStateTypes newStateType)
	{
		if (newStateType == PlayerMovementStateTypes.PlayerIdleStanding)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Standing_Type1.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerWalkingStanding)
		{
			if (_playerBehaviour.IsPlayerArmed || _playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
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
			else
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingForward.ToString());
			}
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerRunning)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_RunningForward.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerJumping)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Jumping.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerFalling)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Falling.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerIdleCrouhcing)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidIdleEnum.Idle_Crouching_Type1.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerWalkingCrouching)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Crouching.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerSliding)
		{
			ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Sliding.ToString());
		}
		else if (newStateType == PlayerMovementStateTypes.PlayerLedgeClimbingStanding)
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