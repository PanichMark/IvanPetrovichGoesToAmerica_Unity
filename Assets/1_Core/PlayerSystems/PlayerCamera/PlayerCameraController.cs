using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerMovementController _movementController;
	private PlayerColliderController _playerCollider;
	private GameObject _player;
	private GameObject _playerCamera;
	private float _mouseSensitivityMultiplierX;
	private float _mouseSensitivityMultiplierY;
	private bool _isCameraFirstPerson;
	private Camera _mainCamera;
	private Vector2 _mouseRotation;
	private float _mouseScrollWheel;
	private RaycastHit _hit;
	private float _currentFOV;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	PauseSubMenuSettingsSectionControlsController _pauseSubMenuSettingsSectionControlsController;
	private Coroutine _activeRecoilCoroutineSingle;
	private Vector2 _recoilStartRotationSingle;
	private Coroutine _activeAutoRecoilCoroutine;
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

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PauseSubMenuSettingsSectionControlsController pauseSubMenuSettingsSectionControlsController,
		PlayerMovementController movementController,
		PlayerColliderController playerCollider,
		GameObject playerModel,
		GameObject playerCamera)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_pauseSubMenuSettingsSectionControlsController = pauseSubMenuSettingsSectionControlsController;
		_movementController = movementController;
		_playerCollider = playerCollider;
		_player = playerModel;
		_playerCamera = playerCamera;
		PlayerCameraDistanceX = -0.85f;
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		_mainCamera = _playerCamera.GetComponent<Camera>();

		_pauseSubMenuSettingsSectionGeneralController.OnCameraFOVchanged += SetCameraFOV;
		_pauseSubMenuSettingsSectionGeneralController.OnSaveCameraSettingsData += SendCameraFOV;

		_pauseSubMenuSettingsSectionControlsController.OnMouseSensitivityXchanged += ChangeMouseSensitivityMultiplierX;
		_pauseSubMenuSettingsSectionControlsController.OnMouseSensitivityYchanged += ChangeMouseSensitivityMultiplierY;

		_gameController.OnCloseMainMenu += () =>
		{
			SendCameraFOV();
			//Debug.Log(_currentFOV);
			_pauseSubMenuSettingsSectionGeneralController.SetCameraFOV(_currentFOV);
		};
		_gameController.OnOpenMainMenu += SendCameraFOV;

		Debug.Log("PlayerCameraController Initialized");
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
		{
			return;
		}

		//Debug.Log(_mouseSensitivityMultiplierX);
		//Debug.Log(_mouseSensitivityMultiplierY);

		if (_mouseScrollWheel < 0 && IsAbleToZoomCameraOut == true && !_isCameraFirstPerson)
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
		if (_mouseScrollWheel > 0 && !_isCameraFirstPerson)
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
		//Debug.Log(_mouseRotation.x);
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
		transform.position = _player.transform.position + Quaternion.Euler(0, _mouseRotation.x, 0) *
		new Vector3(0, _movementController.PlayerCurrentHeight - 0.13f, 0.1f);
	}

	public void ThirdPersonCameraTransform()
	{
		transform.position = _player.transform.position - Quaternion.Euler(-_mouseRotation.y, _mouseRotation.x, 0) *
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

	private void ChangeMouseSensitivityMultiplierX(float newMouseSensitivityX)
	{
		_mouseSensitivityMultiplierX = newMouseSensitivityX;	
	}

	private void ChangeMouseSensitivityMultiplierY(float newMouseSensitivityY)
	{
		_mouseSensitivityMultiplierY = newMouseSensitivityY;
	}

	public void RotateCamera()
	{
		if (!_menuManager.IsAnyMenuOpened)
		{
			_mouseRotation.y += _inputDevice.CameraAxisY() * _mouseSensitivityMultiplierY;
			_mouseRotation.x += _inputDevice.CameraAxisX() * _mouseSensitivityMultiplierX;
			_mouseRotation.y = Mathf.Clamp(_mouseRotation.y, _MouseRotationLimit * -1, _MouseRotationLimit);
			_mouseScrollWheel = _inputDevice.CameraScroll();

			transform.rotation = Quaternion.Euler(-_mouseRotation.y, _mouseRotation.x, 0);
		}
	}

	public void ApplyWeaponRecoilSingle(int UpForce, float UpForceDuration, float DownForceDuration)
	{
		if (_activeRecoilCoroutineSingle != null)
		{
			StopCoroutine(_activeRecoilCoroutineSingle);
		}
		_recoilStartRotationSingle = _mouseRotation;
		_activeRecoilCoroutineSingle = StartCoroutine(WeaponRecoilSingle(UpForce, UpForceDuration, DownForceDuration));
	}

	private IEnumerator WeaponRecoilSingle(int UpForce, float UpForceDuration, float DownForceDuration)
	{
		float startY = _recoilStartRotationSingle.y;
		float targetUp = startY + UpForce;

		float timeElapsed = 0f;
		while (timeElapsed < UpForceDuration)
		{
			if (Vector2.Distance(_mouseRotation, _recoilStartRotationSingle) > 25f)
			{
				_activeRecoilCoroutineSingle = null;
				//StopCoroutine(SawedOffShotgunRecoil());
				yield break;
			}

			_mouseRotation.y = Mathf.Lerp(startY, targetUp, timeElapsed / UpForceDuration);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		_mouseRotation.y = targetUp;

		timeElapsed = 0f;
		while (timeElapsed < DownForceDuration)
		{
			if (Vector2.Distance(_mouseRotation, _recoilStartRotationSingle) > 25f)
			{
				_activeRecoilCoroutineSingle = null;
				//StopCoroutine(SawedOffShotgunRecoil());
				yield break;
			}

			_mouseRotation.y = Mathf.Lerp(targetUp, startY, timeElapsed / DownForceDuration);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		_mouseRotation.y = startY;
		_activeRecoilCoroutineSingle = null;
	}

	public void ApplyWeaponRecoilAuto()
	{
		if (_activeAutoRecoilCoroutine != null)
		{
			StopCoroutine(_activeAutoRecoilCoroutine);
		}
		_activeAutoRecoilCoroutine = StartCoroutine(WeaponRecoilAutoRoutine());
	}

	private IEnumerator WeaponRecoilAutoRoutine()
	{
		while (true)
		{
			float verticalRecoil = Random.Range(0.5f, 3f);

			float horizontalRecoil;
			if (Random.value < 0.5f)
			{
				horizontalRecoil = -Random.Range(0.5f, 2.5f);
			}
			else
			{
				horizontalRecoil = Random.Range(0.5f, 2.5f);
			}

			float timeElapsed = 0f;
			float duration = 0.05f;
			Vector2 startRotation = _mouseRotation;
			Vector2 targetRotation = _mouseRotation + new Vector2(horizontalRecoil, verticalRecoil);

			while (timeElapsed < duration)
			{
				_mouseRotation = Vector2.Lerp(startRotation, targetRotation, timeElapsed / duration);
				timeElapsed += Time.deltaTime;
				yield return null;
			}

			_mouseRotation = targetRotation;

			yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

			yield break;
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
}