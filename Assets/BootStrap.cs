using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private IInputDevice inputDevice;

	[SerializeField] private GameObject playerModel;

	[SerializeField] private MenuManager menuManager;

	[SerializeField] private PlayerBehaviour playerBehaviour;

	[SerializeField] private PlayerMovementController movementController;
	[SerializeField] private PlayerCapluseCollider playerCollider;
	

	[SerializeField] private PlayerCameraController cameraController;
	[SerializeField] private PlayerCameraBlurFilter cameraBlurFilter;

	[SerializeField] private WeaponController weaponController;
	[SerializeField] private WeaponWheelController weaponWheelController;
	
	

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
        inputDevice = new InputKeyboard();

		menuManager.Initialize(inputDevice);

		playerBehaviour.Initialize(inputDevice);

		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);

		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, playerModel);
		cameraBlurFilter.Initialize(menuManager);

		weaponController.Initialize(inputDevice, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController);

	}
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
