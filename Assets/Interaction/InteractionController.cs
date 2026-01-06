using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
	private IInputDevice inputDevice;

	// Конструктор принимает зависимость
	public InteractionController(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
	}

	private float interactionRange = 50f; // Диапазон взаимодействия
	public TextMeshProUGUI mainInteractionText; // Подсказка (назначается вручную через Inspector)
	public TextMeshProUGUI additionalInteractionText;

	public TextMeshProUGUI Item1Text;
	public TextMeshProUGUI Item2Text;
	public TextMeshProUGUI Item3Text;

	public Image Item1Image;
	public Image Item2Image;
	public Image Item3Image;
	private Sprite NoItemImageExeption;

	private PlayerCameraController playerCamera;
	//public GameObject PlayerCameraObject;

	private Coroutine showAdditionalHintCoroutine;

	private PlayerBehaviour playerBehaviour;

	private RaycastHit hitInfo;
	private bool isHit;

	private GameObject previousInteractableObject; // Переменная для хранения предыдущего объекта
	private GameObject currentInteractableObject; // Текущий объект взаимодействия
	public GameObject CurrentPickableObject { get; private set; }

	public void Initialize(IInputDevice inputDevice, PlayerCameraController playerCameraController, PlayerBehaviour playerBehaviour)
	{
		this.inputDevice = inputDevice;
		playerCamera = playerCameraController;
		this.playerBehaviour = playerBehaviour;

		// Подписываемся на события игрока

		Debug.Log("InteractionController Initialized");
	}

	void Start()
	{
		//playerCamera = PlayerCameraObject.GetComponent<PlayerCameraController>();
		//playerBehaviour = GetComponent<PlayerBehaviour>();
		//additionalInteractionText.gameObject.SetActive(false);
	}

	void Update()
	{
		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
			interactionRange = 2.5f;
		else if (playerCamera.CurrentPlayerCameraStateType == "ThirdPerson")
			interactionRange = 2f + playerCamera.PlayerCameraDistanceZ;

		if (mainInteractionText != null)
			mainInteractionText.text = null;

		if (showAdditionalHintCoroutine == null)
			additionalInteractionText.text = null;

		if (playerCamera != null)
		{
			isHit = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, interactionRange);
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
					mainInteractionText.text = $"Отпустить {inputDevice.GetNameOfKeyInteract()}\nБросить {inputDevice.GetNameOfKeyLeftHandWeaponAttack()}";
				}
				else
				{
					mainInteractionText.text = $"Отпустить на {inputDevice.GetNameOfKeyInteract()}";
				}

				// При нажатии кнопки освобождаем объект
				if (inputDevice.GetKeyInteract())
				{
					pickableObj.DropOffObject();
					CurrentPickableObject = null;
					if (playerBehaviour.WasPlayerArmed == true)
					{
						playerBehaviour.ArmPlayer();
					}
				}

				if (throwableObj != null && inputDevice.GetKeyLeftHandWeaponAttack())
				{
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
					mainInteractionText.text = $"{interactableObj.MainInteractionHint}\nНажмите {inputDevice.GetNameOfKeyInteract()}";
				}

				// Если это стандартный объект IInteractable, обрабатываем нажатие
				if (inputDevice.GetKeyInteract())
				{
					
					interactableObj.Interact();
				


					if (interactableObj.IsAdditionalInteractionHintActive == true)
					{
						additionalInteractionText.text = interactableObj.AdditionalInteractionHint;
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
							if (Item1Text.gameObject.activeInHierarchy == false)
							{
								Item1Text.gameObject.SetActive(true);
								Item1Text.text = interactableObj.InteractionObjectNameUI;

								Item1Image.gameObject.SetActive(true);
								if (gainedObject.GainedItemImage != null)
								{
									Item1Image.sprite = gainedObject.GainedItemImage;
								}
								else
								{
									Item1Image.sprite = NoItemImageExeption;
								}
							}
							else if (Item1Text.gameObject.activeInHierarchy == true && Item2Text.gameObject.activeInHierarchy == false)
							{
								Item2Text.gameObject.SetActive(true);
								Item2Text.text = Item1Text.text;
								Item1Text.text = interactableObj.InteractionObjectNameUI;

								Item2Image.gameObject.SetActive(true);
								Item2Image.sprite = Item1Image.sprite;
								if (gainedObject.GainedItemImage != null)
								{
									Item1Image.sprite = gainedObject.GainedItemImage;
								}
								else
								{
									Item1Image.sprite = NoItemImageExeption;
								}
							}
							else if (Item2Text.gameObject.activeInHierarchy == true && Item1Text.gameObject.activeInHierarchy == true)
							{
								Item3Text.gameObject.SetActive(true);
								Item3Text.text = Item2Text.text;
								Item2Text.text = Item1Text.text;
								Item1Text.text = interactableObj.InteractionObjectNameUI;

								Item3Image.gameObject.SetActive(true);
								Item3Image.sprite = Item2Image.sprite;
								Item2Image.sprite = Item1Image.sprite;
								if (gainedObject.GainedItemImage != null)
								{
									Item1Image.sprite = gainedObject.GainedItemImage;
								}
								else
								{
									Item1Image.sprite = NoItemImageExeption;
								}
							}
							else if (Item3Text.gameObject.activeInHierarchy == true &&
									 Item1Text.gameObject.activeInHierarchy == true &&
									 Item2Text.gameObject.activeInHierarchy == true)
							{
								Item3Text.text = Item2Text.text;
								Item2Text.text = Item1Text.text;
								Item1Text.text = interactableObj.InteractionObjectNameUI;

								Item3Image.sprite = Item2Image.sprite;
								Item2Image.sprite = Item1Image.sprite;
								if (gainedObject.GainedItemImage != null)
								{
									Item1Image.sprite = gainedObject.GainedItemImage;
								}
								else
								{
									Item1Image.sprite = NoItemImageExeption;
								}
							}

							StartCoroutine(ShowItemsGained());
						}
					}

					if (pickableObj != null)
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
		if (Item3Text.gameObject.activeInHierarchy == true)
		{
			Item3Text.text = null;
			Item3Text.gameObject.SetActive(false);
			Item3Image.sprite = null;
			Item3Image.gameObject.SetActive(false);
		}
		else if (Item2Text.gameObject.activeInHierarchy == true)
		{
			Item2Text.text = null;
			Item2Text.gameObject.SetActive(false);
			Item2Image.sprite = null;
			Item2Image.gameObject.SetActive(false);
		}
		else if (Item1Text.gameObject.activeInHierarchy == true)
		{
			Item1Text.text = null;
			Item1Text.gameObject.SetActive(false);
			Item1Image.sprite = null;
			Item1Image.gameObject.SetActive(false);
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