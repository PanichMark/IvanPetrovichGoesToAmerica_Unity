using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private IInputDevice inputDevice;
    
	private PlayerCameraController cameraController;
	private PlayerMovementController movementController;
	private PlayerBehaviour playerBehaviour;
	

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
        inputDevice = new InputKeyboard();


		playerBehaviour = new PlayerBehaviour(inputDevice);


		movementController = new PlayerMovementController(inputDevice, playerBehaviour);


		cameraController = new PlayerCameraController(inputDevice);
	}
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
