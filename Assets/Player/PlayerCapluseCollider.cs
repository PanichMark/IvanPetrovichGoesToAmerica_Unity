using UnityEngine;
public class PlayerCapluseCollider : MonoBehaviour
{
	

	
	private PlayerMovementController movementController;
	// Конструктор принимает зависимость
	

	
  
    void Update()
    {
		if (movementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle"
			|| movementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"
			|| movementController.CurrentPlayerMovementStateType == "PlayerSliding")
		{
            transform.position = transform.parent.position+new Vector3(0f, 0.5f, 0f); ;
            transform.localScale = new Vector3(1f,  0.5f, 1f);
        }
		else 
		{
			transform.position = transform.parent.position+new Vector3(0f, 1f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public void Initialize(PlayerMovementController movementController)
	{
		this.movementController = movementController; // Новый аргумент
		Debug.Log("PlayerCollider Initialized");
	}
}

