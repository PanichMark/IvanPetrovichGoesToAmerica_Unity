using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour
{
	private IInputDevice inputDevice;
	private IGameController gameController;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject weaponSystem;
	[SerializeField] private GameObject weaponWheelCanvas;
	[SerializeField] private GameObject canvasImagesSubMenu;
	[SerializeField] private GameObject menuManagerGameobject;
	private MenuManager menuManager;
	[SerializeField] private GameObject PauseMenuControllerGameObject;
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController movementController;
	private PlayerCapsuleCollider playerCollider;
	[SerializeField] private GameObject PauseMenuCanvas;
	private PlayerCameraController cameraController;
	private PlayerCameraBlurFilter cameraBlurFilter;
	private ImagesSubMenuController imagesSubMenuController;

	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;

	private PlayerCameraFirstPersonRender firstPersonRender;
	private GameObject PlayerFirstPersonHandRight;
	private GameObject PlayerFirstPersonHandLeft;
	private GameObject PlayerHeadParent;
	private GameObject PlayerHandRightParent;
	private GameObject PlayerHandLeftParent;

	 private InteractionController interactionController;
	private PlayerAnimationController playerAnimationController;
	[SerializeField] private GameObject interactionControllerGameObject;
	[SerializeField] private GameObject interactionCanvas;
	private TextMeshProUGUI mainInteractionText;
	private TextMeshProUGUI additionalInteractionText;
	private Button ExitInteraction;
	private TextMeshProUGUI ReadableText;
	private Image BackgroundBlack;

	private TextMeshProUGUI Item1Text;
	private TextMeshProUGUI Item2Text;
	private TextMeshProUGUI Item3Text;

	private Image Item1Image;
	private Image Item2Image;
	private Image Item3Image;

	private PauseMenuController PauseMenuController;
	private Image ImageNewspaper;

	private GameObject wheelSegmentPrefab;          // Префаб сегмента
	private Transform centerPoint;                  // Центр круга
	private TextMeshProUGUI WeaponText;             // Текущий выбор оружия
	private TextMeshProUGUI WeaponWheelName;        // Название меню (левая/правая рука)
													// Метод для рекурсивного поиска объекта по имени


	private GameObject ButtonImagesSubMenu;

	
	private GameObject FindDeepChildByName(GameObject root, string targetName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(root.transform);

		while (queue.Count > 0)
		{
			Transform current = queue.Dequeue();
			if (current.name == targetName)
				return current.gameObject;

			foreach (Transform child in current)
			{
				queue.Enqueue(child);
			}
		}

		return null;
	}
	// Простая обычная инициализация без корутин
	private IEnumerator SequentialInitialization()
	{
		gameController = new GameController();

		// Создание устройства ввода
		inputDevice = new InputKeyboard(gameController);
		//inputDevice = new InputController();
		weaponWheelCanvas = Instantiate(weaponWheelCanvas);
		interactionCanvas = Instantiate(interactionCanvas);
		PauseMenuCanvas = Instantiate(PauseMenuCanvas);
	
		canvasImagesSubMenu = Instantiate(canvasImagesSubMenu);
	
		ButtonImagesSubMenu = FindDeepChildByName(PauseMenuCanvas, "PauseMenu Images Button");
	

		// Загрузка ресурсов
		wheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = weaponWheelCanvas.transform.Find("Centre")?.transform;
		WeaponText = weaponWheelCanvas.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		WeaponWheelName = weaponWheelCanvas.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();


		mainInteractionText = interactionCanvas.transform.Find("mainInteractionText")?.GetComponent<TextMeshProUGUI>();
		additionalInteractionText = interactionCanvas.transform.Find("additionalInteractionText")?.GetComponent<TextMeshProUGUI>();

		Item1Text = interactionCanvas.transform.Find("Item1text")?.GetComponent<TextMeshProUGUI>();
		Item2Text = interactionCanvas.transform.Find("Item2text")?.GetComponent<TextMeshProUGUI>();
		Item3Text = interactionCanvas.transform.Find("Item3text")?.GetComponent<TextMeshProUGUI>();

		Item1Image = interactionCanvas.transform.Find("Image1Icon")?.GetComponent<Image>();
		Item2Image = interactionCanvas.transform.Find("Image2Icon")?.GetComponent<Image>();
		Item3Image = interactionCanvas.transform.Find("Image3Icon")?.GetComponent<Image>();

		ExitInteraction = interactionCanvas.transform.Find("ExitInteraction")?.GetComponent<Button>();
		
		ImageNewspaper = interactionCanvas.transform.Find("ReadableImage")?.GetComponent<Image>();
		ReadableText = interactionCanvas.transform.Find("ReadableText")?.GetComponent<TextMeshProUGUI>();
		BackgroundBlack = interactionCanvas.transform.Find("BackgroundBlack")?.GetComponent<Image>();



		// ИНСТАНТИРУЕМ ОБЪЕКТЫ ПЕРЕД ВСЕМИ ОПЕРАЦИЯМИ
		player = Instantiate(player);
		playerCamera = Instantiate(playerCamera);
		weaponSystem = Instantiate(weaponSystem);
		PauseMenuControllerGameObject = Instantiate(PauseMenuControllerGameObject);
		interactionControllerGameObject = Instantiate(interactionControllerGameObject);
		menuManagerGameobject = Instantiate(menuManagerGameobject);


		// НАХОДИМ НУЖНЫЕ GAMEOBJECTS ПО ИМЕНАМ
		PlayerFirstPersonHandRight = FindDeepChildByName(playerCamera, "UNITY HandRight");
		PlayerFirstPersonHandLeft = FindDeepChildByName(playerCamera, "UNITY  HandLeft");
		PlayerHeadParent = FindDeepChildByName(player, "UNITY PlayerHead");
		PlayerHandRightParent = FindDeepChildByName(player, "UNITY HandRight");
		PlayerHandLeftParent = FindDeepChildByName(player, "UNITY  HandLeft");

		// Получить компоненты ПОСЛЕ инстанцирования
		playerBehaviour = player.GetComponent<PlayerBehaviour>();
		movementController = player.GetComponent<PlayerMovementController>();
		playerCollider = player.GetComponentInChildren<PlayerCapsuleCollider>();
		cameraController = playerCamera.GetComponent<PlayerCameraController>();
		cameraBlurFilter = playerCamera.GetComponent<PlayerCameraBlurFilter>();
		weaponController = weaponSystem.GetComponent<WeaponController>();
		weaponWheelController = weaponSystem.GetComponent<WeaponWheelController>();
		playerAnimationController = player.GetComponent<PlayerAnimationController>();
		firstPersonRender = playerCamera.GetComponent<PlayerCameraFirstPersonRender>();
		imagesSubMenuController = PauseMenuControllerGameObject.GetComponent<ImagesSubMenuController>();
		PauseMenuController = PauseMenuControllerGameObject.GetComponent<PauseMenuController>();
		interactionController = interactionControllerGameObject.GetComponent<InteractionController>();
		menuManager = menuManagerGameobject.GetComponent<MenuManager>();
		// ИНЦИАЛИЗАЦИЯ КОМПОНЕНТОВ
		menuManager.Initialize(inputDevice, gameController);
		PauseMenuController.Initialize(inputDevice, menuManager, PauseMenuCanvas, ButtonImagesSubMenu);
		imagesSubMenuController.Initialize(inputDevice, menuManager, PauseMenuController, canvasImagesSubMenu);
		playerBehaviour.Initialize(inputDevice);
		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);
		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, player);
		cameraBlurFilter.Initialize(menuManager);
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, wheelSegmentPrefab, centerPoint, weaponWheelCanvas, WeaponText, WeaponWheelName);

		firstPersonRender.Initialize(cameraController, weaponController,
							 PlayerFirstPersonHandRight, PlayerFirstPersonHandLeft,
							 PlayerHeadParent, PlayerHandRightParent, PlayerHandLeftParent);
		playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);
		interactionController.Initialize(
	inputDevice,
	cameraController,
	playerBehaviour,
	mainInteractionText,
	additionalInteractionText,
	Item1Text,
	Item2Text,
	Item3Text,
	Item1Image,
	Item2Image,
	Item3Image
);

		// Поднимаем флаг только после завершения всех шагов

		// Зарегистрировали контроллер оружия в Service Locator
	    ServiceLocator.Register("Player", player);
        ServiceLocator.Register("MenuManager", menuManager);
        ServiceLocator.Register("WeaponController", weaponController);
        ServiceLocator.Register("ExitInteraction", ExitInteraction);
        ServiceLocator.Register("ImageNewspaper", ImageNewspaper);
		ServiceLocator.Register("ReadableText", ReadableText);
		ServiceLocator.Register("BackgroundBlack", BackgroundBlack);
		//Debug.Log(BackgroundBlack);

		//yield return null;
		yield return StartCoroutine(LoadNextScene());

		Debug.Log("Все компоненты успешно инициализированы!");
	}
	private IEnumerator LoadNextScene()
	{
		// Сразу подгружаем New_SceneTest поверх Bootstrap
		//var operation = SceneManager.LoadSceneAsync("NEW_SceneTest", LoadSceneMode.Additive);

		//while (!operation.isDone)
			yield return null;

		Debug.Log("Дополнительная сцена загружена!");
	}



	// Main entry point
	private void Awake()
	{
		StartCoroutine(SequentialInitialization()); // Используем StartCoroutine!
		
	}

	private void OnApplicationQuit()
	{
		ServiceLocator.ClearServices();
	}
}
