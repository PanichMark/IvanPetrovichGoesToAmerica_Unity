using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private GameObject _canvasHUDinteraction;
	private float _interactionRange = 50f;
	private bool _isInitialized = false;
	private LocalizationManager _localizationManager;
	public delegate void NonThrowableHandler();
	public event NonThrowableHandler OnPickUpThrowable;
	public event NonThrowableHandler OnPickUpNonThrowable;
	public event NonThrowableHandler OnGetRidOfPickable;

	private string _HUDInteractionMainTextInteract;

	private TextMeshProUGUI _mainInteractionText;
	private TextMeshProUGUI _additionalInteractionText;

	private TextMeshProUGUI[] _itemsTexts;
	private Image[] _itemsImages;

	private MenuManager _menuManager;

	private Sprite _noItemImageExeption;

	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private Coroutine _showAdditionalHintCoroutine;

	private PlayerBehaviourController _playerBehaviour;

	private RaycastHit _hitInfo;
	private bool _isHit;

	private GameObject _previousInteractableObject;
	private GameObject _currentInteractableObject;

	public GameObject CurrentPickableObject { get; private set; }

	private GameSceneManager _gameSceneManager;
	private GameController _gameController;

	public void Initialize(
		GameController gameController,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject canvasHUDInteraction,
		TextMeshProUGUI mainInteractionText,
		TextMeshProUGUI additionalInteractionText,
		TextMeshProUGUI[] itemsTexts,
		Image[] itemsImages)
	{
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_playerCameraController = playerCameraController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_playerBehaviour = playerBehaviour;
		_menuManager = menuManager;
		_canvasHUDinteraction = canvasHUDInteraction;

		_itemsTexts = itemsTexts;
		_itemsImages = itemsImages;

		_mainInteractionText = mainInteractionText;
		_additionalInteractionText = additionalInteractionText;

		_isInitialized = true;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDInteraction;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDInteraction;

		_menuManager.OnOpenInteractionHUD += ShowCanvasHUDInteraction;
		_menuManager.OnCloseInteractionHUD += HideCanvasHUDInteraction;

		_gameController.OnPlayerEarlyDeath += HideCanvasHUDInteraction;

		Debug.Log("InteractionController Initialized");
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_HUDInteractionMainTextInteract = localizationManager.GetLocalizedString("HUDInteraction_MainTextInteract");
	}

	private void ShowCanvasHUDInteraction()
	{
		if (!_gameController.IsMainMenuOpen)
		{
			_canvasHUDinteraction.SetActive(true);
		}

		ResetItemsUI();
	}

	private void HideCanvasHUDInteraction()
	{
		_canvasHUDinteraction.SetActive(false);
	}

	void Update()
	{
		if (!_isInitialized) return;

		UpdateInteractionRange();

		if (_playerCameraController != null)
			_isHit = Physics.Raycast(_playerCameraController.transform.position, _playerCameraController.transform.forward, out _hitInfo, _interactionRange);

		if (_isHit && _hitInfo.collider != null && _hitInfo.collider.CompareTag("Interactable"))
			ProcessInteractableObject(_hitInfo.collider.gameObject);
		else
			ClearCurrentInteractable();

		HandleInput();

		_previousInteractableObject = _currentInteractableObject;
	}

	private void UpdateInteractionRange()
	{
		if (_menuManager.IsAnyMenuOpened || _gameController.IsPlayerDead || _menuManager.IsCutsceneMenuOpened)
			_interactionRange = 0f;
		else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "FirstPerson")
			_interactionRange = 2.5f;
		else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "ThirdPerson")
			_interactionRange = 2f + _playerCameraController.PlayerCameraDistanceZ;
	}

	private void ProcessInteractableObject(GameObject interactableObject)
	{
		var interactable = interactableObject.GetComponent<IInteractable>();

		if (interactable != null)
			HighlightAndShowHint(interactableObject, interactable);

		var pickable = interactableObject.GetComponent<IPickable>();

		if (pickable != null && pickable.IsObjectPickedUp)
			CurrentPickableObject = interactableObject;
	}

	private void HighlightAndShowHint(GameObject obj, IInteractable interactable)
	{
		if (CurrentPickableObject != null)
		{
			_currentInteractableObject = null;
			_mainInteractionText.text = null;
			return;
		}

		if (_currentInteractableObject != null && _currentInteractableObject != obj)
			ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Default"));

		ChangeLayerRecursively(obj, LayerMask.NameToLayer("Outline"));

		_currentInteractableObject = obj;

		_mainInteractionText.text = $"{interactable.InteractionHintMessageMain}\n{_HUDInteractionMainTextInteract} {_inputDevice.GetNameOfKeyInteract()}";
	}

	private void ClearCurrentInteractable()
	{
		if (_currentInteractableObject != null)
			ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Default"));

		_mainInteractionText.text = null;

		if (_showAdditionalHintCoroutine != null)
			StopCoroutine(_showAdditionalHintCoroutine);

		_additionalInteractionText.text = null;

		_currentInteractableObject = null;
	}

	private void HandleInput()
	{
		if (CurrentPickableObject != null)
			HandleHeldObjectInput();
		else if (_isHit && _hitInfo.collider != null && _hitInfo.collider.CompareTag("Interactable"))
			HandleInteractInput(_hitInfo.collider.gameObject);
	}

	private void HandleHeldObjectInput()
	{
		var pickableObj = CurrentPickableObject.GetComponent<IPickable>();

		if (pickableObj == null) return;

		var throwableObj = CurrentPickableObject.GetComponent<IThrowable>();

		if (throwableObj != null)
			OnPickUpThrowable?.Invoke();
		else
			OnPickUpNonThrowable?.Invoke();

		string actionKey = throwableObj != null ? $"Бросить {_inputDevice.GetNameOfKeyRightHandWeaponAttack()}" : $"Отпустить на {_inputDevice.GetNameOfKeyInteract()}";

		_mainInteractionText.text = $"Отпустить {_inputDevice.GetNameOfKeyInteract()}\n{actionKey}";

		if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
			ReleaseHeldObject(pickableObj, throwableObj);

		if (throwableObj != null && _inputDevice.GetKeyRightHandWeaponAttack())
			ThrowHeldObject(pickableObj, throwableObj);
	}

	private void ReleaseHeldObject(IPickable pickableObj, IThrowable throwableObj)
	{
		OnGetRidOfPickable?.Invoke();
		pickableObj.DropOffObject();
		CurrentPickableObject = null;
		if (_playerBehaviour.WasPlayerArmed)
			_playerBehaviour.ArmPlayer();
	}

	private void ThrowHeldObject(IPickable pickableObj, IThrowable throwableObj)
	{
		OnGetRidOfPickable?.Invoke();
		throwableObj.ThrowObject();
		CurrentPickableObject = null;
		if (_playerBehaviour.WasPlayerArmed)
			_playerBehaviour.ArmPlayer();
	}

	private void HandleInteractInput(GameObject interactableObject)
	{
		var interactable = interactableObject.GetComponent<IInteractable>();
		var pickable = interactableObject.GetComponent<IPickable>();
		var throwable = interactableObject.GetComponent<IThrowable>();
		var gainedItem = interactableObject.GetComponent<IGainedItem>();

		if (interactable == null || !_inputDevice.GetKeyInteract()) return;

		interactable.Interact();

		if (interactable.IsInteractionHintMessageFailActive)
			StartFailMessageCoroutine(interactable.InteractionHintMessageFail);

		if (gainedItem != null)
			AddGainedItemToUI(gainedItem, interactable);

		if (pickable != null && throwable == null)
			_playerBehaviour.DisarmPlayer();

		if (pickable != null && pickable.IsObjectPickedUp)
			CurrentPickableObject = interactableObject;
	}

	private void StartFailMessageCoroutine(string message)
	{
		if (_showAdditionalHintCoroutine != null)
			StopCoroutine(_showAdditionalHintCoroutine);

		_showAdditionalHintCoroutine = StartCoroutine(ShowHintForSeconds(message));
	}

	private IEnumerator ShowHintForSeconds(string message)
	{
		if (_additionalInteractionText != null)
			_additionalInteractionText.text = message;

		yield return new WaitForSeconds(1f);

		if (_additionalInteractionText != null && _showAdditionalHintCoroutine != null)
			_additionalInteractionText.text = null;
	}

	private void AddGainedItemToUI(IGainedItem gainedItem, IInteractable interactable)
	{
		for (int i = 2; i >= 0; i--)
			if (_itemsTexts[i].gameObject.activeInHierarchy)
				ShiftItemsUp(i);

		SetNewItemSlot(0, gainedItem, interactable);

		StartCoroutine(ShowItemsGained());
	}

	private void ShiftItemsUp(int startIndex)
	{
		for (int i = startIndex; i >= 0; i--)
		{
			if (i > 0 && !_itemsTexts[i - 1].gameObject.activeInHierarchy) continue;

			if (i > 0)
			{
				_itemsTexts[i].text = _itemsTexts[i - 1].text;
				_itemsImages[i].sprite = _itemsImages[i - 1].sprite;
			}

			if (i == 0)
			{
				_itemsTexts[i].text = string.Empty;
				_itemsImages[i].sprite = null;
			}

			if (i < 2) continue;

			_itemsTexts[i].gameObject.SetActive(false);
			_itemsImages[i].gameObject.SetActive(false);
		}
	}

	private void SetNewItemSlot(int index, IGainedItem gainedItem, IInteractable interactable)
	{
		if (!_itemsTexts[index].gameObject.activeInHierarchy)
			_itemsTexts[index].gameObject.SetActive(true);

		if (!_itemsImages[index].gameObject.activeInHierarchy)
			_itemsImages[index].gameObject.SetActive(true);

		string itemName = interactable.InteractionObjectNameUI ?? string.Empty;

		Sprite itemIcon = gainedItem.IconGainedItem ?? _noItemImageExeption ?? null;

		if (!string.IsNullOrEmpty(itemName))
			_itemsTexts[index].text = itemName.Trim();

		if (itemIcon != null)
			_itemsImages[index].sprite = itemIcon;
	}

	private IEnumerator ShowItemsGained()
	{
		yield return new WaitForSeconds(2f);
		for (int i = 2; i >= 0; i--)
		{
			if (_itemsTexts[i].gameObject.activeInHierarchy)
			{
				_itemsTexts[i].text = string.Empty;
				_itemsImages[i].sprite = null;
				yield return new WaitForSeconds(0.5f);
				_itemsTexts[i].gameObject.SetActive(false);
				_itemsImages[i].gameObject.SetActive(false);
			}
		}
	}
	private void ResetItemsUI()
	{
		for (int i = 0; i < 3; i++)
		{
			_itemsTexts[i].text = string.Empty;
			_itemsImages[i].sprite = null;
			_itemsTexts[i].gameObject.SetActive(false);
			_itemsImages[i].gameObject.SetActive(false);
		}
	}
	private void ChangeLayerRecursively(GameObject obj, int layerIndex)
	{
		obj.layer = layerIndex;
		foreach (Transform child in obj.transform)
			ChangeLayerRecursively(child.gameObject, layerIndex);
	}
}