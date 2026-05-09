using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubSystemPlayerSystems
{
	private IInputDevice inputDevice;
	private GameSceneManager gameSceneManager;
	private Bootstrap bootstrap;
	private GameObject playerGameObject;
	private GameObject playerMainCameraGameObject;
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;


	private GameObject playerHeadParent;
	private GameObject playerColliderGameObject;
	public GameObject playerHandRightParent {  get; private set; }
	public GameObject playerHandLeftParent { get; private set; }
	public GameObject playerFirstPersonHandRight {  get; private set; }
	public GameObject playerFirstPersonHandLeft {  get; private set; }


	// Игрок системы
	public PlayerBehaviour playerBehaviour {  get; private set; }
	public PlayerMovementController playerMovementController { get; private set; }
	private PlayerCapsuleCollider playerColliderController;
	private PlayerAnimationController playerAnimationController;
	// Игрок камера

	public PlayerCameraController playerCameraController {  get; private set; }
	private PlayerCameraBlurFilter playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender playerCameraFirstPersonRender;

	public BootstrapSubSystemPlayerSystems(IInputDevice inputDevice,
	GameSceneManager gameSceneManager,
	Bootstrap bootstrap,
	GameObject playerGameObject,
	GameObject playerMainCameraGameObject,
	BootstrapSubSystemMenu bootstrapSubSystemMenu)
	{
		this.inputDevice = inputDevice;
		this.gameSceneManager = gameSceneManager;
		this.bootstrap = bootstrap;
		this.playerGameObject = playerGameObject;
		this.playerMainCameraGameObject = playerMainCameraGameObject;
		this.bootstrapSubSystemMenu = bootstrapSubSystemMenu;
	}


	public IEnumerator InitializePlayerSystems()
	{
		//loadingStatusText.text = "Player Systems";

		playerColliderGameObject = bootstrap.FindDeepChildByName(playerGameObject, "Collider");

		// Получение компонентов игрока
		playerBehaviour = playerGameObject.GetComponent<PlayerBehaviour>();
		playerMovementController = playerGameObject.GetComponent<PlayerMovementController>();
		playerColliderController = playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
		playerAnimationController = playerGameObject.GetComponent<PlayerAnimationController>();

		// Компоненты камеры игрока
		playerCameraController = playerMainCameraGameObject.GetComponent<PlayerCameraController>();
		playerCameraBlurFilter = playerMainCameraGameObject.GetComponent<PlayerCameraBlurFilter>();
		playerCameraFirstPersonRender = playerMainCameraGameObject.GetComponent<PlayerCameraFirstPersonRender>();

		// Внутренние объекты игрока
		playerFirstPersonHandRight = bootstrap.FindDeepChildByName(playerMainCameraGameObject, "UNITY HandRight");
		playerFirstPersonHandLeft = bootstrap.FindDeepChildByName(playerMainCameraGameObject, "UNITY  HandLeft");
		playerHeadParent = bootstrap.FindDeepChildByName(playerGameObject, "UNITY PlayerHead");
		playerHandRightParent = bootstrap.FindDeepChildByName(playerGameObject, "UNITY HandRight");
		playerHandLeftParent = bootstrap.FindDeepChildByName(playerGameObject, "UNITY  HandLeft");

		// Инициализация полученных компонентов
		playerBehaviour.Initialize(inputDevice);
		playerMovementController.Initialize(inputDevice, gameSceneManager, playerBehaviour);
		playerColliderController.Initialize(playerMovementController);
		playerCameraController.Initialize(inputDevice, gameSceneManager, bootstrapSubSystemMenu.menuManager, playerMovementController, playerColliderController, playerGameObject);
		playerCameraBlurFilter.Initialize(bootstrapSubSystemMenu.menuManager);
		playerAnimationController.Initialize(inputDevice, playerGameObject, playerBehaviour, playerMovementController, playerCameraController);
		playerCameraFirstPersonRender.Initialize(playerCameraController, playerHeadParent);


		ServiceLocator.Register("PlayerCameraController", playerCameraController);
		ServiceLocator.Register("PlayerMovementController", playerMovementController);

		ServiceLocator.Register("PlayerCameraBlurFilter", playerCameraBlurFilter);
		ServiceLocator.Register("Player", playerGameObject);
		ServiceLocator.Register("playerMainCameraGameObject", playerMainCameraGameObject);
		ServiceLocator.Register("PlayerBehaviour", playerBehaviour);
		ServiceLocator.Register("playerColliderGameObject", playerColliderGameObject);
		Debug.Log("PLAYER SYSTEMS INITIALIZED");
		yield break;
	}
}
