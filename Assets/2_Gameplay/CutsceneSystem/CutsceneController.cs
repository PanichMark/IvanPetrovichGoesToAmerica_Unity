using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[ExecuteAlways]
public class CutsceneController : MonoBehaviour
{
	// --- ЗАВИСИМОСТИ (Получаем через Service Locator) ---
	private GameSceneManager gameSceneManager;
	private MenuManager menuManager;
	private IInputDevice inputDevice;
	private PlayerWeaponController playerWeaponController;
	private SaveLoadController saveLoadController;
	private PlayerCameraController playerCameraController;
	// --- НАСТРОЙКИ В ИНСПЕКТОРЕ ---
	[Header("Cutscene")][SerializeField] bool ShouldMovePlayer;
	[SerializeField] Vector3 newPlayerPosition;

	[Header("Cutscene")][SerializeField] bool shouldLoadScene;
	[SerializeField] private GameScenesEnum sceneToLoadAfterCutscene;

	[Header("NPC Settings")]
	[SerializeField] private List<CutsceneDataNPC> npcStateChanges = new List<CutsceneDataNPC>();

	private bool WasCutsceneSkipped;
	private bool isInitialized;
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

	private bool WasCutsceneCanceled;
	// --- ИНИЦИАЛИЗАЦИЯ В START ---
	private void Start()
	{
		RealPlayer = ServiceLocator.Resolve<GameObject>("Player");
		MainCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		// Получаем все зависимости (без проверок на null)
		playerCameraController = ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");
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
		gameSceneManager.OnBeginLoadGameplayScene += StopCutsceneOnLoad;
		gameSceneManager.OnBeginLoadMainMenuScene += StopCutsceneOnLoad;
		director.stopped += OnTimelineStopped;
		menuManager.OnOpenPauseMenu += PauseCutscene;
		menuManager.OnClosePauseMenu += ResumeCutscene;
		//saveLoadController.OnSafeFileLoad += StopCutsceneOnLoad;

		RebindProxyObjects();
		isInitialized = true;
		Debug.Log($"Катсцена {gameObject.name} инициализирована. Flags: Load[{shouldLoadScene}], NPC[{shouldChangeNPCState}], Weapon[{shouldUnlockWeapon}].");
	}

	private void OnDestroy()
	{
		// Отписываемся от событий, чтобы избежать ошибок
		director.stopped -= OnTimelineStopped;


		menuManager.OnOpenPauseMenu -= PauseCutscene;
		menuManager.OnClosePauseMenu -= ResumeCutscene;

		gameSceneManager.OnBeginLoadGameplayScene -= StopCutsceneOnLoad;
		gameSceneManager.OnBeginLoadMainMenuScene -= StopCutsceneOnLoad;
		//saveLoadController.OnSafeFileLoad -= StopCutsceneOnLoad;
	}

	// --- В вашем классе CutsceneController ---

	private void Update()
	{
		if (!isInitialized)
			return;

		// 1. Проверка на пропуск катсцены (клавиша)
		if (IsCutscenePlaying && inputDevice.GetKeySkipCutscene())
		{
			SkipCutscene();
			//return; // Выходим, чтобы не вызывать Evaluate() на остановленной катсцене
		}

		
			
			//Debug.Log(IsCutscenePlaying);
		
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
		if (WasCutsceneSkipped || WasCutsceneCanceled)
		{
		
		}
		else
		{
			Debug.Log("ON_TIME_STOPPED");
			ExecutePostCutsceneActions();
		}
	}

	private void CutsceneStopTime()
	{
		Time.timeScale = 0f;
		Debug.Log("Cutscene Time = 0");
	}
	private void CutsceneResumeTime()
	{
		Time.timeScale = 1f;
		Debug.Log("Cutscene Time = 1");
	}
	private void StopCutsceneOnLoad()
	{
		
		
			//Time.timeScale = 1f;
			//if (director.playableGraph.IsValid())
			//{
			Debug.Log($"Катсцена {gameObject.name} остановлена из-за загрузки сохранения.");

		
		WasCutsceneCanceled = true;
		IsCutscenePlaying = false;
		director.Stop();
		director.stopped -= OnTimelineStopped;
		//CutsceneResumeTime();
		menuManager.CloseCutsceneMenu();
		//gameController.MakePlayerControllable();
		Destroy(gameObject);
		//saveLoadController.OnSafeFileLoad -= StopCutsceneOnLoad;
		//}

	}

	private void SkipCutscene()
	{
		WasCutsceneSkipped = true;
	
		//WasCutsceneSkipped = true;
		ExecutePostCutsceneActions();
		//Destroy(this);
	}

	// --- ГЛАВНЫЙ МЕТОД: ВЫПОЛНЕНИЕ ДЕЙСТВИЙ ПОСЛЕ КАТСЦЕНЫ ---
	// Этот метод просто проверяет флаги, установленные в Start().
	// Здесь нет проверок на null!
	private void ExecutePostCutsceneActions()
	{
			
			//WasCutsceneSkipped = true;

			IsCutscenePlaying = false;
			CutsceneResumeTime();
			menuManager.CloseCutsceneMenu();
			gameController.MakePlayerControllable();

			// 1. Загрузка сцены (ЕСЛИ ФЛАГ ПОЗВОЛЯЕТ)
			playerCameraController.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		
		
		

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
		if (!WasCutsceneSkipped)
		{
			IsCutscenePlaying = false;
			director.Pause();
			//isPausedByMenu = true;
			//gameController.MakePlayerNonControllable();
			Debug.Log($"Катсцена {gameObject.name} поставлена на паузу.");
		}	
	}
	public void TriggerCutscene()
	{
		Debug.Log("CUTSCENE!!!");
		director.Play();

		// 1. ОСТАНОВКА ВРЕМЕНИ (используем существующий метод)
		CutsceneStopTime();



		// 2. ПОДГОТОВКА И ЗАПУСК ТАЙМЛАЙНА (Это должно произойти в первую очередь)
		// Привязываем объекты и запускаем воспроизведение.
		RebindProxyObjects();

		// 3. ИЗМЕНЕНИЕ СОСТОЯНИЯ ИГРЫ (Теперь, когда катсцена технически запущена)
		playerCameraController.SetPlayerCameraState(PlayerCameraStateTypes.Cutscene);
		menuManager.OpenCutsceneMenu();

		// Флаг включаем после запуска, чтобы Update() начал вызывать Evaluate()
		IsCutscenePlaying = true;

		// Блокировка управления игроком
		gameController.MakePlayerNonControllable();

		Debug.Log($"Катсцена {gameObject.name} запущена внешним вызовом.");
	}
	private void ResumeCutscene()
	{
		if (!WasCutsceneSkipped)
		{
			director.Resume();
			IsCutscenePlaying = true;
			gameController.MakePlayerNonControllable();
			//isPausedByMenu = false;
			Debug.Log($"Катсцена {gameObject.name} возобновлена.");

		}
		// Проверка на случай, если катсцена была остановлена вручную во время паузы меню.
		//if (director.playableGraph.IsPlaying())
		//{
			//ExecutePostCutsceneActions();
			//Destroy(gameObject);
		//}
	}
}