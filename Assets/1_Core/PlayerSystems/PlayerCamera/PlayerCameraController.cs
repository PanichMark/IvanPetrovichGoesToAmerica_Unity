using UnityEngine;

public class PlayerCameraController : MonoBehaviour, ISaveLoad
{
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerMovementController _movementController;
	private PlayerColliderController _playerCollider;
	private GameObject _player;
	private GameObject _playerCamera;

	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private bool _isCameraFirstPerson;
	private Camera _mainCamera;
	private Vector2 _mouseRotation;
	private Vector2 _mouseScrollWheel;
	private GameSceneManager _gameSceneManager;
	private RaycastHit _hit;
	private float _currentFOV;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;

	public bool IsAbleToZoomCameraOut { get; private set; } = true;

	public float PlayerCameraDistanceX { get; private set; }
	public float PlayerCameraDistanceY { get; private set; }
	public float PlayerCameraDistanceZ { get; private set; }

	private float _MouseRotationLimit = 80f;

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

		if (_mouseScrollWheel.y < 0 && IsAbleToZoomCameraOut == true && !_isCameraFirstPerson)
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
		if (_mouseScrollWheel.y > 0 && !_isCameraFirstPerson)
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

		if (_inputDevice.GetKeyChangeCameraShoulder() && !_isCameraFirstPerson)
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
			if (Physics.Linecast(_playerCollider.transform.position, transform.position, out _hit, ~LayerMask.GetMask("InvisibleWall")))
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

	public void SetToFirstPerson()
	{
		_isCameraFirstPerson = true;
	}

	public void SetToThirdPerson()
	{
		_isCameraFirstPerson = false;
	}

	private void FixedUpdate()
	{
		if (_mouseRotation.y >= 360)
		{
			_mouseRotation.y = 0;
		}
		if (_mouseRotation.y <= -360)
		{
			_mouseRotation.y = 0;
		}
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
		transform.position = _player.transform.position + Quaternion.Euler(0, _mouseRotation.y, 0) *
		new Vector3(0, _movementController.PlayerCurrentHeight - 0.13f, 0.1f);
	}

	public void ThirdPersonCameraTransform()
	{
		transform.position = _player.transform.position - Quaternion.Euler(-_mouseRotation.x, _mouseRotation.y, 0) *
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
		data.PlayerCameraDistanceY = PlayerCameraDistanceY;
		data.PlayerCameraDistanceZ = PlayerCameraDistanceZ;
		data.CameraRotation = new Quaternion(-_mouseRotation.x, _mouseRotation.y, 0, 0);
		data.IsCameraShoulderRight = _isCameraShoulderRight;
	}

	public void LoadData(GameData data)
	{
		PlayerCameraDistanceY = data.PlayerCameraDistanceY;
		PlayerCameraDistanceZ = data.PlayerCameraDistanceZ;
		_mouseRotation.x = -data.CameraRotation.x;
		_mouseRotation.y = data.CameraRotation.y;
		_isCameraShoulderRight = data.IsCameraShoulderRight;

	}

	public void RotateCamera()
	{
		if (!_menuManager.IsAnyMenuOpened)
		{
			_mouseRotation.y += Input.GetAxis("Mouse X");
			_mouseRotation.x += Input.GetAxis("Mouse Y");
			_mouseRotation.x = Mathf.Clamp(_mouseRotation.x, _MouseRotationLimit * -1, _MouseRotationLimit);
			_mouseScrollWheel = Input.mouseScrollDelta;

			transform.rotation = Quaternion.Euler(-_mouseRotation.x, _mouseRotation.y, 0);
		}
	}

	private void SetCameraFOV(float newFov, float MIN_FOV_VALUE, float MAX_FOV_VALUE)
	{
		_mainCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);

		_currentFOV = _pauseSubMenuSettingsSectionGeneralController.CurrentValueCameraFOV;
	}

	private void SendCameraFOV()
	{
		_pauseSubMenuSettingsSectionGeneralController.GetCameraCurrentFOV(_currentFOV);
	}

	public void Initialize(GameController gameController, IInputDevice inputDevice, GameSceneManager gameSceneManager, MenuManager menuManager, PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController, PlayerMovementController movementController, PlayerMovementStateMachineController playerMovementStateMachineController, PlayerColliderController playerCollider, GameObject playerModel, GameObject playerCamera)
	{
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_movementController = movementController; 
		_playerCollider = playerCollider;
		_player = playerModel;
		_playerCamera = playerCamera;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		PlayerCameraDistanceX = -0.85f;
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		_mainCamera = _playerCamera.GetComponent<Camera>();

		_pauseSubMenuSettingsSectionGeneralController.OnMainCameraFOVchanged += SetCameraFOV;
		_pauseSubMenuSettingsSectionGeneralController.OnSaveCameraSettingsData += SendCameraFOV;

		_gameController.OnCloseMainMenu += () =>
		{
			SendCameraFOV();
			Debug.Log(_currentFOV);
			_pauseSubMenuSettingsSectionGeneralController.SetFOV(_currentFOV);
		};
		_gameController.OnOpenMainMenu += SendCameraFOV;

		_isInitialized = true;

		Debug.Log("CameraController Initialized");
	}
}