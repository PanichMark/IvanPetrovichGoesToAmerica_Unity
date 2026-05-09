using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObjectLockMechanical : MonoBehaviour, IInteractable
{
	public delegate void UnlockLockEventHandler();
	public event UnlockLockEventHandler OnUnlockLock;

	[SerializeField] private GameObject gearPrefab;
	[SerializeField] private int segmentsCount;
	[SerializeField] private float rotationSpeed;
	[SerializeField] private float moveSpeed;
	[SerializeField] private GameObject CubeFollow;

	private SaveLoadController saveLoadController;
	private LocalizationManager localizationManager;
	private GameObject canvasLockpickMechanicalMenu;
	private Button buttonExitLockpickMechanicalMenu;
	private MenuManager menuManager;
	private GameSceneManager gameSceneManager;

	private bool IsPuzzleActive;
	public bool WasUnlocked { get; private set; } = false;
	private bool isMovingOrRotating = false;
	private GameObject currentGearInstance;
	private GameObject currentCubeFollow;
	private MeshCollider EndCollider;
	private float rotationStep;
	private float movementStep;
	private MeshCollider CentreZoneCollider;
	private MeshCollider UpZoneCollider;
	private MeshCollider DownZoneCollider;
	private MeshCollider LeftZoneCollider;
	private MeshCollider RightZoneCollider;

	private bool _canMoveUp = true;
	private bool _canMoveDown = true;
	private bool _canMoveLeft = true;
	private bool _canMoveRight = true;

	private List<MeshCollider> cachedWallColliders;

	[SerializeField] private string interactionObjectNameSystem;
	public string InteractionObjectNameSystem => interactionObjectNameSystem;

	private string interactionHintMessageMain;
	public string InteractionHintMessageMain => interactionHintMessageMain;

	public string InteractionHintAction { get; protected set; }

	public string InteractionHintMessageAdditional => null;

	public bool IsInteractionHintMessageAdditionalActive => false;

	public string InteractionObjectNameUI { get; protected set; }

	private Text buttonText;

	private void Awake()
	{
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasLockpickMechanicalMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuLockpickMechanical");
		buttonExitLockpickMechanicalMenu = ServiceLocator.Resolve<Button>("ButtonExitLockpickMechanicalMenu");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		gameSceneManager.OnBeginLoadMainMenuScene += OnClosePuzzle;
		gameSceneManager.OnBeginLoadGameplayScene += OnClosePuzzle;
		buttonText = buttonExitLockpickMechanicalMenu.GetComponentInChildren<Text>();

		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintActione_Lockpick");

		buttonText.text = localizationManager.GetLocalizedString("MenuInteractionLockPick_ExitButton");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";

		localizationManager.OnLanguageChangeEvent += ChangeLanguage;

		menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		menuManager.OnClosePauseMenu += ShowPuzzleCanvas;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintActione_Lockpick");

		buttonText.text = localizationManager.GetLocalizedString("MenuInteractionLockPick_ExitButton");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}

	private void Update()
	{
		if (!isMovingOrRotating && currentGearInstance != null && !menuManager.IsPauseMenuOpened)
		{
			if (_canMoveUp && Input.GetKeyDown(KeyCode.UpArrow))
			{
				StartCoroutine(RotateGear(rotationStep));
			}
			else if (_canMoveDown && Input.GetKeyDown(KeyCode.DownArrow))
			{
				StartCoroutine(RotateGear(-rotationStep));
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

	private void HidePuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickMechanicalMenu.SetActive(false);
			currentGearInstance.SetActive(false);
			currentCubeFollow.SetActive(false);
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickMechanicalMenu.SetActive(true);
			currentGearInstance.SetActive(true);
			currentCubeFollow.SetActive(true);
		}
	}

	public void Interact()
	{
		menuManager.OpenInteractionMenu();
		IsPuzzleActive = true;

		currentGearInstance = Instantiate(gearPrefab, GetPuzzleSpawnPosition(), Quaternion.identity);
		currentGearInstance.transform.LookAt(Camera.main.transform);
		currentGearInstance.transform.Translate(-0.05f, 0f, 0f, Space.Self);

		Transform endTransform = currentGearInstance.transform.Find("END");
		if (endTransform != null)
		{
			EndCollider = endTransform.GetComponent<MeshCollider>();
		}
		else
		{
			Debug.LogError("END object not found.");
		}

		canvasLockpickMechanicalMenu.SetActive(true);
		buttonExitLockpickMechanicalMenu.onClick.RemoveAllListeners();
		buttonExitLockpickMechanicalMenu.onClick.AddListener(OnClosePuzzle);

		gameObject.tag = "Untagged";

		currentCubeFollow = Instantiate(CubeFollow, GetCubeSpawnPosition(), Quaternion.identity);
		currentCubeFollow.transform.LookAt(Camera.main.transform);

		Transform root = currentCubeFollow.transform;
		CentreZoneCollider = root.Find("CentreZone")?.GetComponent<MeshCollider>();
		UpZoneCollider = root.Find("UpZone")?.GetComponent<MeshCollider>();
		DownZoneCollider = root.Find("DownZone")?.GetComponent<MeshCollider>();
		LeftZoneCollider = root.Find("LeftZone")?.GetComponent<MeshCollider>();
		RightZoneCollider = root.Find("RightZone")?.GetComponent<MeshCollider>();

		Transform wallsGroup = currentGearInstance.transform.Find("Walls");
		if (wallsGroup != null)
		{
			cachedWallColliders = new List<MeshCollider>(wallsGroup.GetComponentsInChildren<MeshCollider>());
		}
		else
		{
			Debug.LogError("Walls group not found.");
		}

		CheckForIntersection();

		if (UpZoneCollider == null || DownZoneCollider == null ||
			LeftZoneCollider == null || RightZoneCollider == null)
		{
			Debug.LogWarning("Failed to assign zone detectors!");
		}

		rotationStep = 360f / segmentsCount;
		movementStep = 0.1f;
	}

	private void OnClosePuzzle()
	{
		if (IsPuzzleActive)
		{
			IsPuzzleActive = false;
			canvasLockpickMechanicalMenu.SetActive(false);
			Destroy(currentGearInstance);
			Destroy(currentCubeFollow);
			gameObject.tag = "Interactable";
			menuManager.CloseInteractionMenu();
		}
	}

	private void CheckForIntersection()
	{
		Physics.SyncTransforms();

		if (currentCubeFollow != null && currentGearInstance != null)
		{
			_canMoveUp = true;
			_canMoveDown = true;
			_canMoveLeft = true;
			_canMoveRight = true;

			if (cachedWallColliders != null)
			{
				foreach (var collider in cachedWallColliders)
				{
					if (IsIntersectingWithCollider(UpZoneCollider, collider)) _canMoveUp = false;
					if (IsIntersectingWithCollider(DownZoneCollider, collider)) _canMoveDown = false;
					if (IsIntersectingWithCollider(LeftZoneCollider, collider)) _canMoveLeft = false;
					if (IsIntersectingWithCollider(RightZoneCollider, collider)) _canMoveRight = false;
				}
			}
			else
			{
				Debug.LogError("Wall colliders list is not populated.");
			}

			if (EndCollider != null && IsIntersectingWithCollider(CentreZoneCollider, EndCollider))
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
		isMovingOrRotating = true;
		Quaternion startRotation = currentGearInstance.transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(new Vector3(targetAngle, 0, 0));

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * rotationSpeed;
			yield return null;
		}

		currentGearInstance.transform.rotation = endRotation;
		CheckForIntersection();
		isMovingOrRotating = false;
	}

	IEnumerator MoveRight()
	{
		isMovingOrRotating = true;
		Vector3 startPosition = currentGearInstance.transform.position;
		Vector3 endPosition = startPosition + currentGearInstance.transform.right * movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * moveSpeed;
			yield return null;
		}

		currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		isMovingOrRotating = false;
	}

	IEnumerator MoveLeft()
	{
		isMovingOrRotating = true;
		Vector3 startPosition = currentGearInstance.transform.position;
		Vector3 endPosition = startPosition - currentGearInstance.transform.right * movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * moveSpeed;
			yield return null;
		}

		currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		isMovingOrRotating = false;
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