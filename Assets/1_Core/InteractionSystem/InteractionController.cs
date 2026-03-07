using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class InteractionController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameObject canvasHUDInteraction;

	private float interactionRange = 50f; // Диапазон взаимодействия

	private bool _isInitialized = false;

	private LocalizationManager localizationManager;

	public delegate void NonThrowableHandler();
	public event NonThrowableHandler OnPickUpThrowable;
	public event NonThrowableHandler OnPickUpNonThrowable;
	public event NonThrowableHandler OnGetRidOfPickable;

	private string HUDInteraction_MainTextInteract;

	private TextMeshProUGUI mainInteractionText; 
	private TextMeshProUGUI additionalInteractionText;

	private TextMeshProUGUI[] itemsTexts; 
	private Image[] itemsImages; 

	private MenuManager menuManager;

	private Sprite NoItemImageExeption;

	private PlayerCameraController playerCameraController;
	//public GameObject PlayerCameraObject;

	private Coroutine showAdditionalHintCoroutine;

	private PlayerBehaviour playerBehaviour;

	private RaycastHit hitInfo;
	private bool isHit;

	private GameObject previousInteractableObject; // Переменная для хранения предыдущего объекта
	private GameObject currentInteractableObject; // Текущий объект взаимодействия
	public GameObject CurrentPickableObject { get; private set; }
	private GameSceneManager gameSceneManager;
	private GameController gameController;
	public void Initialize(GameController gameController,
		GameSceneManager gameSceneManager,
		IInputDevice inputDevice,
		MenuManager menuManager,
		PlayerCameraController playerCameraController,
		PlayerBehaviour playerBehaviour,
		GameObject canvasHUDInteraction,
		TextMeshProUGUI mainInteractionText,
		TextMeshProUGUI additionalInteractionText,
		TextMeshProUGUI[] itemsTexts, // Передача массива
		Image[] itemsImages // Передача массива
	)
	{
		this.gameController = gameController;
		this.gameSceneManager = gameSceneManager;
		this.inputDevice = inputDevice;
		//this.localizationManager = localizationManager;
		this.playerCameraController = playerCameraController;
		this.playerBehaviour = playerBehaviour;
		this.menuManager = menuManager;
		this.canvasHUDInteraction = canvasHUDInteraction;	

		// Присваиваем новые массивы
		this.itemsTexts = itemsTexts;
		this.itemsImages = itemsImages;

		// Старые параметры
		this.mainInteractionText = mainInteractionText;
		this.additionalInteractionText = additionalInteractionText;

		_isInitialized = true;

		
		//HUDInteraction_MainTextInteract = "bruh";



		this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasHUDInteraction;
		this.gameSceneManager.OnBeginLoadGameplayScene += ShowCanvasHUDInteraction;

		this.menuManager.OnOpenInteractionMenu += ShowCanvasHUDInteraction;
		this.menuManager.OnCloseInteractionMenu += HideCanvasHUDInteraction;

		Debug.Log("InteractionController Initialized");
	}


	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		this.localizationManager = localizationManager;
		HUDInteraction_MainTextInteract = this.localizationManager.GetLocalizedString("HUDInteraction_MainTextInteract");
	}

	private void ShowCanvasHUDInteraction()
	{
		if (!gameController.IsMainMenuOpen)
		canvasHUDInteraction.gameObject.SetActive(true);

		itemsTexts[2].text = null;
		itemsTexts[2].gameObject.SetActive(false);
		itemsImages[2].sprite = null;
		itemsImages[2].gameObject.SetActive(false);
		
		itemsTexts[1].text = null;
		itemsTexts[1].gameObject.SetActive(false);
		itemsImages[1].sprite = null;
		itemsImages[1].gameObject.SetActive(false);
		
		itemsTexts[0].text = null;
		itemsTexts[0].gameObject.SetActive(false);
		itemsImages[0].sprite = null;
		itemsImages[0].gameObject.SetActive(false);
		
	}

	private void HideCanvasHUDInteraction()
	{
		canvasHUDInteraction.gameObject.SetActive(false);
	
	}



	void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;

		if (!menuManager.IsAnyMenuOpened)
		{
			if (playerCameraController.CurrentPlayerCameraStateType == "FirstPerson")
				interactionRange = 2.5f;
			else if (playerCameraController.CurrentPlayerCameraStateType == "ThirdPerson")
				interactionRange = 2f + playerCameraController.PlayerCameraDistanceZ;
		}
		else interactionRange = 0;

		if (mainInteractionText != null)
			mainInteractionText.text = null;

		if (showAdditionalHintCoroutine == null)
			additionalInteractionText.text = null;

		if (playerCameraController != null)
		{
			isHit = Physics.Raycast(playerCameraController.transform.position, playerCameraController.transform.forward, out hitInfo, interactionRange);
		}
		

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
					OnPickUpThrowable?.Invoke();
					mainInteractionText.text = $"Отпустить {inputDevice.GetNameOfKeyInteract()}\nБросить {inputDevice.GetNameOfKeyRightHandWeaponAttack()}";
					ChangeLayerRecursively(previousInteractableObject, LayerMask.NameToLayer("Default"));
				}
				else
				{
					OnPickUpNonThrowable?.Invoke();
					mainInteractionText.text = $"Отпустить на {inputDevice.GetNameOfKeyInteract()}";
					ChangeLayerRecursively(previousInteractableObject, LayerMask.NameToLayer("Default"));
				}

				// При нажатии кнопки освобождаем объект
				if (inputDevice.GetKeyInteract())
				{
					OnGetRidOfPickable?.Invoke();
					pickableObj.DropOffObject();
					CurrentPickableObject = null;
					if (playerBehaviour.WasPlayerArmed == true)
					{
						playerBehaviour.ArmPlayer();
					}
				}

				if (throwableObj != null && inputDevice.GetKeyRightHandWeaponAttack())
				{
					OnGetRidOfPickable?.Invoke();
					throwableObj.ThrowObject();
					CurrentPickableObject = null;
					if (playerBehaviour.WasPlayerArmed == true)
					{
						playerBehaviour.ArmPlayer();
					}
				}

				return; // Завершаем цикл обработки, не реагируя на другие объекты
			}
		}

		// Нормальная обработка объектов
		if (isHit && hitInfo.collider != null && hitInfo.collider.tag == "Interactable")
		{
			var interactableObj = hitInfo.collider.GetComponent<IInteractable>();
			var throwableObj = hitInfo.collider.GetComponent<IThrowable>();
			var pickableObj = hitInfo.collider.GetComponent<IPickable>();
			var gainedObject = hitInfo.collider.GetComponent<IInteractGainedItem>();
			var usedObject = hitInfo.collider.GetComponent<IInteractUsedItem>();

			if (interactableObj != null)
			{
				GameObject renderer = hitInfo.collider.gameObject;

				if (renderer != null)
				{
					// Подсветка текущего объекта
					currentInteractableObject = renderer;

					// Если сменился объект, меняем слои для правильного рендеринга
					if (previousInteractableObject != null && previousInteractableObject != currentInteractableObject)
					{
						ChangeLayerRecursively(previousInteractableObject, LayerMask.NameToLayer("Default"));
					}

					// Применяем новый слой Outline
					ChangeLayerRecursively(currentInteractableObject, LayerMask.NameToLayer("Outline"));
				}

				if (currentInteractableObject != null)
				{
					// Подсказка для взаимодействия
					mainInteractionText.text = $"{interactableObj.InteractionHintMessageMain}\n{HUDInteraction_MainTextInteract} {inputDevice.GetNameOfKeyInteract()}";
					
				}

				// Если это стандартный объект IInteractable, обрабатываем нажатие
				if (inputDevice.GetKeyInteract())
				{
					
					interactableObj.Interact();
				


					if (interactableObj.IsInteractionHintMessageAdditionalActive == true)
					{
						additionalInteractionText.text = interactableObj.InteractionHintMessageAdditional;
						if (showAdditionalHintCoroutine != null)
							StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена

						showAdditionalHintCoroutine = StartCoroutine(ShowHintForSeconds());
					}
					else
					{
						if (showAdditionalHintCoroutine != null)
						{
							StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
						}

						if (gainedObject != null)
						{
							if (!itemsTexts[0].gameObject.activeInHierarchy)
							{
								itemsTexts[0].gameObject.SetActive(true);
								itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								itemsImages[0].gameObject.SetActive(true);
								if (gainedObject.GainedItemImage != null)
								{
									itemsImages[0].sprite = gainedObject.GainedItemImage;
								}
								else
								{
									itemsImages[0].sprite = NoItemImageExeption;
								}
							}
							else if (itemsTexts[0].gameObject.activeInHierarchy && !itemsTexts[1].gameObject.activeInHierarchy)
							{
								itemsTexts[1].gameObject.SetActive(true);
								itemsTexts[1].text = itemsTexts[0].text;
								itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								itemsImages[1].gameObject.SetActive(true);
								itemsImages[1].sprite = itemsImages[0].sprite;
								if (gainedObject.GainedItemImage != null)
								{
									itemsImages[0].sprite = gainedObject.GainedItemImage;
								}
								else
								{
									itemsImages[0].sprite = NoItemImageExeption;
								}
							}
							else if (itemsTexts[1].gameObject.activeInHierarchy && itemsTexts[0].gameObject.activeInHierarchy)
							{
								itemsTexts[2].gameObject.SetActive(true);
								itemsTexts[2].text = itemsTexts[1].text;
								itemsTexts[1].text = itemsTexts[0].text;
								itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								itemsImages[2].gameObject.SetActive(true);
								itemsImages[2].sprite = itemsImages[1].sprite;
								itemsImages[1].sprite = itemsImages[0].sprite;
								if (gainedObject.GainedItemImage != null)
								{
									itemsImages[0].sprite = gainedObject.GainedItemImage;
								}
								else
								{
									itemsImages[0].sprite = NoItemImageExeption;
								}
							}
							else if (itemsTexts[2].gameObject.activeInHierarchy &&
									 itemsTexts[0].gameObject.activeInHierarchy &&
									 itemsTexts[1].gameObject.activeInHierarchy)
							{
								itemsTexts[2].text = itemsTexts[1].text;
								itemsTexts[1].text = itemsTexts[0].text;
								itemsTexts[0].text = interactableObj.InteractionObjectNameUI;

								itemsImages[2].sprite = itemsImages[1].sprite;
								itemsImages[1].sprite = itemsImages[0].sprite;
								if (gainedObject.GainedItemImage != null)
								{
									itemsImages[0].sprite = gainedObject.GainedItemImage;
								}
								else
								{
									itemsImages[0].sprite = NoItemImageExeption;
								}
							}

							StartCoroutine(ShowItemsGained());
						}
					}

					if (pickableObj != null && throwableObj == null)
					{
						playerBehaviour.DisarmPlayer();
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
			if (currentInteractableObject != null)
			{
				ChangeLayerRecursively(currentInteractableObject, LayerMask.NameToLayer("Default"));

				if (showAdditionalHintCoroutine != null)
				{
					StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
					additionalInteractionText.text = null;
				}
			}

			currentInteractableObject = null;
		}

	

		// Помечаем текущий объект как предыдущий
		previousInteractableObject = currentInteractableObject;
	}

	IEnumerator ShowHintForSeconds()
	{
		yield return new WaitForSeconds(1f); // Ждем
		additionalInteractionText.text = null;
	}

	IEnumerator ShowItemsGained()
	{
		yield return new WaitForSeconds(2f);

		// Начинаем проверку с третьего элемента (если активен)
		if (itemsTexts[2].gameObject.activeInHierarchy)
		{
			itemsTexts[2].text = null;
			itemsTexts[2].gameObject.SetActive(false);

			itemsImages[2].sprite = null;
			itemsImages[2].gameObject.SetActive(false);
		}
		else if (itemsTexts[1].gameObject.activeInHierarchy)
		{
			itemsTexts[1].text = null;
			itemsTexts[1].gameObject.SetActive(false);

			itemsImages[1].sprite = null;
			itemsImages[1].gameObject.SetActive(false);
		}
		else if (itemsTexts[0].gameObject.activeInHierarchy)
		{
			itemsTexts[0].text = null;
			itemsTexts[0].gameObject.SetActive(false);

			itemsImages[0].sprite = null;
			itemsImages[0].gameObject.SetActive(false);
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
