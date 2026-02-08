using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
	public Material defaultMaterial; // Материал по умолчанию
	public Material hoverMaterial;   // Материал при наведении (белый)
	private PauseMenuController pauseMenuController;
	private GameController gameController;
	private Renderer _renderer;
	private GameSceneManager gameSceneManager;
	private Collider collider;
	private SaveLoadController saveLoadController;

	void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_renderer = GetComponent<Renderer>();
		pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");


		collider = GetComponent<Collider>();

		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			//OnClosePauseSubMenu?.Invoke();
		}
		//Debug.Log(gameController.IsPauseMenuAvailable);
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
	
		if (this.name == "NewGame")
		{
			Debug.Log("Началась новая игра.");
			collider.enabled = false;
			StartCoroutine(StartNewGame());
		}
		else if (this.name == "LoadGame")
		{
			Debug.Log("Игра загружается.");
		}
		else if (this.name == "ExitGame")
		{
			Debug.Log("Приложение закрывается.");
			Application.Quit();
		}
		else if (this.name == "Options")
		{
			Debug.Log("Открываются опции.");
		}
		else if (this.name == "ReadNews")
		{
			Debug.Log("Читают новости.");
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