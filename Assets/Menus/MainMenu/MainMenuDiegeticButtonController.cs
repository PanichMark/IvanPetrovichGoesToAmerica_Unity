using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuDiegeticButtonController : MonoBehaviour
{
	public Material defaultMaterial;     // Материал по умолчанию
	public Material hoverMaterial;      // Материал при наведении (белый)

	private static List<MainMenuDiegeticButtonController> instances = new List<MainMenuDiegeticButtonController>();

	private PauseMenuController pauseMenuController;
	private GameController gameController;
	private Renderer _renderer;
	private GameSceneManager gameSceneManager;
	private Collider collider;
	private SaveLoadController saveLoadController;
	private MenuManager menuManager;

	[SerializeField] private GameObject DiegeticText;

	void Awake()
	{
		// Регистрация текущего экземпляра компонента в списке экземпляров
		instances.Add(this);
	}

	void OnDestroy()
	{
		instances.Remove(this);
		menuManager.OnCloseAnyMenu -= () => DiegeticText.SetActive(true);
	}

	void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_renderer = GetComponent<Renderer>();
		pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		collider = GetComponent<Collider>();
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		pauseMenuController.OnClosePauseSubMenu += EnableAllColliders;

		// Обработчик с проверкой наличия объекта
		menuManager.OnCloseAnyMenu += () => {
			if (DiegeticText != null)
			{
				DiegeticText.SetActive(true);
			}
		};
	}

	

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) && menuManager.PauseMenuLevel.Count == 1)
		{
			menuManager.CloseAnyMenu();
			DiegeticText.SetActive(true);
			pauseMenuController.ClosePauseSubMenu();
		
			//EnableAllColliders();
			//Debug.Log("BRUH!");
		}
	}

	void OnMouseEnter()
	{
		_renderer.material = hoverMaterial;
	}

	void OnMouseExit()
	{
		_renderer.material = defaultMaterial;
	}

	void OnMouseDown()
	{
		// Отключение коллайдеров у всех объектов с данным компонентом

		if (name == "NewGame")
		{
			Debug.Log("START NEW GAME");
			DisableAllColliders();
			gameController.CloseMainMenu();
			StartCoroutine(StartNewGame());

		}
		else if (name == "LoadGame")
		{
			Debug.Log("OPEN LOAD GAME");
			DiegeticText.SetActive(false);
			DisableAllColliders();
			menuManager.OpenAnyMenu();
			pauseMenuController.OpenLoadSubMenu();
		}
		else if (this.name == "ExitGame")
		{
			Debug.Log("EXIT GAME");
			Application.Quit();
		}
		else if (this.name == "Options")
		{
			Debug.Log("OPEN OPTIONS");
			DiegeticText.SetActive(false);
			DisableAllColliders();
			menuManager.OpenAnyMenu();
			pauseMenuController.OpenSettingsSubMenu();
		}
		else if (this.name == "ReadNews")
		{
			Debug.Log("OPEN NEWS");
		}
	}

	// Методы для обработки коллайдеров всех объектов с данным компонентом
	public void EnableAllColliders()
	{
		foreach (var instance in instances)
		{
			var colliderInstance = instance.collider;
				colliderInstance.enabled = true;
		}
	}

	// Методы для обработки коллайдеров всех объектов с данным компонентом
	private void DisableAllColliders()
	{
		foreach (var instance in instances)
		{
			var colliderInstance = instance.collider;
			colliderInstance.enabled = false;
		}
	}

	IEnumerator StartNewGame()
	{
		DontDestroyOnLoad(gameObject);
		yield return StartCoroutine(saveLoadController.NewGame());
		yield return StartCoroutine(gameSceneManager.LoadScene(GameScenesEnum.Scene_0_Test));
		
		Destroy(gameObject);
	}
}