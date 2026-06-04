using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private CapsuleCollider _playerCollider;
	
	private PlayerMovementStateMachineController _playerMovementStateMachineController;


	public void Initialize(
		Bootstrap bootstrap,
		PlayerMovementStateMachineController movementController)
	{
		_bootstrap = bootstrap;
		_playerMovementStateMachineController = movementController; // Новый аргумент
		_playerCollider = GetComponent<CapsuleCollider>();
	
		Debug.Log("PlayerColliderController Initialized");
	}

	void Update()
    {
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle"
			|| _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"
			|| _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerSliding"
			|| _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerJumping")
		{
            transform.position = transform.parent.position+new Vector3(0f, 0.5f, 0f);
            transform.localScale = new Vector3(1f,  0.5f, 1f);
        }
		else 
		{
			transform.position = transform.parent.position+new Vector3(0f, 1f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
		{
			_playerCollider.enabled = false;
		}
		else 
		{
			_playerCollider.enabled = true;
		}
	}
}