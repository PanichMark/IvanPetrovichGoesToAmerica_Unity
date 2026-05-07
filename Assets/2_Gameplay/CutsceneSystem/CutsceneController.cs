using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneController : MonoBehaviour
{
	// --- ЗАВИСИМОСТИ (Получаем через Service Locator) ---
	private GameSceneManager gameSceneManager;
	private MenuManager menuManager;
	private IInputDevice inputDevice;
	private PlayerWeaponController playerWeaponController;
	private SaveLoadController saveLoadController;

	// --- НАСТРОЙКИ В ИНСПЕКТОРЕ ---
	[Header("Cutscene")] [SerializeField] private GameScenesEnum sceneToLoadAfterCutscene;

	[Header("NPC")] [SerializeField] private GameObject affectedNPC;
	[SerializeField] private NPCStateTypes stateToSetAfterCutscene;


	[Header("Weapon")] [SerializeField] private GameObject weaponPrefabToUnlock;

	// --- ФЛАГИ ДЛЯ ЛОГИКИ (Задаются в Start) ---
	// По умолчанию все действия выключены.
	private bool shouldLoadScene = false;
	private bool shouldChangeNPCState = false;
	private bool shouldUnlockWeapon = false;

	// Полученный компонент NPC.
	private NPCStateMachineController npcController;

	// --- КОМПОНЕНТЫ И СОСТОЯНИЯ ---
	private PlayableDirector director;
	private bool isPausedByMenu = false;

	// Ссылки на реальные объекты из Bootstrap-сцены.
	private GameObject RealPlayer;
	private GameObject MainCamera;


	// --- ИНИЦИАЛИЗАЦИЯ В START ---
	private void Start()
	{
		RealPlayer = ServiceLocator.Resolve<GameObject>("Player");
		MainCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		// Получаем все зависимости (без проверок на null)
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
		playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");

		director = GetComponent<PlayableDirector>();

		// --- ПРОВЕРКА ПОЛЕЙ НА NULL И УСТАНОВКА ФЛАГОВ (ТОЛЬКО ЗДЕСЬ!) ---
		// Логика: Если условие выполняется -> флаг = true. Иначе остается false.

		// 1. Проверка для загрузки сцены
		if (sceneToLoadAfterCutscene != default(GameScenesEnum))
		{
			shouldLoadScene = true;
		}

		// 2. Проверка для смены состояния NPC
		if (affectedNPC != null)
		{
			npcController = affectedNPC.GetComponent<NPCStateMachineController>();
			if (npcController != null)
			{
				shouldChangeNPCState = true;
			}
			// Если компонент не найден, флаг остается false
		}
		// Если affectedNPC не задан, флаг остается false

		// 3. Проверка для разблокировки оружия
		if (weaponPrefabToUnlock != null)
		{
			shouldUnlockWeapon = true;
		}
		// Если префаб не задан, флаг остается false


		// Подписываемся на события (без проверок на null)
		director.stopped += OnTimelineStopped;
		menuManager.OnOpenPauseMenu += PauseCutscene;
		menuManager.OnClosePauseMenu += ResumeCutscene;
		saveLoadController.OnSafeFileLoad += StopCutsceneOnSaveLoad;

		RebindProxyObjects();

		Debug.Log($"Катсцена {gameObject.name} инициализирована. Flags: Load[{shouldLoadScene}], NPC[{shouldChangeNPCState}], Weapon[{shouldUnlockWeapon}].");
	}

	private void OnDestroy()
	{
		// Отписываемся от событий, чтобы избежать ошибок
		director.stopped -= OnTimelineStopped;


		menuManager.OnOpenPauseMenu -= PauseCutscene;
		menuManager.OnClosePauseMenu -= ResumeCutscene;
	

		saveLoadController.OnSafeFileLoad -= StopCutsceneOnSaveLoad;
	}

	private void Update()
	{
		if (director.state == PlayState.Playing && !isPausedByMenu)
		{
			if (inputDevice.GetKeySkipCutscene())
			{
				SkipCutscene();
			}
			
		
		}
	}

	// --- МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ КАТСЦЕНОЙ ---
	public void RebindProxyObjects()
	{

		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		director.SetGenericBinding(playerProxy, RealPlayer.transform);
		director.SetGenericBinding(cameraProxy, MainCamera.transform);

		if (!director.playableGraph.IsPlaying() && !isPausedByMenu)
		{
			director.Play();
		}

		//Debug.Log($"Катсцена {gameObject.name} привязана к реальным объектам.");
		
	}

	// --- ОБРАБОТЧИКИ СОБЫТИЙ TIMELINE И ЗАГРУЗКИ ---
	private void OnTimelineStopped(PlayableDirector aDirector)
	{
		ExecutePostCutsceneActions();
	}

	private void StopCutsceneOnSaveLoad()
	{
		Debug.Log($"Катсцена {gameObject.name} остановлена из-за загрузки сохранения.");
		SkipCutscene();

		saveLoadController.OnSafeFileLoad -= StopCutsceneOnSaveLoad;
	}

	private void SkipCutscene()
	{
		director.Stop();
		director.stopped -= OnTimelineStopped;

		ExecutePostCutsceneActions();
		Destroy(gameObject);
	}

	// --- ГЛАВНЫЙ МЕТОД: ВЫПОЛНЕНИЕ ДЕЙСТВИЙ ПОСЛЕ КАТСЦЕНЫ ---
	// Этот метод просто проверяет флаги, установленные в Start().
	// Здесь нет проверок на null!
	private void ExecutePostCutsceneActions()
	{
		// 1. Загрузка сцены (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
		if (shouldLoadScene)
		{
			gameSceneManager.LoadScene(sceneToLoadAfterCutscene);
		}

		// 2. Смена состояния NPC (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
		if (shouldChangeNPCState)
		{
			npcController.SetNPCState(stateToSetAfterCutscene);
		}

		// 3. Разблокировка оружия (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
		if (shouldUnlockWeapon)
		{
			playerWeaponController.UnlockWeapon(weaponPrefabToUnlock);
			//Debug.Log($"Катсцена {gameObject.name} разблокировала оружие: {weaponPrefabToUnlock.name}");
		}

		Debug.Log("Post-cutscene actions executed.");
	}

	// --- МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ПАУЗОЙ ОТ МЕНЮ ---
	private void PauseCutscene()
	{
		director.Pause();
		isPausedByMenu = true;
		Debug.Log($"Катсцена {gameObject.name} поставлена на паузу.");
	}

	private void ResumeCutscene()
	{
		director.Resume();
		isPausedByMenu = false;
		Debug.Log($"Катсцена {gameObject.name} возобновлена.");

		// Проверка на случай, если катсцена была остановлена вручную во время паузы меню.
		if (!director.playableGraph.IsPlaying())
		{
			ExecutePostCutsceneActions();
			Destroy(gameObject);
		}
	}
}