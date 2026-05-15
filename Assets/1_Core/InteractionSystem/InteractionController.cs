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
	public delegate void PickableObjectsHandler();
	public event PickableObjectsHandler OnPickUpThrowableObject;
	public event PickableObjectsHandler OnPickUpNonThrowableObject;
	public event PickableObjectsHandler OnGetRidOfPickableObject;

	private string _HUDInteractionMainTextInteract;

	private TextMeshProUGUI _mainInteractionText; 
	private TextMeshProUGUI _additionalInteractionText;

	private TextMeshProUGUI[] _itemsTexts; 
	private Image[] _itemsImages; 

	private MenuManager _menuManager;

	private Sprite _ImageMissing;

	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private Coroutine _showAdditionalHintCoroutine;

	private PlayerBehaviourController _playerBehaviour;

	private IInteractable _lookedAtIInteractable;
	private IPickable _lookedAtIPickable;
	private IThrowable _lookedAtIThrowable;

	private IGainedItem _lookedAtIGainedItem;

	private bool _isLookingAtNewInteractionObject;

	private IPickable _inHandsIPickable;
	private IThrowable _inHandsIThrowable;

	private RaycastHit _hitObject;
	private bool _isInteractionObjectLookedAt;

	private GameObject _previousInteractableObject;
	private GameObject _currentInteractableObject; 
	public GameObject InHandsPickableObject { get; private set; }
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

		_gameController.OnPlayerDeath += ChangeInteractionRange;
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
		_HUDInteractionMainTextInteract = _localizationManager.GetLocalizedString("HUDInteraction_MainTextInteract");
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

	private void HideCanvasHUDInteraction()
	{
		_canvasHUDinteraction.gameObject.SetActive(false);
	}

	private void ChangeInteractionRange()
	{
		//Debug.Log("RANGE");
		//ADD ON THAT FROM INPUT DEVICE!!!
		//	_interactionRange = 2f + _playerCameraController.PlayerCameraDistanceZ;
		if (_menuManager.IsAnyMenuOpened || _gameController.IsPlayerDead || _menuManager.IsCutsceneMenuOpened)
		{
			_interactionRange = 0;
		}
		else
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "FirstPerson")
			{
				_interactionRange = 2.5f;
			}
			else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "ThirdPerson")
			{
				_interactionRange = 2f + _playerCameraController.PlayerCameraDistanceZ;
			}
		}
	}

	private void PickUpInteractableObject()
	{
		_inHandsIPickable = InHandsPickableObject.GetComponent<IPickable>();
		_inHandsIThrowable = InHandsPickableObject.GetComponent<IThrowable>();
			
		if (_inHandsIThrowable != null)
		{
			OnPickUpThrowableObject?.Invoke();
			_mainInteractionText.text = $"Отпустить {_inputDevice.GetNameOfKeyInteract()}\nБросить {_inputDevice.GetNameOfKeyRightHandWeaponAttack()}";
			ChangeLayerRecursively(InHandsPickableObject, LayerMask.NameToLayer("Default"));
		}
		else
		{
			OnPickUpNonThrowableObject?.Invoke();
			_mainInteractionText.text = $"Отпустить на {_inputDevice.GetNameOfKeyInteract()}";
			ChangeLayerRecursively(InHandsPickableObject, LayerMask.NameToLayer("Default"));
			_playerBehaviour.DisarmPlayer();
		}

		if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
		{
			OnGetRidOfPickableObject?.Invoke();
			_inHandsIPickable.DropOffObject();
			InHandsPickableObject = null;
			if (_playerBehaviour.WasPlayerArmed == true)
			{
				_playerBehaviour.ArmPlayer();
			}
		}

		if (_inHandsIThrowable != null && _inputDevice.GetKeyRightHandWeaponAttack())
		{
			OnGetRidOfPickableObject?.Invoke();
			_inHandsIThrowable.ThrowObject();
			InHandsPickableObject = null;
			if (_playerBehaviour.WasPlayerArmed == true)
			{
				_playerBehaviour.ArmPlayer();
			}
		}
	}

	void Update()
	{
		if (!_isInitialized)
			return;

		if (_isInteractionObjectLookedAt = Physics.Raycast(_playerCameraController.transform.position, _playerCameraController.transform.forward, out _hitObject, _interactionRange))
		{

		}
		else
		{
			_mainInteractionText.text = null;
			_additionalInteractionText.text = null;
		}

		if (_isInteractionObjectLookedAt && _hitObject.collider.tag == "Interactable")
		{
			_lookedAtIInteractable = _hitObject.collider.GetComponent<IInteractable>();
			_lookedAtIThrowable = _hitObject.collider.GetComponent<IThrowable>();
			_lookedAtIPickable = _hitObject.collider.GetComponent<IPickable>();
			_lookedAtIGainedItem = _hitObject.collider.GetComponent<IGainedItem>();

			if (_lookedAtIInteractable != null)
			{
				_currentInteractableObject = _hitObject.collider.gameObject;
				_previousInteractableObject = _currentInteractableObject;

				if (_isLookingAtNewInteractionObject)
				{
					_isLookingAtNewInteractionObject = false;

					ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Outline"));
					_mainInteractionText.text = $"{_lookedAtIInteractable.InteractionHintMessageMain}\n{_HUDInteractionMainTextInteract} {_inputDevice.GetNameOfKeyInteract()}";

					if (_previousInteractableObject != null && _previousInteractableObject != _currentInteractableObject)
					{
						ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));
					}
				}
				else
				{
					_isLookingAtNewInteractionObject = true;

					ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));

					_currentInteractableObject = null;
					_mainInteractionText.text = null;

					if (_showAdditionalHintCoroutine != null)
					{
						StopCoroutine(_showAdditionalHintCoroutine);
						_additionalInteractionText.text = null;
					}
				}

				if (_inputDevice.GetKeyInteract())
				{
					_lookedAtIInteractable.Interact();
					if (_lookedAtIPickable != null)
					{
						InHandsPickableObject = _currentInteractableObject;
						PickUpInteractableObject();
					}
				
					if (_lookedAtIInteractable.IsInteractionHintMessageFailActive == true)
					{
						_additionalInteractionText.text = _lookedAtIInteractable.InteractionHintMessageFail;
						if (_showAdditionalHintCoroutine != null)
							StopCoroutine(_showAdditionalHintCoroutine); 

						_showAdditionalHintCoroutine = StartCoroutine(ShowHintForSeconds());
					}
					else
					{
						if (_showAdditionalHintCoroutine != null)
						{
							StopCoroutine(_showAdditionalHintCoroutine);
						}

						if (_lookedAtIGainedItem != null)
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

							StartCoroutine(ShowItemsGained());
						}
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

			}

			_currentInteractableObject = null;
		}

	}

	IEnumerator ShowHintForSeconds()
	{
		yield return new WaitForSeconds(1f); // Ждем
		_additionalInteractionText.text = null;
	}

	IEnumerator ShowItemsGained()
	{
		yield return new WaitForSeconds(2f);

		// Начинаем проверку с третьего элемента (если активен)
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

	private void ChangeLayerRecursively(GameObject obj, int layerIndex)
	{
		Debug.Log(_currentInteractableObject);
		obj.layer = layerIndex;
		foreach (Transform child in obj.transform)
		{
			ChangeLayerRecursively(child.gameObject, layerIndex);
		}
	}
}