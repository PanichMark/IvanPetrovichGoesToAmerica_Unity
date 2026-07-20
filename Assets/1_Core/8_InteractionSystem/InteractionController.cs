using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Codice.Client.BaseCommands.CheckIn.Progress;

public class InteractionController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameObject _canvasHUDinteraction;

	private float _interactionRange = 50f;
	private ViewModelHUDInteraction _viewModelHUDInteraction;
	private LocalizationManager _localizationManager;
	public delegate void PickableObjectsHandler();
	public event PickableObjectsHandler OnPickUpThrowable;
	public event PickableObjectsHandler OnPickUpNonThrowable;
	public event PickableObjectsHandler OnGetRidOfNonThrowable;
	public event PickableObjectsHandler OnGetRidOfThrowable;

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

	private PlayerBehaviourController _playerBehaviour;

	private IInteractable _lookedAtIInteractable;
	private IPickable _lookedAtIPickable;
	private IThrowable _lookedAtIThrowableObject;

	private IGainedItem _lookedAtIGainedItem;

	private IPickable _currentIPickable;
	private IThrowable _currentIThrowable;

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
			_canvasHUDinteraction.gameObject.SetActive(true);

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

	private void PickUpInteractableObject()
	{
		if (CurrentPickableObject != null)
		{
			_currentIPickable = CurrentPickableObject.GetComponent<IPickable>();
			_currentIThrowable = CurrentPickableObject.GetComponent<IThrowable>();

			if (!_changedPickedUpState)
			{
				ChangeInteractionRange();
				_changedPickedUpState = true;
			}

			if (_currentIThrowable != null)
			{
				OnPickUpThrowable?.Invoke();
				_mainInteractionText.text = $"{_HUDInteractionDropText} {_inputDevice.GetNameOfKeyInteract()}\n{_HUDInteractionThrowText} {_inputDevice.GetNameOfKeyRightHandWeaponAttack()}";
				ChangeLayerRecursively(CurrentPickableObject, LayerMask.NameToLayer("Default"));
			}
			else
			{
				OnPickUpNonThrowable?.Invoke();
				_mainInteractionText.text = $"{_HUDInteractionDropText} {_inputDevice.GetNameOfKeyInteract()}";
				ChangeLayerRecursively(CurrentPickableObject, LayerMask.NameToLayer("Default"));
			}

			if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
			{
				
				_currentIPickable.DropOffObject();
				_currentIPickable = null;
				_changedPickedUpState = false;
				CurrentPickableObject = null;
				ChangeInteractionRange();
				//Debug.Log(_playerBehaviour.WasPlayerArmed);
				if (_currentIThrowable == null)
				{
					OnGetRidOfNonThrowable?.Invoke();

					if (_playerBehaviour.WasPlayerArmed == true)
					{
						_playerBehaviour.ArmPlayer();
					}
				}
				else
				{
					OnGetRidOfThrowable?.Invoke();

					if (_playerBehaviour.IsPlayerArmed == true)
					{
						_playerBehaviour.ArmPlayer();
					}
				}
			}

			if (_currentIThrowable != null && _inputDevice.GetKeyRightHandWeaponAttack())
			{
				OnGetRidOfThrowable?.Invoke();
				_currentIThrowable.ThrowObject();
				_currentIPickable = null;
				_currentIThrowable = null;
				_changedPickedUpState = false;
				CurrentPickableObject = null;
				ChangeInteractionRange();
				//Debug.Log(_playerBehaviour.WasPlayerArmed);
				if (_playerBehaviour.WasPlayerArmed == true)
				{
					_playerBehaviour.ArmPlayer();
				}
			}
		}
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_isInteractionObjectLookedAt = Physics.Raycast(_playerCameraController.transform.position, _playerCameraController.transform.forward, out _hitObject, _interactionRange))
		{

		}
		else
		{
			_mainInteractionText.text = null;
			_failInteractionText.text = null;
		}

		PickUpInteractableObject();

		if (_isInteractionObjectLookedAt && _hitObject.collider.tag == "Interactable")
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
					_mainInteractionText.text = $"{_lookedAtIInteractable.InteractionHintMessageMain}\n{_HUDInteractionMainTextInteract} {_inputDevice.GetNameOfKeyInteract()}";

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
				ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Default"));

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
		//Debug.Log("CALL!!!");
		obj.layer = layerIndex;
		foreach (Transform child in obj.transform)
		{
			ChangeLayerRecursively(child.gameObject, layerIndex);
			//Debug.Log(obj);
		}
			//Debug.Log(layerIndex);
	}
}