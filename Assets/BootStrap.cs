using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private IInputDevice inputDevice;

	[SerializeField] private GameObject playerModel;
	[SerializeField] private PlayerMovementController movementController;
	[SerializeField] private PlayerCapluseCollider playerCollider;

	[SerializeField] private PlayerCameraController cameraController;
	
	private PlayerBehaviour playerBehaviour;
	
	

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
        inputDevice = new InputKeyboard();


		playerBehaviour = new PlayerBehaviour(inputDevice);

		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);


		cameraController.Initialize(inputDevice, movementController, playerCollider, playerModel);
	}
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
