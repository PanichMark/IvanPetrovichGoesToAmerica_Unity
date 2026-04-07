using System.Collections;
using UnityEngine;

public class PlayerAnimationController: MonoBehaviour
{
	private IInputDevice inputDevice;
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController playerMovementController;
	private PlayerCameraController playerCameraController;


	private Camera playerCameraObject;
	private Animator playerAnimator;
	private bool _isInitialized = false;

	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice, GameObject player, PlayerBehaviour playerBehaviour, PlayerMovementController playerMovementController,
		PlayerCameraController playerCameraController)
	{
		this.inputDevice = inputDevice;
		playerAnimator = player.GetComponent<Animator>();
		this.playerBehaviour = playerBehaviour;
		this.playerMovementController = playerMovementController;
		this.playerCameraController = playerCameraController;
	
		
	
		playerCameraObject = Camera.main;
		
		ChangePlayerMovementAnimation("Idle");


		_isInitialized = true;
		Debug.Log("PlayerAnimationController Initialized");
	}


	

	
	

	
	private string currentPlayerMovementAnimation = "";


	

	




	private void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
	
		
	




			// анимации PlayerMovement state машины
		if (playerMovementController.CurrentPlayerMovementStateType == "PlayerIdle")
		{
			
			ChangePlayerMovementAnimation("Idle");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerWalking")
		{
			if (playerBehaviour.IsPlayerArmed == true || (playerCameraController.CurrentPlayerCameraStateType == "FirstPerson"))
			{
				if (inputDevice.GetKeyUp())
				{
					ChangePlayerMovementAnimation("Walking Forward");
				}
				else if (inputDevice.GetKeyDown())
				{
					ChangePlayerMovementAnimation("Walking Backwards");
				}
				if (inputDevice.GetKeyRight())
				{
					ChangePlayerMovementAnimation("Walking Right");
				}
				else if (inputDevice.GetKeyLeft())
				{
					ChangePlayerMovementAnimation("Walking Left");
				}
			}
			else ChangePlayerMovementAnimation("Walking Forward");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerRunning")
		{

			ChangePlayerMovementAnimation("Running");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerJumping")
		{

			ChangePlayerMovementAnimation("Jumping");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerFalling")
		{

			ChangePlayerMovementAnimation("Falling");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle")
		{

			ChangePlayerMovementAnimation("CrouchingIdle");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{

			ChangePlayerMovementAnimation("CrouchingWalking");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerSliding")
		{

			ChangePlayerMovementAnimation("Sliding");
		}
		else if (playerMovementController.CurrentPlayerMovementStateType == "PlayerLedgeClimbing")
		{
			ChangePlayerMovementAnimation("Ledge Climbing");
		}



	}




	private void ChangePlayerMovementAnimation(string animation, float crossfade = 0.2f)
	{
		if (currentPlayerMovementAnimation != animation)
		{
			currentPlayerMovementAnimation = animation;
			playerAnimator.CrossFade(animation, crossfade);
		}
	}

}



