using System;
using UnityEngine;
using System.Collections;
public class PlayerCameraController : MonoBehaviour, ISaveLoad
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PlayerMovementController movementController;
	private PlayerCapsuleCollider playerCollider;
	private GameObject player;
	// Конструктор принимает зависимость



	private PlayerCameraState playerCameraState;
	private PlayerCameraStateType playerCameraStateType;

	private Vector2 MouseRotation;
	private Vector2 MouseScrollWheel;

	//private Vector3 CameraForward;
	//private Vector3 CameraRight;

	private RaycastHit hit;

	public bool IsAbleToZoomCameraOut { get; private set; } = true;

	public float PlayerCameraDistanceX { get; private set; }
	public float PlayerCameraDistanceY { get; private set; }
	public float PlayerCameraDistanceZ { get; private set; }

	//public float CameraRotationY { get; private set; }
	private float MouseRotationLimit = 65f;

	public string CurrentPlayerCameraStateType { get; private set; } = "ThirdPerson";

	private string _currentPlayerCameraType;
	private string _previousPlayerCameraType;

	private bool IsCameraShoulderRight= true;

	private bool canReturn = false;     
	private float startTransitionTime; 
	public float transitionDelay { get; private set; } = 0.5f;


	private bool _isInitialized = false;


	void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
		{
			//Debug.Log("bruh");
			return;
			
		}

		

		//Debug.Log(CurrentPlayerCameraStateType);

	

		if (MouseScrollWheel.y < 0 && IsAbleToZoomCameraOut == true && CurrentPlayerCameraStateType != "FirstPerson")
		{
			if (PlayerCameraDistanceY > -1.99f)
			{
				PlayerCameraDistanceY -= 0.05f;
			}
			if (PlayerCameraDistanceZ < 4.99f)
			{
				PlayerCameraDistanceZ += 0.35f;
			}
		}
		if (MouseScrollWheel.y > 0 && CurrentPlayerCameraStateType != "FirstPerson")
		{
			if (PlayerCameraDistanceY < -1.51f)
			{
				PlayerCameraDistanceY += 0.05f;
			}
			if (PlayerCameraDistanceZ > 1.51f)
			{
				PlayerCameraDistanceZ -= 0.35f;
			}
		}

		
		playerCameraState.Update();
	

	

	
	
		




		if (inputDevice.GetKeyChangeCameraShoulder() && CurrentPlayerCameraStateType != "FirstPerson")
		{
			IsCameraShoulderRight = !IsCameraShoulderRight;
		}

		if (IsCameraShoulderRight == true)
		{
			PlayerCameraDistanceX = Mathf.Lerp(PlayerCameraDistanceX, -0.85f, Time.deltaTime * 4);
		}
		else
		{
			PlayerCameraDistanceX = Mathf.Lerp(PlayerCameraDistanceX, 0.85f, Time.deltaTime * 4);
		}

		if (playerCollider != null)
		{
			if (Physics.Linecast(playerCollider.transform.position, transform.position, out hit))
			{
				// Камера снова видит игрока
				if (!canReturn)
				{
					// Запускаем обратный отсчёт
					canReturn = true;
					startTransitionTime = Time.time;
				}
				else
				{
					// Проверяем, прошёл ли период ожидания
					if (Time.time - startTransitionTime >= transitionDelay)
					{
						if (PlayerCameraDistanceZ >= 0.75f)
						{
							// Потеря контакта с игроком, идём на минимальное расстояние
							PlayerCameraDistanceZ = Mathf.Lerp(PlayerCameraDistanceZ, hit.distance, Time.deltaTime * 4f);
							IsAbleToZoomCameraOut = false;
						}
						//else 
					}
				}
			}
			else
			{
				if (PlayerCameraDistanceZ <= 5f)
				{
					IsAbleToZoomCameraOut = true;
					// Начинаем постепенное удаление камеры
					//	PlayerCameraDistanceZ = Mathf.Lerp(PlayerCameraDistanceZ, 5f, Time.deltaTime * 4f);
				}

				canReturn = false; // Отменяем возвращение
			}
		}
		





		//CameraForward = transform.forward;
		//CameraRight = transform.right;

		
		
		//CameraRotationY = transform.eulerAngles.y;









	}

	// Корутина для плавного перемещения камеры



	private void FixedUpdate()
	{
		if (MouseRotation.y >= 360)
		{
			MouseRotation.y = 0;
		}
		if (MouseRotation.y <= -360)
			{
				MouseRotation.y = 0;
			}
	}

	public void SetPlayerCameraState(PlayerCameraStateType playerCameraStateType)
	{
		PlayerCameraState newState;

		if (playerCameraStateType == PlayerCameraStateType.FirstPerson)
		{
			CurrentPlayerCameraStateType = "FirstPerson";
			movementController.GiveCurrentPlayerCameraType("FirstPerson");
			newState = new FirstPersonPlayerCameraState(this, movementController, inputDevice);
			//IsPlayerCameraFirstPerson = true;
		}
		else if (playerCameraStateType == PlayerCameraStateType.ThirdPerson)
		{
			CurrentPlayerCameraStateType = "ThirdPerson";
			movementController.GiveCurrentPlayerCameraType("ThirdPerson");
			newState = new ThirdPersonPlayerCameraState(this, inputDevice);
			//IsPlayerCameraFirstPerson = false;
		}
		else if (playerCameraStateType == PlayerCameraStateType.Cutscene)
		{
			CurrentPlayerCameraStateType = "Cutscene";
			movementController.GiveCurrentPlayerCameraType("Cutscene");
			newState = new CutscenePlayerCameraState(this, new Vector3(0.2f, 1.35f, -0.9f), new Vector3(20, -12, 0));
			//IsPlayerCameraFirstPerson = false;
		}
		else
		{
			newState = null;
		}
		Debug.Log("CameraState: " + CurrentPlayerCameraStateType);
		playerCameraState = newState;
	}
	public void CameraStanding()
	{
		
		// Восстанавливаем оригинальную высоту камеры
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		

	}
	public void CameraCrouching()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
	}
	public void FirstPersonCameraTransform()
	{

			transform.position = player.transform.position + Quaternion.Euler(0, MouseRotation.y, 0) *
			new Vector3(0, movementController.PlayerCurrentHeight - 0.13f, 0.1f);
		
	}
	public void ThirdPersonCameraTransform()
	{
	
		transform.position = player.transform.position - Quaternion.Euler(-MouseRotation.x, MouseRotation.y, 0) *
		new Vector3(PlayerCameraDistanceX, PlayerCameraDistanceY, PlayerCameraDistanceZ);

	}
	public void CutsceneCameraTransformPosition(Vector3 position)
	{
		transform.position = position;
	}
	public void CutsceneCameraTransformRotation(Quaternion rotation)
	{
		transform.rotation = rotation;
	}

	public string GetCurrentPlayerCameraType()
	{
		return _currentPlayerCameraType.ToString();
	}
	public string GetPreviousPlayerCameraType()
	{
		return _previousPlayerCameraType.ToString();
	}

	public void SaveData(ref GameData data)
	{
		data.CurrentPlayerCameraStateType = this.CurrentPlayerCameraStateType;
		data.PlayerCameraDistanceY = this.PlayerCameraDistanceY;
		data.PlayerCameraDistanceZ = this.PlayerCameraDistanceZ;
		data.CameraRotation = new Quaternion(-this.MouseRotation.x, this.MouseRotation.y, 0, 0);
		data.IsCameraShoulderRight = this.IsCameraShoulderRight;
	}

	public void LoadData(GameData data)
	{

		this.CurrentPlayerCameraStateType = data.CurrentPlayerCameraStateType;
		this.PlayerCameraDistanceY = data.PlayerCameraDistanceY;
		this.PlayerCameraDistanceZ = data.PlayerCameraDistanceZ;
		this.MouseRotation.x = -data.CameraRotation.x;
		this.MouseRotation.y = data.CameraRotation.y;
		this.IsCameraShoulderRight = data.IsCameraShoulderRight;

		playerCameraStateType = (PlayerCameraStateType)Enum.Parse(typeof(PlayerCameraStateType), CurrentPlayerCameraStateType);
		SetPlayerCameraState(playerCameraStateType);

	}
	public void RotateCamera()
	{
		if (!menuManager.IsAnyMenuOpened)
		{
			MouseRotation.y += Input.GetAxis("Mouse X");
			MouseRotation.x += Input.GetAxis("Mouse Y");
			MouseRotation.x = Mathf.Clamp(MouseRotation.x, MouseRotationLimit * -1, MouseRotationLimit);
			MouseScrollWheel = Input.mouseScrollDelta;

			transform.rotation = Quaternion.Euler(-MouseRotation.x, MouseRotation.y, 0);
		}
	}
	public void Initialize(IInputDevice inputDevice, GameSceneManager gameSceneManager, MenuManager menuManager, PlayerMovementController movementController, PlayerCapsuleCollider playerCollider, GameObject playerModel)
	{
		this.gameSceneManager = gameSceneManager;
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.movementController = movementController; // Новый аргумент
		this.playerCollider = playerCollider;
		this.player = playerModel;


		PlayerCameraDistanceX = -0.85f;
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;


		//playerCameraStateType = (PlayerCameraStateType)Enum.Parse(typeof(PlayerCameraStateType), CurrentPlayerCameraStateType);
		this.gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerCameraState(PlayerCameraStateType.Cutscene);
		SetPlayerCameraState(PlayerCameraStateType.FirstPerson);
		_isInitialized = true;
		Debug.Log("CameraController Initialized");
	}
	private GameSceneManager gameSceneManager;
}


