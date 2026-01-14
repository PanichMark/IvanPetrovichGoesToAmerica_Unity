using UnityEngine;
using System.Collections;
using TMPro;

public class BootStrap : MonoBehaviour
{
	private IInputDevice inputDevice;

	[SerializeField] private GameObject player;
	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject weaponSystem;
	[SerializeField] private GameObject weaponWheelCanvas;

	[SerializeField] private MenuManager menuManager;

	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController movementController;
	private PlayerCapluseCollider playerCollider;

	private PlayerCameraController cameraController;
	private PlayerCameraBlurFilter cameraBlurFilter;

	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;

	private PlayerCameraFirstPersonRender firstPersonRender;
	private PlayerAnimationController playerAnimationController;
	private InteractionController interactionController;

	private GameObject wheelSegmentPrefab;           // Префаб сегмента
	private Transform centerPoint;                   // Центр круга
	          // Canvas меню выбора оружия
	private TextMeshProUGUI WeaponText;              // Текущий выбор оружия
	private TextMeshProUGUI WeaponWheelName;         // Название меню (левая/правая рука)




	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
		StartCoroutine(InitializeComponents());
	}

	private IEnumerator InitializeComponents()
	{
		yield return new WaitForSeconds(0.1f);  // Небольшая пауза перед инициализацией

		inputDevice = new InputKeyboard();

		// Инстанцируем игровые объекты игрока и камеры
		var instantiatedPlayer = Instantiate(player);
		var instantiatedCamera = Instantiate(playerCamera);
		var instantiatedWeaponSystem = Instantiate(weaponSystem);
		var instantiatedWeaponWheelCanvas = Instantiate(weaponWheelCanvas);

		wheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = instantiatedWeaponWheelCanvas.transform.Find("Centre")?.transform;
		
		WeaponText = instantiatedWeaponWheelCanvas.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		WeaponWheelName = instantiatedWeaponWheelCanvas.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();


		// Компоненты оружейной системы
		weaponController = instantiatedWeaponSystem.GetComponent<WeaponController>();
		weaponWheelController = instantiatedWeaponSystem.GetComponent<WeaponWheelController>();

		// Теперь получаем компоненты непосредственно на инстанцированных объектах
		playerBehaviour = instantiatedPlayer.GetComponent<PlayerBehaviour>();
		movementController = instantiatedPlayer.GetComponent<PlayerMovementController>();

		// А вот collider находится на дочернем объекте, поэтому сначала найдем его
		var colliderGameObject = instantiatedPlayer.transform.Find("Collider")?.gameObject;
		playerCollider = colliderGameObject?.GetComponent<PlayerCapluseCollider>();

		// Так как камера контроллер и фильтр расположены на камере,
		// можем сразу брать их с инстанцированной камеры
		cameraController = instantiatedCamera.GetComponent<PlayerCameraController>();
		cameraBlurFilter = instantiatedCamera.GetComponent<PlayerCameraBlurFilter>();

		// Инициализация компонентов
		menuManager.Initialize(inputDevice);
		playerBehaviour.Initialize(inputDevice);
		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);

		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, instantiatedPlayer);
		cameraBlurFilter.Initialize(menuManager);

		



		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, wheelSegmentPrefab,
			centerPoint, weaponWheelCanvas, WeaponText, WeaponWheelName);
		//firstPersonRender.Initialize(cameraController, weaponController);
		//playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);
		//interactionController.Initialize(inputDevice, cameraController, playerBehaviour);

		Debug.Log("BootStrap ended!");
		Debug.Log("#########################");

		
	}

	void Start()
	{
	}

	void Update()
	{
	}
}

//firstPersonRender.Initialize(cameraController, weaponController);
//playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);
//interactionController.Initialize(inputDevice, cameraController, playerBehaviour);
