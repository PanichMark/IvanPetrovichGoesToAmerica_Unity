using log4net;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneController : MonoBehaviour
{
	// --- ЗАВИСИМОСТИ (DI) ---
	private GameSceneManager gameSceneManager;
	private NPCStateMachineController npcController;
	private GameObject AffectedNPC;
	private MenuManager menuManager;
	private IInputDevice inputDevice;
	private PlayerWeaponController playerWeaponController; // Новая зависимость
	private SaveLoadController saveLoadController;
	// --- НАСТРОЙКИ ДЕЙСТВИЙ ПОСЛЕ КАТСЦЕНЫ (Настраиваются в Инспекторе) ---
	public GameScenesEnum sceneToLoadAfterCutscene;
	public NPCStateTypes stateToSetAfterCutscene;

	// Новые настройки для разблокировки оружия
	public bool shouldUnlockWeapon = false;
	public GameObject weaponPrefabToUnlock;

	public bool shouldLoadScene = false;
	public bool shouldChangeNPCState = false;

	// --- КОМПОНЕНТЫ И СОСТОЯНИЯ ---
	private PlayableDirector director;
	private float holdTimer = 0f;
	private bool isPausedByMenu = false; // Флаг для паузы меню

	// Ссылки на реальные объекты из Bootstrap-сцены.
	public GameObject RealPlayer { get; set; }
	public Camera RealMainCamera { get; set; }

	// --- МЕТОД ДЛЯ ВНЕДРЕНИЯ ЗАВИСИМОСТЕЙ (DI) ---

	private void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		npcController = AffectedNPC.GetComponent<NPCStateMachineController>();
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
		playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		director = GetComponent<PlayableDirector>();
		director.stopped += OnTimelineStopped;

		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		if (playerProxy != null && cameraProxy != null)
		{
			if (RealPlayer != null && RealMainCamera != null)
			{
				director.SetGenericBinding(playerProxy, RealPlayer.transform);
				director.SetGenericBinding(cameraProxy, RealMainCamera.transform);
			}
		}

		// Подписываемся на глобальное событие загрузки сохранения
		saveLoadController.OnSafeFileLoad += StopCutsceneOnSaveLoad;

		menuManager.OnOpenPauseMenu += PauseCutscene;
		menuManager.OnClosePauseMenu += ResumeCutscene;
	}
	private void OnDestroy()
	{
		director.stopped -= OnTimelineStopped;
		saveLoadController.OnSafeFileLoad -= StopCutsceneOnSaveLoad;

		// Отписываемся от событий меню
	
		menuManager.OnOpenPauseMenu -= PauseCutscene;
		menuManager.OnClosePauseMenu -= ResumeCutscene;

	}

	private void Update()
	{
		// Проверяем, активна ли катсцена и не стоит ли она на паузе из-за меню
		if (director.state == PlayState.Playing && !isPausedByMenu)
		{
			// Используем метод из внедренного интерфейса IInputDevice
			if (inputDevice.GetKeySkipCutscene())
			{
				holdTimer += Time.deltaTime;
				if (holdTimer >= 3f) // Условие удержания клавиши 3 секунды
				{
					SkipCutscene();
				}
			}
			else
			{
				holdTimer = 0f; // Сбрасываем таймер, если клавиша отпущена
			}
		}
	}

	// --- МЕТОД ДЛЯ ПЕРЕБИНДА ПРОКСИ-ОБЪЕКТОВ ---
	public void RebindProxyObjects()
	{
		if (RealPlayer == null || RealMainCamera == null)
		{
			Debug.LogWarning($"Cannot rebind proxies for {gameObject.name}: Real Player or Camera is not set.");
			return;
		}

		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		if (playerProxy != null && cameraProxy != null)
		{
			director.SetGenericBinding(playerProxy, RealPlayer.transform);
			director.SetGenericBinding(cameraProxy, RealMainCamera.transform);

			Debug.Log($"Катсцена {gameObject.name} успешно привязана к реальным Player и Camera.");

			// Запускаем катсцену после привязки, если она еще не играет и не стоит на паузе меню
			if (!director.playableGraph.IsPlaying() && !isPausedByMenu)
			{
				director.Play();
			}
		}
	}

	// --- ОБРАБОТЧИКИ СОБЫТИЙ TIMELINE И ЗАГРУЗКИ ---
	private void OnTimelineStopped(PlayableDirector aDirector)
	{
		if (aDirector == director)
		{
			ExecutePostCutsceneActions();
		}
	}

	private void StopCutsceneOnSaveLoad()
	{
		if (this != null && gameObject.activeInHierarchy && director != null && director.state == PlayState.Playing)
		{
			Debug.Log($"Катсцена {gameObject.name} остановлена из-за загрузки сохранения.");
			SkipCutscene();

			saveLoadController.OnSafeFileLoad -= StopCutsceneOnSaveLoad;
		}
	}

	private void SkipCutscene()
	{
		director.Stop();
		director.stopped -= OnTimelineStopped;

		ExecutePostCutsceneActions();
		Destroy(gameObject);
	}

	// --- ГЛАВНЫЙ МЕТОД: ВЫПОЛНЕНИЕ ДЕЙСТВИЙ ПОСЛЕ КАТСЦЕНЫ ---
	private void ExecutePostCutsceneActions()
	{
		// 1. Загрузка сцены (если включена в настройках)
		if (shouldLoadScene && gameSceneManager != null)
		{
			gameSceneManager.LoadScene(sceneToLoadAfterCutscene);
		}

		// 2. Смена состояния NPC (если включена в настройках)
		if (shouldChangeNPCState && npcController != null)
		{
			npcController.SetNPCState(stateToSetAfterCutscene);
		}

		// 3. Разблокировка оружия (НОВАЯ ЛОГИКА)
		// Проверяем, что эта функция включена и что все зависимости назначены.
		if (shouldUnlockWeapon && playerWeaponController != null && weaponPrefabToUnlock != null)
		{
			// Вызываем метод разблокировки у контроллера оружия.
			playerWeaponController.UnlockWeapon(weaponPrefabToUnlock);
			Debug.Log($"Катсцена {gameObject.name} разблокировала оружие: {weaponPrefabToUnlock.name}");
		}

		Debug.Log("Post-cutscene actions executed.");
	}

	// --- МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ПАУЗОЙ ОТ МЕНЮ ---
	private void PauseCutscene()
	{
		if (director.state == PlayState.Playing)
		{
			director.Pause();
			isPausedByMenu = true;
			Debug.Log($"Катсцена {gameObject.name} поставлена на паузу из-за открытия Pause Menu.");
		}
	}

	private void ResumeCutscene()
	{
		if (director.state == PlayState.Paused)
		{
			director.Resume();
			isPausedByMenu = false;
			Debug.Log($"Катсцена {gameObject.name} возобновлена после закрытия Pause Menu.");

			// Проверка на случай, если катсцена была остановлена вручную во время паузы меню.
			if (!director.playableGraph.IsPlaying())
			{
				ExecutePostCutsceneActions();
				Destroy(gameObject);
			}
		}
	}
}