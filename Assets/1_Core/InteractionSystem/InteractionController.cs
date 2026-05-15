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

	private RaycastHit _hitObject;
	private bool _isInteractionObjectLookedAt;

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
		_HUDInteractionMainTextInteract = this._localizationManager.GetLocalizedString("HUDInteraction_MainTextInteract");
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






		////

		// Если у нас есть захваченный объект, запрещаем любое другое взаимодействие
		if (CurrentPickableObject != null)
		{
			var pickableObj = CurrentPickableObject.GetComponent<IPickable>();
			var throwableObj = CurrentPickableObject.GetComponent<IThrowable>();

			if (pickableObj != null && pickableObj.IsObjectPickedUp)
			{
				// Сообщаем, что игрок держит объект
				if (throwableObj != null)
				{
					OnPickUpThrowableObject?.Invoke();
					_mainInteractionText.text = $"Отпустить {_inputDevice.GetNameOfKeyInteract()}\nБросить {_inputDevice.GetNameOfKeyRightHandWeaponAttack()}";
					ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));
				}
				else
				{
					OnPickUpNonThrowableObject?.Invoke();
					_mainInteractionText.text = $"Отпустить на {_inputDevice.GetNameOfKeyInteract()}";
					ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));
				}

				// При нажатии кнопки освобождаем объект
				if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
				{
					OnGetRidOfPickableObject?.Invoke();
					pickableObj.DropOffObject();
					CurrentPickableObject = null;
					if (_playerBehaviour.WasPlayerArmed == true)
					{
						_playerBehaviour.ArmPlayer();
					}
				}

				if (throwableObj != null && _inputDevice.GetKeyRightHandWeaponAttack())
				{
					OnGetRidOfPickableObject?.Invoke();
					throwableObj.ThrowObject();
					CurrentPickableObject = null;
					if (_playerBehaviour.WasPlayerArmed == true)
					{
						_playerBehaviour.ArmPlayer();
					}
				}

				return; // Завершаем цикл обработки, не реагируя на другие объекты
			}
		}

		// Нормальная обработка объектов
		if (_isInteractionObjectLookedAt && _hitObject.collider != null && _hitObject.collider.tag == "Interactable")
		{
			var interactableObj = _hitObject.collider.GetComponent<IInteractable>();
			var throwableObj = _hitObject.collider.GetComponent<IThrowable>();
			var pickableObj = _hitObject.collider.GetComponent<IPickable>();
			var gainedObject = _hitObject.collider.GetComponent<IGainedItem>();
			//var usedObject = hitInfo.collider.GetComponent<IInteractUsedItem>();

			if (interactableObj != null)
			{
				GameObject renderer = _hitObject.collider.gameObject;

				if (renderer != null)
				{
					// Подсветка текущего объекта
					_currentInteractableObject = renderer;

					// Если сменился объект, меняем слои для правильного рендеринга
					if (_previousInteractableObject != null && _previousInteractableObject != _currentInteractableObject)
					{
						ChangeLayerRecursively(_previousInteractableObject, LayerMask.NameToLayer("Default"));
					}

					// Применяем новый слой Outline
					ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Outline"));
				}

				if (_currentInteractableObject != null)
				{
					// Подсказка для взаимодействия
					_mainInteractionText.text = $"{interactableObj.InteractionHintMessageMain}\n{_HUDInteractionMainTextInteract} {_inputDevice.GetNameOfKeyInteract()}";
					
				}

				// Если это стандартный объект IInteractable, обрабатываем нажатие
				if (_inputDevice.GetKeyInteract())
				{
					interactableObj.Interact();
				
					if (interactableObj.IsInteractionHintMessageFailActive == true)
					{
						_additionalInteractionText.text = interactableObj.InteractionHintMessageFail;
						if (_showAdditionalHintCoroutine != null)
							StopCoroutine(_showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена

						_showAdditionalHintCoroutine = StartCoroutine(ShowHintForSeconds());
					}
					else
					{
						if (_showAdditionalHintCoroutine != null)
						{
							StopCoroutine(_showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
						}

						if (gainedObject != null)
						{
							if (!_itemsTexts[0].gameObject.activeInHierarchy)
							{
								_itemsTexts[0].gameObject.SetActive(true);
								_itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								_itemsImages[0].gameObject.SetActive(true);
								if (gainedObject.IconGainedItem != null)
								{
									_itemsImages[0].sprite = gainedObject.IconGainedItem;
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
								_itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								_itemsImages[1].gameObject.SetActive(true);
								_itemsImages[1].sprite = _itemsImages[0].sprite;
								if (gainedObject.IconGainedItem != null)
								{
									_itemsImages[0].sprite = gainedObject.IconGainedItem;
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
								_itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								_itemsImages[2].gameObject.SetActive(true);
								_itemsImages[2].sprite = _itemsImages[1].sprite;
								_itemsImages[1].sprite = _itemsImages[0].sprite;
								if (gainedObject.IconGainedItem != null)
								{
									_itemsImages[0].sprite = gainedObject.IconGainedItem;
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
								_itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								_itemsImages[2].sprite = _itemsImages[1].sprite;
								_itemsImages[1].sprite = _itemsImages[0].sprite;
								if (gainedObject.IconGainedItem != null)
								{
									_itemsImages[0].sprite = gainedObject.IconGainedItem;
								}
								else
								{
									_itemsImages[0].sprite = _ImageMissing;
								}
							}

							StartCoroutine(ShowItemsGained());
						}
					}

					if (pickableObj != null && throwableObj == null)
					{
						_playerBehaviour.DisarmPlayer();
					}

					// Кэшируем захваченный объект, если это IPickable
					if (pickableObj != null && pickableObj.IsObjectPickedUp)
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
			// Очистка текущих объектов
			if (_currentInteractableObject != null)
			{
				ChangeLayerRecursively(_currentInteractableObject, LayerMask.NameToLayer("Default"));

				if (_showAdditionalHintCoroutine != null)
				{
					StopCoroutine(_showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
					_additionalInteractionText.text = null;
				}
			}

			_currentInteractableObject = null;
		}

		// Помечаем текущий объект как предыдущий
		_previousInteractableObject = _currentInteractableObject;
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
		obj.layer = layerIndex;
		foreach (Transform child in obj.transform)
		{
			ChangeLayerRecursively(child.gameObject, layerIndex);
		}
	}
}