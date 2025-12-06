using TMPro;
using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
	private float interactionRange = 50f; // Диапазон взаимодействия
	public TextMeshProUGUI mainInteractionText; // Подсказка (назначается вручную через Inspector)
	public TextMeshProUGUI additionalInteractionText;

	public TextMeshProUGUI GainItem1;
	public TextMeshProUGUI GainItem2;
	public TextMeshProUGUI GainItem3;

	public PlayerCameraController playerCamera;
	public GameObject PlayerCameraObject;

	private Coroutine showAdditionalHintCoroutine;

	public PlayerBehaviour playerBehaviour;

	private RaycastHit hitInfo;
	private bool isHit;

	private GameObject previousInteractableObject; // Переменная для хранения предыдущего объекта
	private GameObject currentInteractableObject; // Текущий объект взаимодействия
	public GameObject CurrentPickableObject { get; private set; }

	void Start()
	{
		playerCamera = PlayerCameraObject.GetComponent<PlayerCameraController>();
		playerBehaviour = GetComponent<PlayerBehaviour>();
		//additionalInteractionText.gameObject.SetActive(false);
	}

	void Update()
	{
		//Debug.Log(CurrentPickableObject);
		//Debug.Log(mainInteractionText.text);
		//Debug.Log(additionalInteractionText.text);
		//Debug.Log(showAdditionalHintCoroutine);

		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
			interactionRange = 2.5f;
		else if (playerCamera.CurrentPlayerCameraStateType == "ThirdPerson")
			interactionRange = 2f + playerCamera.PlayerCameraDistanceZ;


		//if (mainInteractionText != null)
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
					mainInteractionText.text = $"Отпустить {InputManager.Instance.GetNameOfKeyInteract()}\nБросить {InputManager.Instance.GetNameOfKeyLeftHandWeaponAttack()}";
				}
				else
				{
					mainInteractionText.text = $"Отпустить на {InputManager.Instance.GetNameOfKeyInteract()}";
				}



				// При нажатии кнопки освобождаем объект
				if (InputManager.Instance.GetKeyInteract())
				{
					pickableObj.DropOffObject();
					CurrentPickableObject = null;
					if (playerBehaviour.WasPlayerArmed == true)
					{
						playerBehaviour.ArmPlayer();
					}
				}


				if (throwableObj != null && InputManager.Instance.GetKeyLeftHandWeaponAttack())
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
		if (isHit && hitInfo.collider != null && hitInfo.collider.tag == "Interactable" && !MenuManager.IsAnyMenuOpened)
		{
			var interactableObj = hitInfo.collider.GetComponent<IInteractable>();
			var pickableObj = hitInfo.collider.GetComponent<IPickable>();
			var gainedObject = hitInfo.collider.GetComponent<IInteractGainedItem>();
			var usedObject = hitInfo.collider.GetComponent<IInteractUsedItem>();

			//Debug.Log(pickableObj != null ? pickableObj.IsObjectPickedUp.ToString() : "Нет компонента PickableObjectAbstract");

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
						previousInteractableObject.layer = LayerMask.NameToLayer("Default");
					}

					// Применяем новый слой Outline
					currentInteractableObject.layer = LayerMask.NameToLayer("Outline");
				}

				if (currentInteractableObject != null)
				{
					// Подсказка для взаимодействия
					mainInteractionText.text = $"{interactableObj.MainInteractionHint}\nНажмите {InputManager.Instance.GetNameOfKeyInteract()}";
				}

				// Если это стандартный объект IInteractable, обрабатываем нажатие
				if (InputManager.Instance.GetKeyInteract())
				{
					//Debug.Log("bruh11111111");

					

					interactableObj.Interact();



					
					if (interactableObj.IsAdditionalInteractionHintActive == true)
					{
						additionalInteractionText.text = interactableObj.AdditionalInteractionHint;
						if (showAdditionalHintCoroutine != null)
							StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена

						showAdditionalHintCoroutine = StartCoroutine(ShowHintForSeconds()); // Запускаем новую корутину
					//	Debug.Log("!!!");
					}
					else 
					{
						if (showAdditionalHintCoroutine != null)
						{
							StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
						}
						//additionalInteractionText.gameObject.SetActive(false); // Скрываем подсказку


						if (gainedObject != null)
						{
							
							if (GainItem1.gameObject.activeInHierarchy == false)
							{
								//Debug.Log("111");
								GainItem1.gameObject.SetActive(true);
								GainItem1.text = interactableObj.InteractionObjectNameUI;

							}
							else if (GainItem1.gameObject.activeInHierarchy == true && GainItem2.gameObject.activeInHierarchy == false)
							{
								//Debug.Log("222");


								GainItem2.gameObject.SetActive(true);
								GainItem2.text = GainItem1.text;
								GainItem1.text = interactableObj.InteractionObjectNameUI;

							}
							else if (GainItem2.gameObject.activeInHierarchy == true && GainItem1.gameObject.activeInHierarchy == true)
							{
								//Debug.Log("333");
								GainItem3.gameObject.SetActive(true);
								GainItem3.text = GainItem2.text;
								GainItem2.text = GainItem1.text;
								GainItem1.text = interactableObj.InteractionObjectNameUI;

							}
							else if (GainItem3.gameObject.activeInHierarchy == true && GainItem1.gameObject.activeInHierarchy == true
								&& GainItem1.gameObject.activeInHierarchy == true)
							{
								GainItem3.text = GainItem2.text;
								GainItem2.text = GainItem1.text;
								GainItem1.text = interactableObj.InteractionObjectNameUI;
							}
							//Debug.Log("222");
							StartCoroutine(ShowItemsGained());

						}
					}

					if (pickableObj != null)
					{
						//Debug.Log("bruh222222222");
						playerBehaviour.DisarmPlayer();
					}
					//playerBehaviour.DisarmPlayer();
				}

				// Если это захватываемый объект, добавляем его в кэш
				if (pickableObj != null && pickableObj.IsObjectPickedUp)
				{
					CurrentPickableObject = renderer;
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
				currentInteractableObject.layer = LayerMask.NameToLayer("Default");
				
				
				if (showAdditionalHintCoroutine != null)
				{
					StopCoroutine(showAdditionalHintCoroutine); // Останавливаем предыдущую корутину, если она запущена
					additionalInteractionText.text = null;
					//additionalInteractionText.gameObject.SetActive(false); // Скрываем подсказку
					//Debug.Log("STOOP");
				}
				
			}

			

			currentInteractableObject = null;

			
		}

		// Помечаем текущий объект как предыдущий
		previousInteractableObject = currentInteractableObject;


		
	}

	IEnumerator ShowHintForSeconds()
	{
		
		//Debug.Log("Start");
		//additionalInteractionText.gameObject.SetActive(true); // Показываем подсказку
		yield return new WaitForSeconds(1f); // Ждем
		additionalInteractionText.text = null;
		//Debug.Log("End");

		//additionalInteractionText.gameObject.SetActive(false); // Скрываем подсказку
	}

	IEnumerator ShowItemsGained()
	{
		//Debug.Log("333");

		yield return new WaitForSeconds(2f);
		if (GainItem3.gameObject.activeInHierarchy == true)
		{
			GainItem3.text = null;
			GainItem3.gameObject.SetActive(false);
			//Debug.Log("delete 333");

		}
		else if (GainItem2.gameObject.activeInHierarchy == true)
		{
			GainItem2.text = null;
			GainItem2.gameObject.SetActive(false);
			//Debug.Log("delete 222");

		}
		else if (GainItem1.gameObject.activeInHierarchy == true)
		{
			GainItem1.text = null;
			GainItem1.gameObject.SetActive(false);
			//Debug.Log("delete 111");

		}
	}

	/*
	IEnumerator ShowItemsUsed()
	{
		yield return new WaitForSeconds(1f);
	}
	*/
}