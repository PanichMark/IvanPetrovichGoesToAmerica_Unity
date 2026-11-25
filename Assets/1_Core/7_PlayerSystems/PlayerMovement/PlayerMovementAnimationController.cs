using UnityEngine;

public class PlayerMovementAnimationController : MonoBehaviour
{
	private GameController _gameController;
	private IInputDevice _inputDevice;
	
	private PlayerBehaviourController _playerBehaviour;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private string _currentPlayerMovementAnimation;
	private Animator _playerAnimator;
	private PlayerMovementController _playerMovementController;

	public void Initialize(
		GameController gameController,
		IInputDevice inputDevice,
		PlayerBehaviourController playerBehaviour,
		PlayerMovementController playerMovementController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject player)
	{
		_gameController = gameController;
		_inputDevice = inputDevice;
		_playerBehaviour = playerBehaviour;
		_playerMovementController = playerMovementController;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_playerAnimator = player.GetComponent<Animator>();

		_playerMovementStateMachineController.OnChangeMovementState += HandleMovementStateChanged;

		ChangePlayerMovementAnimation(AnimationsHumanoidStationaryActionsEnum.StationaryAction_IdleStanding.ToString());

		_playerMovementController.OnChangePlayerMovementSpeedChangedByPickable += ChangeMovementAnimationsSpeed;

		_playerMovementController.OnMovementSpeedChangedByStateMachine += () =>
		{
			ChangeMovementAnimationsSpeed(1);
		};
	}

	private void HandleMovementStateChanged(PlayerMovementStateTypes newStateType)
	{
		if (!_gameController.IsPlayerDead)
		{
			if (newStateType == PlayerMovementStateTypes.PlayerIdleStanding)
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidStationaryActionsEnum.StationaryAction_IdleStanding.ToString());
			}
			else if (newStateType == PlayerMovementStateTypes.PlayerWalkingStanding)
			{
				if (_playerBehaviour.IsPlayerArmed || _playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
				{
					if (_inputDevice.GetKeyUp())
					{
						ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingStandingForward.ToString());
					}
					else if (_inputDevice.GetKeyDown())
					{
						ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingStandingBackward.ToString());
					}

					if (_inputDevice.GetKeyRight())
					{
						ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingStandingRight.ToString());
					}
					else if (_inputDevice.GetKeyLeft())
					{
						ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingStandingLeft.ToString());
					}
				}
				else
				{
					ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingStandingForward.ToString());
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
				ChangePlayerMovementAnimation(AnimationsHumanoidStationaryActionsEnum.StationaryAction_IdleCrouching.ToString());
			}
			else if (newStateType == PlayerMovementStateTypes.PlayerWalkingCrouching)
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_WalkingCrouchingForward.ToString());
			}
			else if (newStateType == PlayerMovementStateTypes.PlayerSliding)
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_Sliding.ToString());
			}
			else if (newStateType == PlayerMovementStateTypes.PlayerLedgeClimbingStanding)
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidMovementEnum.Movement_LedgeClimbingStanding.ToString());
			}
			else if (newStateType == PlayerMovementStateTypes.PlayerStranglingNPC)
			{
				ChangePlayerMovementAnimation(AnimationsHumanoidStationaryActionsEnum.StationaryAction_IdleStanding.ToString());
			}
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

	private void ChangeMovementAnimationsSpeed(float speed)
	{
		_playerAnimator.SetFloat("Speed", speed);
	}

	public void PlayerDeathAnimation()
	{
		ChangePlayerMovementAnimation(AnimationsHumanoidStationaryActionsEnum.StationaryAction_Dying.ToString());
	}
}