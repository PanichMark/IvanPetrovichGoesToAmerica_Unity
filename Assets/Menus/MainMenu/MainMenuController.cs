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

	

	void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_renderer = GetComponent<Renderer>();
		pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		if (_renderer != null && defaultMaterial != null)
			_renderer.material = defaultMaterial;


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
		if (_renderer != null && hoverMaterial != null)
			_renderer.material = hoverMaterial;
	}

	void OnMouseExit()
	{
		if (_renderer != null && defaultMaterial != null)
			_renderer.material = defaultMaterial;
	}

	void OnMouseDown()
	{

		StartCoroutine(DestroyAfterLoad());

	}

	

	// Вспомогательная корутина для очистки объекта после завершения основной корутины
	IEnumerator DestroyAfterLoad()
	{
		Debug.Log("START!!!!!!!!!");
		// Сначала делаем объект постоянным, чтобы предотвратить преждевременное разрушение
		DontDestroyOnLoad(gameObject);

		// Начинаем загрузку сцены
		yield return StartCoroutine(gameSceneManager.LoadScene(GameScenesEnum.NEW_SceneTest));

		// Удаляем объект после завершения загрузки
		Destroy(gameObject);
	}
}