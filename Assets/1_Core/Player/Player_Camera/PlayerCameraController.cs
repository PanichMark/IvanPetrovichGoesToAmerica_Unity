using System;
using UnityEngine;
using System.Collections;
public class PlayerCameraController : MonoBehaviour, ISaveLoad
{
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerMovementController _movementController;
	private PlayerCapsuleCollider _playerCollider;
	private GameObject _player;

	public delegate void CameraStateHandler();
	public event CameraStateHandler OnFirstPersonCameraState;
	public event CameraStateHandler OnThirdPersonCameraState;

	private AbstractPlayerCameraState _playerCameraState;
	private PlayerCameraStateTypes _playerCameraStateType;

	private Vector2 _MouseRotation;
	private Vector2 _MouseScrollWheel;
	private GameSceneManager _gameSceneManager;
	private RaycastHit _hit;

	public bool IsAbleToZoomCameraOut { get; private set; } = true;

	public float PlayerCameraDistanceX { get; private set; }
	public float PlayerCameraDistanceY { get; private set; }
	public float PlayerCameraDistanceZ { get; private set; }

	private float _MouseRotationLimit = 65f;

	public string CurrentPlayerCameraStateType { get; private set; } = "ThirdPerson";

	private string _currentPlayerCameraType;
	private string _previousPlayerCameraType;

	private bool _isCameraShoulderRight= true;

	private bool _canReturn = false;     
	private float _startTransitionTime; 
	public float TransitionDelay { get; private set; } = 0.5f;

	private bool _isInitialized = false;

	void Update()
	{
		if (!_isInitialized)
		{
			return;
			
		}

		if (_MouseScrollWheel.y < 0 && IsAbleToZoomCameraOut == true && CurrentPlayerCameraStateType != "FirstPerson")
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
		if (_MouseScrollWheel.y > 0 && CurrentPlayerCameraStateType != "FirstPerson")
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
		
		_playerCameraState.Update();

		if (_inputDevice.GetKeyChangeCameraShoulder() && CurrentPlayerCameraStateType != "FirstPerson")
		{
			_isCameraShoulderRight = !_isCameraShoulderRight;
		}

		if (_isCameraShoulderRight == true)
		{
			PlayerCameraDistanceX = Mathf.Lerp(PlayerCameraDistanceX, -0.85f, Time.deltaTime * 4);
		}
		else
		{
			PlayerCameraDistanceX = Mathf.Lerp(PlayerCameraDistanceX, 0.85f, Time.deltaTime * 4);
		}

		if (_playerCollider != null)
		{
			if (Physics.Linecast(_playerCollider.transform.position, transform.position, out _hit))
			{

				if (!_canReturn)
				{
					_canReturn = true;
					_startTransitionTime = Time.time;
				}
				else
				{
					if (Time.time - _startTransitionTime >= TransitionDelay)
					{
						if (PlayerCameraDistanceZ >= 0.75f)
						{
							PlayerCameraDistanceZ = Mathf.Lerp(PlayerCameraDistanceZ, _hit.distance, Time.deltaTime * 4f);
							IsAbleToZoomCameraOut = false;
						}
					}
				}
			}
			else
			{
				if (PlayerCameraDistanceZ <= 5f)
				{
					IsAbleToZoomCameraOut = true;
				}

				_canReturn = false; 
			}
		}
	}

	private void FixedUpdate()
	{
		if (_MouseRotation.y >= 360)
		{
			_MouseRotation.y = 0;
		}
		if (_MouseRotation.y <= -360)
		{
			_MouseRotation.y = 0;
		}
	}

