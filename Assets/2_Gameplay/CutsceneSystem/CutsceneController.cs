using System.Collections.Generic;
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
	[Header("Cutscene")][SerializeField] bool ShouldMovePlayer;
	[SerializeField] Vector3 newPlayerPosition;

	[Header("Cutscene")][SerializeField] bool shouldLoadScene;
	[SerializeField] private GameScenesEnum sceneToLoadAfterCutscene;

	[Header("NPC Settings")]
	[SerializeField] private List<CutsceneDataNPC> npcStateChanges = new List<CutsceneDataNPC>();

	private bool WasCutsceneSkipped;

	private GameController gameController;
	[Header("Weapon")] [SerializeField] private GameObject weaponPrefabToUnlock;
	public bool IsCutscenePlaying { get; private set; }
	// --- ФЛАГИ ДЛЯ ЛОГИКИ (Задаются в Start) ---
	// По умолчанию все действия выключены.
	private bool shouldChangeNPCState = false;
	private bool shouldUnlockWeapon = false;

	private PlayerMovementController playerMovementController;
	// Полученный компонент NPC.
	private NPCStateMachineController npcController;

	// --- КОМПОНЕНТЫ И СОСТОЯНИЯ ---
	private PlayableDirector director;
//	private bool isPausedByMenu = false;

	// Ссылки на реальные объекты из Bootstrap-сцены.
	private GameObject RealPlayer;
	private GameObject MainCamera;


	// --- ИНИЦИАЛИЗАЦИЯ В START ---
	private void Start()
	{
		RealPlayer = ServiceLocator.Resolve<GameObject>("Player");
		MainCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		// Получаем все зависимости (без проверок на null)
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
		playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");

		director = GetComponent<PlayableDirector>();

		// --- ПРОВЕРКА ПОЛЕЙ НА NULL И УСТАНОВКА ФЛАГОВ (ТОЛЬКО ЗДЕСЬ!) ---
		// Логика: Если условие выполняется -> флаг = true. Иначе остается false.



		// --- ПРОВЕРКА СПИСКА NPC ---
		// Логика: Если в списке есть хотя бы один валидный элемент -> флаг = true.
		

		if (npcStateChanges != null && npcStateChanges.Count > 0)
		{
			// Проверяем, что хотя бы у одного NPC в списке есть компонент контроллера
			foreach (var data in npcStateChanges)
			{
				if (data.npcObject != null && data.npcObject.GetComponent<NPCStateMachineController>() != null)
				{
					shouldChangeNPCState = true;
					break; // Нашли хотя бы один рабочий вариант, выходим из цикла
				}
			}
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
		saveLoadController.OnSafeFileLoad += StopCutsceneOnLoad;

		RebindProxyObjects();

		Debug.Log($"Катсцена {gameObject.name} инициализирована. Flags: Load[{shouldLoadScene}], NPC[{shouldChangeNPCState}], Weapon[{shouldUnlockWeapon}].");
	}

	private void OnDestroy()
	{
		// Отписываемся от событий, чтобы избежать ошибок
		director.stopped -= OnTimelineStopped;


		menuManager.OnOpenPauseMenu -= PauseCutscene;
		menuManager.OnClosePauseMenu -= ResumeCutscene;
	

		saveLoadController.OnSafeFileLoad -= StopCutsceneOnLoad;
	}

	private void Update()
	{
		if (director.state == PlayState.Playing)
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

		//if (!director.playableGraph.IsPlaying() && !isPausedByMenu)
		//{
		//	director.Play();
		//}

		//Debug.Log($"Катсцена {gameObject.name} привязана к реальным объектам.");
		
	}

	// --- ОБРАБОТЧИКИ СОБЫТИЙ TIMELINE И ЗАГРУЗКИ ---
	private void OnTimelineStopped(PlayableDirector aDirector)
	{
		if (!WasCutsceneSkipped)
		{
			ExecutePostCutsceneActions();
		}
	}

	private void StopCutsceneOnLoad()
	{
		Debug.Log($"Катсцена {gameObject.name} остановлена из-за загрузки сохранения.");
		IsCutscenePlaying = false;	
		SkipCutscene();

		saveLoadController.OnSafeFileLoad -= StopCutsceneOnLoad;
	}

	private void SkipCutscene()
	{
		WasCutsceneSkipped = true;
		director.Stop();
		director.stopped -= OnTimelineStopped;
		//WasCutsceneSkipped = true;
		ExecutePostCutsceneActions();
	}

	// --- ГЛАВНЫЙ МЕТОД: ВЫПОЛНЕНИЕ ДЕЙСТВИЙ ПОСЛЕ КАТСЦЕНЫ ---
	// Этот метод просто проверяет флаги, установленные в Start().
	// Здесь нет проверок на null!
	private void ExecutePostCutsceneActions()
	{
		menuManager.CloseCutsceneMenu();
		gameController.MakePlayerControllable();
		// 1. Загрузка сцены (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)

		// 2. Смена состояния NPC (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
		// 2. СМЕНА СОСТОЯНИЯ ВСЕХ NPC ИЗ СПИСКА
		if (shouldChangeNPCState)
		{
			foreach (var data in npcStateChanges)
			{
				if (data.npcObject != null)
				{
					var controller = data.npcObject.GetComponent<NPCStateMachineController>();
					if (controller != null)
					{
						controller.SetNPCState(data.stateToSet);
						Debug.Log($"Катсцена {gameObject.name} изменила состояние NPC {data.npcObject.name} на {data.stateToSet}.");
					}
				}
			}
		}

		// 3. Разблокировка оружия (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
		if (shouldUnlockWeapon)
		{
			playerWeaponController.UnlockWeapon(weaponPrefabToUnlock);
			//Debug.Log($"Катсцена {gameObject.name} разблокировала оружие: {weaponPrefabToUnlock.name}");
		}
		/*
		Debug.Log(shouldLoadScene);
		Debug.Log(sceneToLoadAfterCutscene);
		Debug.Log(gameSceneManager);
		*/
		if (ShouldMovePlayer)
		{
			playerMovementController.SetPlayerPosition(newPlayerPosition);
		}

		if (shouldLoadScene)
		{
		
			gameSceneManager.StartCoroutine(gameSceneManager.LoadScene(sceneToLoadAfterCutscene));
		}


		Destroy(gameObject);
		Debug.Log("Post-cutscene actions executed.");
	}

	// --- МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ПАУЗОЙ ОТ МЕНЮ ---
	private void PauseCutscene()
	{
		IsCutscenePlaying = false;
		director.Pause();
		//isPausedByMenu = true;
		//gameController.MakePlayerNonControllable();
		Debug.Log($"Катсцена {gameObject.name} поставлена на паузу.");
	}
	public void TriggerCutscene()
	{
		menuManager.OpenCutsceneMenu();
		IsCutscenePlaying = true;
		// Если катсцена уже играет, останавливаем её перед повторным запуском
		if (director.state == PlayState.Playing)
		{
			director.Stop();
		}

		// Привязываем прокси-объекты к реальным игровым объектам
		//RebindProxyObjects();

		// Метод RebindProxyObjects() уже содержит вызов director.Play()
		// Если вы убрали вызов Play() из RebindProxyObjects(), добавьте его здесь:
		director.Play();
		gameController.MakePlayerNonControllable();
		Debug.Log($"Катсцена {gameObject.name} запущена внешним вызовом.");
	}
	private void ResumeCutscene()
	{
		director.Resume();
		IsCutscenePlaying = true;
		gameController.MakePlayerNonControllable();
		//isPausedByMenu = false;
		Debug.Log($"Катсцена {gameObject.name} возобновлена.");

		// Проверка на случай, если катсцена была остановлена вручную во время паузы меню.
		//if (director.playableGraph.IsPlaying())
		//{
			//ExecutePostCutsceneActions();
			//Destroy(gameObject);
		//}
	}
}