using UnityEngine;
public class PlayerCapsuleCollider : MonoBehaviour
{

	private CapsuleCollider CapsuleCollider;
	
	private PlayerMovementController movementController;
	// Конструктор принимает зависимость



	private bool _isInitialized = false;
	void Update()
    {
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
		if (movementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle"
			|| movementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"
			|| movementController.CurrentPlayerMovementStateType == "PlayerSliding")
		{
            transform.position = transform.parent.position+new Vector3(0f, 0.5f, 0f);
            transform.localScale = new Vector3(1f,  0.5f, 1f);
			
        }
		else 
		{
			transform.position = transform.parent.position+new Vector3(0f, 1f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}



		if (movementController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
		{
			CapsuleCollider.enabled = false;
		}
		else 
		{
			CapsuleCollider.enabled = true;
		}
	}

	public void Initialize(PlayerMovementController movementController)
	{
		this.movementController = movementController; // Новый аргумент
		CapsuleCollider = GetComponent<CapsuleCollider>();
		_isInitialized = true;
		Debug.Log("PlayerCollider Initialized");
	}
}