	public void SetPlayerCameraState(PlayerCameraStateTypes playerCameraStateType)
	{
		AbstractPlayerCameraState newState;

		if (playerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			CurrentPlayerCameraStateType = "FirstPerson";
			_movementController.GiveCurrentPlayerCameraType("FirstPerson");
			newState = new FirstPersonPlayerCameraState(this, _movementController, _inputDevice);
			OnFirstPersonCameraState?.Invoke();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			CurrentPlayerCameraStateType = "ThirdPerson";
			_movementController.GiveCurrentPlayerCameraType("ThirdPerson");
			newState = new ThirdPersonPlayerCameraState(this, _inputDevice);
			OnThirdPersonCameraState?.Invoke();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.Cutscene)
		{
			CurrentPlayerCameraStateType = "Cutscene";
			_movementController.GiveCurrentPlayerCameraType("Cutscene");
			newState = new CutscenePlayerCameraState();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.MainMenu)
		{
			CurrentPlayerCameraStateType = "MainMenu";
			_movementController.GiveCurrentPlayerCameraType("MainMenu");
			newState = new MainMenuPlayerCameraState(this, new Vector3(0.2f, 1.35f, -0.9f), new Vector3(20, -12, 0));
		}
		else
		{
			newState = null;
		}
		Debug.Log("CameraState: " + CurrentPlayerCameraStateType);
		_playerCameraState = newState;
	}

	public void CameraStanding()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}

	public void CameraCrouching()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
	}

	public void FirstPersonCameraTransform()
	{
		transform.position = _player.transform.position + Quaternion.Euler(0, _MouseRotation.y, 0) *
		new Vector3(0, _movementController.PlayerCurrentHeight - 0.13f, 0.1f);
	}

	public void ThirdPersonCameraTransform()
	{
		transform.position = _player.transform.position - Quaternion.Euler(-_MouseRotation.x, _MouseRotation.y, 0) *
		new Vector3(PlayerCameraDistanceX, PlayerCameraDistanceY, PlayerCameraDistanceZ);
	}

	public void SetCameraMainMenuPosition(Vector3 position)
	{
		transform.position = position;
	}

	public void SetCameraMainMenuRotation(Quaternion rotation)
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
		data.CurrentPlayerCameraStateType = CurrentPlayerCameraStateType;
		data.PlayerCameraDistanceY = PlayerCameraDistanceY;
		data.PlayerCameraDistanceZ = PlayerCameraDistanceZ;
		data.CameraRotation = new Quaternion(-_MouseRotation.x, _MouseRotation.y, 0, 0);
		data.IsCameraShoulderRight = _isCameraShoulderRight;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerCameraStateType = data.CurrentPlayerCameraStateType;
		PlayerCameraDistanceY = data.PlayerCameraDistanceY;
		PlayerCameraDistanceZ = data.PlayerCameraDistanceZ;
		_MouseRotation.x = -data.CameraRotation.x;
		_MouseRotation.y = data.CameraRotation.y;
		_isCameraShoulderRight = data.IsCameraShoulderRight;

		_playerCameraStateType = (PlayerCameraStateTypes)Enum.Parse(typeof(PlayerCameraStateTypes), CurrentPlayerCameraStateType);
		SetPlayerCameraState(_playerCameraStateType);
	}

	public void RotateCamera()
	{
		if (!_menuManager.IsAnyMenuOpened)
		{
			_MouseRotation.y += Input.GetAxis("Mouse X");
			_MouseRotation.x += Input.GetAxis("Mouse Y");
			_MouseRotation.x = Mathf.Clamp(_MouseRotation.x, _MouseRotationLimit * -1, _MouseRotationLimit);
			_MouseScrollWheel = Input.mouseScrollDelta;

			transform.rotation = Quaternion.Euler(-_MouseRotation.x, _MouseRotation.y, 0);
		}
	}

	public void Initialize(IInputDevice inputDevice, GameSceneManager gameSceneManager, MenuManager menuManager, PlayerMovementController movementController, PlayerCapsuleCollider playerCollider, GameObject playerModel)
	{
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_movementController = movementController; 
		_playerCollider = playerCollider;
		_player = playerModel;

		PlayerCameraDistanceX = -0.85f;
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;

		_gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerCameraState(PlayerCameraStateTypes.MainMenu);
		SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		_isInitialized = true;

		Debug.Log("CameraController Initialized");
	}
}