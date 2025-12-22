using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private IInputDevice inputDevice;

	[SerializeField] private GameObject player;

	[SerializeField] private MenuManager menuManager;

	[SerializeField] private PlayerBehaviour playerBehaviour;

	[SerializeField] private PlayerMovementController movementController;
	[SerializeField] private PlayerCapluseCollider playerCollider;
	

	[SerializeField] private PlayerCameraController cameraController;
	[SerializeField] private PlayerCameraBlurFilter cameraBlurFilter;

	[SerializeField] private WeaponController weaponController;
	[SerializeField] private WeaponWheelController weaponWheelController;

	//[SerializeField] private WeaponWheelsButton buttonBaton;
	//[SerializeField] private WeaponWheelsButton buttonRevolver;
	//[SerializeField] private WeaponWheelsButton buttonCrossbow;
	//[SerializeField] private WeaponWheelsButton buttonEugenic;

	[SerializeField] private PlayerCameraFirstPersonRender firstPersonRender;

	[SerializeField] private PlayerAnimationController playerAnimationController;
	
	

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
        inputDevice = new InputKeyboard();

		menuManager.Initialize(inputDevice);

		playerBehaviour.Initialize(inputDevice);

		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);

		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, player);
		cameraBlurFilter.Initialize(menuManager);

		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController);
		//buttonBaton.Initialize(weaponController, weaponWheelController);
		//buttonRevolver.Initialize(weaponController, weaponWheelController);
		//buttonCrossbow.Initialize(weaponController, weaponWheelController);
		//buttonEugenic.Initialize(weaponController, weaponWheelController);

		firstPersonRender.Initialize(cameraController, weaponController);

		playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);

		








		Debug.Log("BootStrap ended!");
		Debug.Log("#########################");

	}
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
