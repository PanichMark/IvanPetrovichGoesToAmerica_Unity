using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObjectLockMechanical : MonoBehaviour, IInteractable
{
	public delegate void UnlockLockEventHandler();
	public event UnlockLockEventHandler OnUnlockLock;

	[SerializeField] private GameObject _gearPrefab;
	[SerializeField] private string _interactionObjectNameSystem;
	[SerializeField] private int _segmentsCount;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private float _moveSpeed;
	private GameObject _cubeFollow;

	private SaveLoadController _saveLoadController;
	private LocalizationManager _localizationManager;
	private GameObject _canvasLockpickMechanicalMenu;
	public event IInteractable.InteractableObjectHandler OnInteract;
	private Button _buttonExitLockpickMechanicalMenu;
	private TextMeshProUGUI _textButtonExitLockpickMechanicalMenu;
	private MenuManager _menuManager;
	private GameSceneManager _gameSceneManager;

	private bool _iIsPuzzleActive;
	public bool WasUnlocked { get; private set; } = false;
	private bool _isMovingOrRotating = false;
	private GameObject _currentGearInstance;
	private GameObject _currentCubeFollow;
	private MeshCollider _endCollider;
	private float _rotationStep;
	private float _movementStep;
	private MeshCollider _centreZoneCollider;
	private MeshCollider _upZoneCollider;
	private MeshCollider _downZoneCollider;
	private MeshCollider _leftZoneCollider;
	private MeshCollider _rightZoneCollider;

	private bool _canMoveUp = true;
	private bool _canMoveDown = true;
	private bool _canMoveLeft = true;
	private bool _canMoveRight = true;

	private List<MeshCollider> _cachedWallColliders;

	public string InteractionObjectNameSystem => _interactionObjectNameSystem;

	private string _interactionHintMessageMain;
	public string InteractionHintMessageMain => _interactionHintMessageMain;

	public string InteractionHintMessageAction { get; protected set; }

	public string InteractionHintMessageFail => null;

	public bool IsInteractionHintMessageFailActive => false;

	public string InteractionObjectNameUI { get; protected set; }


	private void Awake()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_canvasLockpickMechanicalMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuLockpickMechanical");
		_buttonExitLockpickMechanicalMenu = ServiceLocator.Resolve<GameObject>("ButtonCloseLockpickMechanicalMenu").GetComponent<Button>();
		_textButtonExitLockpickMechanicalMenu = ServiceLocator.Resolve<GameObject>("TextButtonCloseLockpickMechanicalMenu").GetComponent<TextMeshProUGUI>();
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += OnClosePuzzle;
		_gameSceneManager.OnBeginLoadingGameplayScene += OnClosePuzzle;
		_textButtonExitLockpickMechanicalMenu.text = _localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_LockMechanical_ButtonCloseMenuLockMechanical");
		
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Lockpick");
		_cubeFollow = Resources.Load<GameObject>("InteractionObjects/InteractionObjects_Locks/Lock_Mechanical_PuzzleCube");


		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		_menuManager.OnClosePauseMenu += ShowPuzzleCanvas;
	}

	private void Update()
	{
		if (!_isMovingOrRotating && _currentGearInstance != null && !_menuManager.IsPauseMenuOpened)
		{
			if (_canMoveUp && Input.GetKeyDown(KeyCode.UpArrow))
			{
				StartCoroutine(RotateGear(_rotationStep));
			}
			else if (_canMoveDown && Input.GetKeyDown(KeyCode.DownArrow))
			{
				StartCoroutine(RotateGear(-_rotationStep));
			}
			else if (_canMoveRight && Input.GetKeyDown(KeyCode.RightArrow))
			{
				StartCoroutine(MoveRight());
			}
			else if (_canMoveLeft && Input.GetKeyDown(KeyCode.LeftArrow))
			{
				StartCoroutine(MoveLeft());
			}
		}
	}

	public void Interact()
	{
		_menuManager.OpenInteractionMenu();
		_iIsPuzzleActive = true;

		_currentGearInstance = Instantiate(_gearPrefab, GetPuzzleSpawnPosition(), Quaternion.identity);
		_currentGearInstance.transform.LookAt(Camera.main.transform);
		_currentGearInstance.transform.Translate(-0.05f, 0f, 0f, Space.Self);

		Transform endTransform = _currentGearInstance.transform.Find("END");
		if (endTransform != null)
		{
			_endCollider = endTransform.GetComponent<MeshCollider>();
		}
		else
		{
			Debug.LogError("END object not found.");
		}

		_canvasLockpickMechanicalMenu.SetActive(true);
		_buttonExitLockpickMechanicalMenu.onClick.RemoveAllListeners();
		_buttonExitLockpickMechanicalMenu.onClick.AddListener(OnClosePuzzle);

		gameObject.tag = "Untagged";

		_currentCubeFollow = Instantiate(_cubeFollow, GetCubeSpawnPosition(), Quaternion.identity);
		_currentCubeFollow.transform.LookAt(Camera.main.transform);

		Transform root = _currentCubeFollow.transform;
		_centreZoneCollider = root.Find("CentreZone")?.GetComponent<MeshCollider>();
		_upZoneCollider = root.Find("UpZone")?.GetComponent<MeshCollider>();
		_downZoneCollider = root.Find("DownZone")?.GetComponent<MeshCollider>();
		_leftZoneCollider = root.Find("LeftZone")?.GetComponent<MeshCollider>();
		_rightZoneCollider = root.Find("RightZone")?.GetComponent<MeshCollider>();

		Transform wallsGroup = _currentGearInstance.transform.Find("Walls");
		if (wallsGroup != null)
		{
			_cachedWallColliders = new List<MeshCollider>(wallsGroup.GetComponentsInChildren<MeshCollider>());
		}
		else
		{
			Debug.LogError("Walls group not found.");
		}

		CheckForIntersection();

		if (_upZoneCollider == null || _downZoneCollider == null ||
			_leftZoneCollider == null || _rightZoneCollider == null)
		{
			Debug.LogWarning("Failed to assign zone detectors!");
		}

		_rotationStep = 360f / _segmentsCount;
		_movementStep = 0.1f;
	}

	public void InteractCutscene()
	{
		Interact();
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Lockpick");
		_textButtonExitLockpickMechanicalMenu.text = _localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_LockMechanical_ButtonCloseMenuLockMechanical");
		//	_buttonText.text = _localizationManager.GetLocalizedString("MenuInteractionLockPick_ExitButton");
	
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	}

	private void HidePuzzleCanvas()
	{
		if (_iIsPuzzleActive)
		{
			_canvasLockpickMechanicalMenu.SetActive(false);
			_currentGearInstance.SetActive(false);
			_currentCubeFollow.SetActive(false);
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (_iIsPuzzleActive)
		{
			_canvasLockpickMechanicalMenu.SetActive(true);
			_currentGearInstance.SetActive(true);
			_currentCubeFollow.SetActive(true);
		}
	}

	private void OnClosePuzzle()
	{
		if (_iIsPuzzleActive)
		{
			_iIsPuzzleActive = false;
			_canvasLockpickMechanicalMenu.SetActive(false);
			Destroy(_currentGearInstance);
			Destroy(_currentCubeFollow);
			gameObject.tag = "Interactable";
			_menuManager.CloseInteractionMenu();
		}
	}

	private void CheckForIntersection()
	{
		Physics.SyncTransforms();

		if (_currentCubeFollow != null && _currentGearInstance != null)
		{
			_canMoveUp = true;
			_canMoveDown = true;
			_canMoveLeft = true;
			_canMoveRight = true;

			if (_cachedWallColliders != null)
			{
				foreach (var collider in _cachedWallColliders)
				{
					if (IsIntersectingWithCollider(_upZoneCollider, collider)) _canMoveUp = false;
					if (IsIntersectingWithCollider(_downZoneCollider, collider)) _canMoveDown = false;
					if (IsIntersectingWithCollider(_leftZoneCollider, collider)) _canMoveLeft = false;
					if (IsIntersectingWithCollider(_rightZoneCollider, collider)) _canMoveRight = false;
				}
			}
			else
			{
				Debug.LogError("Wall colliders list is not populated.");
			}

			if (_endCollider != null && IsIntersectingWithCollider(_centreZoneCollider, _endCollider))
			{
				Debug.Log("Center intersected with END object.");
				WasUnlocked = true;
				OnClosePuzzle();
				OnUnlockLock?.Invoke();
			}
		}
	}

	private bool IsIntersectingWithCollider(MeshCollider firstCollider, MeshCollider secondCollider)
	{
		Vector3 direction;
		float distance;

		return Physics.ComputePenetration(
			firstCollider, firstCollider.transform.position, firstCollider.transform.rotation,
			secondCollider, secondCollider.transform.position, secondCollider.transform.rotation,
			out direction, out distance);
	}

	IEnumerator RotateGear(float targetAngle)
	{
		_isMovingOrRotating = true;
		Quaternion startRotation = _currentGearInstance.transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(new Vector3(targetAngle, 0, 0));

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			_currentGearInstance.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * _rotationSpeed;
			yield return null;
		}

		_currentGearInstance.transform.rotation = endRotation;
		CheckForIntersection();
		_isMovingOrRotating = false;
	}

	IEnumerator MoveRight()
	{
		_isMovingOrRotating = true;
		Vector3 startPosition = _currentGearInstance.transform.position;
		Vector3 endPosition = startPosition + _currentGearInstance.transform.right * _movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			_currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * _moveSpeed;
			yield return null;
		}

		_currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		_isMovingOrRotating = false;
	}

	IEnumerator MoveLeft()
	{
		_isMovingOrRotating = true;
		Vector3 startPosition = _currentGearInstance.transform.position;
		Vector3 endPosition = startPosition - _currentGearInstance.transform.right * _movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			_currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * _moveSpeed;
			yield return null;
		}

		_currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		_isMovingOrRotating = false;
	}

	private Vector3 GetPuzzleSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 1f; // Spawn in front of the camera.
	}

	private Vector3 GetCubeSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 0.7f; // Spawn slightly closer to the camera.
	}
}