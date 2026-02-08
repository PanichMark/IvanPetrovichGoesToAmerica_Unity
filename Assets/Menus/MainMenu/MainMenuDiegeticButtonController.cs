using UnityEngine;
using System.Collections;

public class MainMenuDiegeticButtonController : MonoBehaviour
{
	public Material defaultMaterial; // Материал по умолчанию
	public Material hoverMaterial;   // Материал при наведении (белый)
	private PauseMenuController pauseMenuController;
	private GameController gameController;
	private Renderer _renderer;
	private GameSceneManager gameSceneManager;
	private Collider collider;
	private SaveLoadController saveLoadController;
	private MenuManager menuManager;
	void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_renderer = GetComponent<Renderer>();
		pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");


		collider = GetComponent<Collider>();

		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) && menuManager.PauseMenuLevel.Count == 1)
		{
		pauseMenuController.CloseSubMenu();
		collider.enabled = true;
		//Debug.Log("BRUH!");
		}
		//Debug.Log("Collider is now " + collider.enabled);
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
	
		if (name == "NewGame")
		{
			Debug.Log("START NEW GAME");
			collider.enabled = false;
			gameController.CloseMainMenu();
			StartCoroutine(StartNewGame());
		}
		else if (name == "LoadGame")
		{
			Debug.Log("OPEN LOAD GAME");
			collider.enabled = false;
			pauseMenuController.OpenLoadSubMenu();
		}
		else if (this.name == "ExitGame")
		{
			Debug.Log("EXIT GAME");
			//collider.enabled = false;
			//Application.Quit();
		}
		else if (this.name == "Options")
		{
			Debug.Log("OPEN OPTIONS");
			//collider.enabled = false;
		}
		else if (this.name == "ReadNews")
		{
			Debug.Log("OPEN NEWS");
			//collider.enabled = false;
		}
	}

	

	// Вспомогательная корутина для очистки объекта после завершения основной корутины
	IEnumerator StartNewGame()
	{
		Debug.Log("START!!!!!!!!!");
		// Сначала делаем объект постоянным, чтобы предотвратить преждевременное разрушение
		DontDestroyOnLoad(gameObject);

		// Начинаем загрузку сцены
		yield return StartCoroutine(gameSceneManager.LoadScene(GameScenesEnum.NEW_SceneTest));
		saveLoadController.NewGame();
		// Удаляем объект после завершения загрузки
		Destroy(gameObject);
	}
}