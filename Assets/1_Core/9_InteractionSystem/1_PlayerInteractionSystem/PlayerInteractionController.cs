using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractionController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameObject _canvasHUDinteraction;

	private float _interactionRange = 50f;
	private ViewModelHUDInteraction _viewModelHUDInteraction;
	private LocalizationManager _localizationManager;
	public delegate void PickableObjectsPickUpHandler(InteractionObjectsPickableTypes pickableType);

	public event PickableObjectsPickUpHandler OnPickUpThrowable;
	public event PickableObjectsPickUpHandler OnPickUpNonThrowable;

	public delegate void PickableObjectsGetRidOfHandler();
	public event PickableObjectsGetRidOfHandler OnGetRidOfNonThrowable;
	public event PickableObjectsGetRidOfHandler OnGetRidOfThrowable;

	public delegate void ThrowableObjedctThrowHandler(InteractionObjectsPickableTypes throwableType);
	public event ThrowableObjedctThrowHandler OnThrowTrowable;

	private string _HUDInteractionMainTextInteract;
	private string _HUDInteractionDropText;
	private string _HUDInteractionThrowText;
	private bool _changedPickedUpState;
	private TextMeshProUGUI _mainInteractionText;
	private TextMeshProUGUI _failInteractionText;

	private TextMeshProUGUI[] _itemsTexts;
	private Image[] _itemsImages;

	private MenuManager _menuManager;
	private GameObject _HUDinteraction;
	private GameObject _HUDphraseLine;
	private Sprite _ImageMissing;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private Coroutine _showAdditionalHintCoroutine;
	private int _layersInteractionToIgnore;
	private PlayerBehaviourController _playerBehaviour;

	private IInteractable _lookedAtIInteractable;
	private IPickable _lookedAtIPickable;
	private IThrowable _lookedAtIThrowableObject;

	private IGainedItem _lookedAtIGainedItem;

	private IPickable _currentIPickable;
	public IThrowable CurrentIThrowable {  get; private set; }

	private RaycastHit _hitObject;
	private bool _isInteractionObjectLookedAt;

	private GameObject _previousInteractableObject;
	private GameObject _currentInteractableObject;
	public GameObject CurrentPickableObject { get; private set; }
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject canvasHUDInteraction,
		ViewModelHUDInteraction viewModelHUDInteraction)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_playerCameraController = playerCameraController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_playerBehaviour = playerBehaviour;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasHUDinteraction = canvasHUDInteraction;
		_viewModelHUDInteraction = viewModelHUDInteraction;
		_HUDinteraction = viewModelHUDInteraction.HUDinteraction;
		_HUDphraseLine = viewModelHUDInteraction.HUDphraseLine;

		_itemsTexts = new TextMeshProUGUI[viewModelHUDInteraction.TextsGainedItems.Length];
		for (int i = 0; i < viewModelHUDInteraction.TextsGainedItems.Length; i++)
		{
			_itemsTexts[i] = viewModelHUDInteraction.TextsGainedItems[i].GetComponent<TextMeshProUGUI>();
		}

		_itemsImages = new Image[viewModelHUDInteraction.ImagesGainedItems.Length];
		for (int i = 0; i < viewModelHUDInteraction.ImagesGainedItems.Length; i++)
		{
			_itemsImages[i] = viewModelHUDInteraction.ImagesGainedItems[i].GetComponent<Image>();
		}

		_mainInteractionText = viewModelHUDInteraction.TextInteractionMessageMain.GetComponent<TextMeshProUGUI>();
		_failInteractionText = viewModelHUDInteraction.TextInteractionMessageFail.GetComponent<TextMeshProUGUI>();

		_HUDInteractionDropText = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drop");
		_HUDInteractionThrowText = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Throw");

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDInteraction;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDInteraction;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDinteraction;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDinteraction;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDinteraction;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDinteraction;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDphraseLine;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += ShowHUDphraseLine;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += HideHUDphraseLine;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDphraseLine;

		_menuManager.OnOpenInteractionHUD += ShowCanvasHUDInteraction;
		_menuManager.OnCloseInteractionHUD += HideCanvasHUDInteraction;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_gameController.OnPlayerEarlyDeath += ChangeInteractionRange;
		_gameController.OnPlayerRevive += ChangeInteractionRange;
		_menuManager.OnOpenAnyMenu += ChangeInteractionRange;
		_menuManager.OnCloseAnyMenu += ChangeInteractionRange;
		_menuManager.OnOpenCutsceneMenu += ChangeInteractionRange;
		_menuManager.OnCloseCutsceneMenu += ChangeInteractionRange;
		_playerCameraStateMachineController.OnCameraStateChanged += ChangeInteractionRange;

		_layersInteractionToIgnore = LayerMask.GetMask("HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");

		Debug.Log("InteractionController Initialized");
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_HUDInteractionMainTextInteract = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Main");
		_HUDInteractionDropText = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drop");
		_HUDInteractionThrowText = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Throw");
	}

	private void ShowCanvasHUDInteraction()
	{
		if (!_gameController.IsMainMenuOpen)
		{
			_canvasHUDinteraction.gameObject.SetActive(true);
		}

		_itemsTexts[2].text = null;
		_itemsTexts[2].gameObject.SetActive(false);
		_itemsImages[2].sprite = null;
		_itemsImages[2].gameObject.SetActive(false);

		_itemsTexts[1].text = null;
		_itemsTexts[1].gameObject.SetActive(false);
		_itemsImages[1].sprite = null;
		_itemsImages[1].gameObject.SetActive(false);

		_itemsTexts[0].text = null;
		_itemsTexts[0].gameObject.SetActive(false);
		_itemsImages[0].sprite = null;
		_itemsImages[0].gameObject.SetActive(false);
	}

	private void ShowHUDinteraction()
	{
		_HUDinteraction.SetActive(true);
	}

	private void HideHUDinteraction()
	{
		_HUDinteraction.SetActive(false);
	}

	private void ShowHUDphraseLine()
	{
		_HUDphraseLine.SetActive(true);
	}

	private void HideHUDphraseLine()
	{
		_HUDphraseLine.SetActive(false);
	}

	private void HideCanvasHUDInteraction()
	{
		_canvasHUDinteraction.gameObject.SetActive(false);
	}

	private void ChangeInteractionRange()
	{
		//Debug.Log("RANGE");
		//ADD ON THAT FROM INPUT DEVICE!!!
		//	_interactionRange = 2f + _playerCameraController.PlayerCameraDistanceZ;
		if (_menuManager.IsAnyMenuOpened || _gameController.IsPlayerDead || _menuManager.IsCutsceneMenuOpened || _currentIPickable != null)
		{
			_interactionRange = 0;
		}
		else
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				_interactionRange = 2.5f;
			}
			else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
			{
				_interactionRange = 2f + _playerCameraController.PlayerCameraDistanceZ;
			}
		}
	}

	private void PickUpPickableObject()
	{
		_currentIPickable = CurrentPickableObject.GetComponent<IPickable>();
		CurrentIThrowable = CurrentPickableObject.GetComponent<IThrowable>();

		//Debug.Log(_currentIPickable);
		//Debug.Log(_currentIThrowable);

		if (!_changedPickedUpState)
		{
			ChangeInteractionRange();
			_changedPickedUpState = true;
		}
	
		if (CurrentIThrowable != null)
		{
			OnPickUpThrowable?.Invoke(_currentIPickable.PickableType);

			_mainInteractionText.text = $"{_HUDInteractionDropText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}\n{_HUDInteractionThrowText} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackRightHand)}";
			//ChangeLayerRecursively(CurrentPickableObject, LayerMask.NameToLayer("FirstPerson"));
		}
		else
		{
			OnPickUpNonThrowable?.Invoke(_currentIPickable.PickableType);

			WeaponPickableRangedAbstract pickableRangedWeapon = CurrentPickableObject.GetComponent<WeaponPickableRangedAbstract>();
			if (pickableRangedWeapon == null)
			{
				_mainInteractionText.text = $"{_HUDInteractionDropText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}";
			}
			else
			{
				_mainInteractionText.text = $"{_HUDInteractionDropText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}\n{_localizationManager.GetLocalizedString(pickableRangedWeapon.WeaponRightMouseButtonAttackMessage)} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackRightHand)}\n{_localizationManager.GetLocalizedString(pickableRangedWeapon.WeaponLeftMouseButtonAttackMessage)} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackLeftHand)}";
			}

			ChangeLayerRecursively(CurrentPickableObject, LayerMask.NameToLayer("Default"));
		}
	}

	private void DropPickable()
	{
		_currentIPickable.DropOffObject();
		_currentIPickable = null;
		_changedPickedUpState = false;
		CurrentPickableObject = null;
		ChangeInteractionRange();
		//Debug.Log(_playerBehaviour.WasPlayerArmed);
		if (CurrentIThrowable == null)
		{
			OnGetRidOfNonThrowable?.Invoke();

			if (_playerBehaviour.WasPlayerArmed == true)
			{
				_playerBehaviour.ArmPlayer();
			}
		}
		else
		{
			CurrentIThrowable = null;

			OnGetRidOfThrowable?.Invoke();

			if (_playerBehaviour.IsPlayerArmed == true)
			{
				_playerBehaviour.ArmPlayer();
			}
		}
	}

	public void EarlyThrowThrowable()
	{
		//OnThrowTrowable?.Invoke();
		//OnGetRidOfThrowable?.Invoke();
		CurrentIThrowable.ThrowObject();
		_currentIPickable = null;
		CurrentIThrowable = null;
		_changedPickedUpState = false;
		CurrentPickableObject = null;
		ChangeInteractionRange();
		//Debug.Log(_playerBehaviour.WasPlayerArmed);
		/*
		if (_playerBehaviour.WasPlayerArmed == true)
		{
			_playerBehaviour.ArmPlayer();
		}
		*/
	}

	public void LateThrowThrowable()
	{
		//OnThrowTrowable?.Invoke();
		OnGetRidOfThrowable?.Invoke();
		/*
		CurrentIThrowable.ThrowObject();
		_currentIPickable = null;
		CurrentIThrowable = null;
		_changedPickedUpState = false;
		CurrentPickableObject = null;
		ChangeInteractionRange();
		//Debug.Log(_playerBehaviour.WasPlayerArmed);

		*/

		if (_playerBehaviour.WasPlayerArmed == true)
		{
			//_playerBehaviour.ArmPlayer();
		}
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		//Debug.Log(_currentIPickable);
		//Debug.Log(_currentIThrowable);

		//Debug.Log(_currentIThrowable);

		if (_isInteractionObjectLookedAt = Physics.Raycast(
		_playerCameraController.transform.position,
		_playerCameraController.transform.forward,
		out _hitObject,
		_interactionRange,
		~_layersInteractionToIgnore) &&
		_hitObject.collider.CompareTag("Interactable"))
		{

		}
		else
		{
			if (_currentIPickable != null || CurrentIThrowable != null)
			{
		
			}
			else
			{
				_mainInteractionText.text = null;
				_failInteractionText.text = null;
			}
		}

		if (CurrentPickableObject != null)
		{
			if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
			{
				DropPickable();
			}

			if (_inputDevice.GetKeyRightHandWeaponAttack() && CurrentIThrowable != null)
			{
				OnThrowTrowable?.Invoke(_currentIPickable.PickableType);
				//ThrowThrowable();
			}
		}

		if (_isInteractionObjectLookedAt)
		{
			_lookedAtIInteractable = _hitObject.collider.GetComponent<IInteractable>();
			_lookedAtIThrowableObject = _hitObject.collider.GetComponent<IThrowable>();
			_lookedAtIPickable = _hitObject.collider.GetComponent<IPickable>();
			_lookedAtIGainedItem = _hitObject.collider.GetComponent<IGainedItem>();

			if (_lookedAtIInteractable != null)
			{
				GameObject renderer = _hitObject.collider.gameObject;

				if (renderer != null)
				{
					_currentInteractableObject = renderer;

					if (_previousInteractableObject != null && _previousInteractableObject != _currentInteractableObject)
					{
						ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));
					}

					if (Time.timeScale == 1)
					{
						ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Outline"));
					}
				}

				if (_currentInteractableObject != null)
				{
					_mainInteractionText.text = $"{_lookedAtIInteractable.InteractionHintMessageMain}\n{_HUDInteractionMainTextInteract} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}";
				}

				if (_inputDevice.GetKeyInteract())
				{
					_lookedAtIInteractable.Interact();

					if (_lookedAtIInteractable.IsInteractionHintMessageFailActive == true)
					{
						_failInteractionText.text = _lookedAtIInteractable.InteractionHintMessageFail;
						if (_showAdditionalHintCoroutine != null)
							StopCoroutine(_showAdditionalHintCoroutine);

						_showAdditionalHintCoroutine = StartCoroutine(ShowInteractionObjectHintMessage());
					}
					else
					{
						if (_showAdditionalHintCoroutine != null)
						{
							StopCoroutine(_showAdditionalHintCoroutine);
						}

						if (_lookedAtIGainedItem != null)
						{
							ShowGainedItems();

							StartCoroutine(HideGainedItems());
						}
					}

					if (_lookedAtIPickable != null && _lookedAtIThrowableObject == null)
					{
						_playerBehaviour.DisarmPlayer();
					}

					if (_lookedAtIPickable != null && _lookedAtIPickable.IsObjectPickedUp)
					{
						CurrentPickableObject = renderer;

						PickUpPickableObject();
					}
				}
			}
			else
			{
				Debug.LogWarning("Объект с тегом 'Interactable' не содержит интерфейс IInteractable.");
			}
		}
		else
		{
			if (_currentInteractableObject != null)
			{
				if (CurrentIThrowable == null)
				{
					ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Default"));
				}

				if (_showAdditionalHintCoroutine != null)
				{
					StopCoroutine(_showAdditionalHintCoroutine);
					_failInteractionText.text = null;
				}
			}

			_currentInteractableObject = null;
		}
		_previousInteractableObject = _currentInteractableObject;
	}

	IEnumerator ShowInteractionObjectHintMessage()
	{
		yield return new WaitForSeconds(1f);
		_failInteractionText.text = null;
	}

	private void ShowGainedItems()
	{
		if (!_itemsTexts[0].gameObject.activeInHierarchy)
		{
			_itemsTexts[0].gameObject.SetActive(true);
			_itemsTexts[0].text = _lookedAtIInteractable.InteractionObjectNameUI;

			_itemsImages[0].gameObject.SetActive(true);
			if (_lookedAtIGainedItem.IconGainedItem != null)
			{
				_itemsImages[0].sprite = _lookedAtIGainedItem.IconGainedItem;
			}
			else
			{
				_itemsImages[0].sprite = _ImageMissing;
			}
		}
		else if (_itemsTexts[0].gameObject.activeInHierarchy && !_itemsTexts[1].gameObject.activeInHierarchy)
		{
			_itemsTexts[1].gameObject.SetActive(true);
			_itemsTexts[1].text = _itemsTexts[0].text;
			_itemsTexts[0].text = _lookedAtIInteractable.InteractionObjectNameUI;

			_itemsImages[1].gameObject.SetActive(true);
			_itemsImages[1].sprite = _itemsImages[0].sprite;
			if (_lookedAtIGainedItem.IconGainedItem != null)
			{
				_itemsImages[0].sprite = _lookedAtIGainedItem.IconGainedItem;
			}
			else
			{
				_itemsImages[0].sprite = _ImageMissing;
			}
		}
		else if (_itemsTexts[1].gameObject.activeInHierarchy && _itemsTexts[0].gameObject.activeInHierarchy)
		{
			_itemsTexts[2].gameObject.SetActive(true);
			_itemsTexts[2].text = _itemsTexts[1].text;
			_itemsTexts[1].text = _itemsTexts[0].text;
			_itemsTexts[0].text = _lookedAtIInteractable.InteractionObjectNameUI;

			_itemsImages[2].gameObject.SetActive(true);
			_itemsImages[2].sprite = _itemsImages[1].sprite;
			_itemsImages[1].sprite = _itemsImages[0].sprite;
			if (_lookedAtIGainedItem.IconGainedItem != null)
			{
				_itemsImages[0].sprite = _lookedAtIGainedItem.IconGainedItem;
			}
			else
			{
				_itemsImages[0].sprite = _ImageMissing;
			}
		}
		else if (_itemsTexts[2].gameObject.activeInHierarchy &&
				 _itemsTexts[0].gameObject.activeInHierarchy &&
				 _itemsTexts[1].gameObject.activeInHierarchy)
		{
			_itemsTexts[2].text = _itemsTexts[1].text;
			_itemsTexts[1].text = _itemsTexts[0].text;
			_itemsTexts[0].text = _lookedAtIInteractable.InteractionObjectNameUI;

			_itemsImages[2].sprite = _itemsImages[1].sprite;
			_itemsImages[1].sprite = _itemsImages[0].sprite;
			if (_lookedAtIGainedItem.IconGainedItem != null)
			{
				_itemsImages[0].sprite = _lookedAtIGainedItem.IconGainedItem;
			}
			else
			{
				_itemsImages[0].sprite = _ImageMissing;
			}
		}
	}

	IEnumerator HideGainedItems()
	{
		yield return new WaitForSeconds(2f);

		if (_itemsTexts[2].gameObject.activeInHierarchy)
		{
			_itemsTexts[2].text = null;
			_itemsTexts[2].gameObject.SetActive(false);

			_itemsImages[2].sprite = null;
			_itemsImages[2].gameObject.SetActive(false);
		}
		else if (_itemsTexts[1].gameObject.activeInHierarchy)
		{
			_itemsTexts[1].text = null;
			_itemsTexts[1].gameObject.SetActive(false);

			_itemsImages[1].sprite = null;
			_itemsImages[1].gameObject.SetActive(false);
		}
		else if (_itemsTexts[0].gameObject.activeInHierarchy)
		{
			_itemsTexts[0].text = null;
			_itemsTexts[0].gameObject.SetActive(false);

			_itemsImages[0].sprite = null;
			_itemsImages[0].gameObject.SetActive(false);
		}
	}

	public void ChangeLayerRecursively(GameObject obj, int layerIndex)
	{
		// Проверяем ТОЛЬКО текущий объект на попадание в списки исключений
		bool isIgnoredByMask = ((1 << obj.layer) & _layersInteractionToIgnore) != 0;
		bool isNpc = LayerMask.LayerToName(obj.layer) == "NPC";

		if (!isIgnoredByMask && !isNpc)
		{
			obj.layer = layerIndex;
		}

		// Рекурсия по детям работает всегда, независимо от того, был ли покрашен родитель
		foreach (Transform child in obj.transform)
		{
			ChangeLayerRecursively(child.gameObject, layerIndex);
		}
	}

	public void PickUpObjectOnLoadData(GameObject pickableObject)
	{
		CurrentPickableObject = pickableObject;
		PickUpPickableObject();
	}

	public void SaveData(ref GameData data)
	{

	}

	public void LoadData(GameData data)
	{
		if (CurrentPickableObject != null)
		{
			DropPickable();
		}
	}
}