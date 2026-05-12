using UnityEngine;
public class PlayerColliderController : MonoBehaviour
{
	private CapsuleCollider _playerCollider;
	
	private PlayerMovementController _playerMovementController;

	private bool _isInitialized = false;

	void Update()
    {
		if (!_isInitialized)
			return;
		if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle"
			|| _playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"
			|| _playerMovementController.CurrentPlayerMovementStateType == "PlayerSliding")
		{
            transform.position = transform.parent.position+new Vector3(0f, 0.5f, 0f);
            transform.localScale = new Vector3(1f,  0.5f, 1f);
        }
		else 
		{
			transform.position = transform.parent.position+new Vector3(0f, 1f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}

		if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
		{
			_playerCollider.enabled = false;
		}
		else 
		{
			_playerCollider.enabled = true;
		}
	}

	public void Initialize(PlayerMovementController movementController)
	{
		_playerMovementController = movementController; // Новый аргумент
		_playerCollider = GetComponent<CapsuleCollider>();
		_isInitialized = true;
		Debug.Log("PlayerCollider Initialized");
	}
}