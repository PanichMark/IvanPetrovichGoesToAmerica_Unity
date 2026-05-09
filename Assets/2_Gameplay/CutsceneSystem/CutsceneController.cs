using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteAlways]
public class CutsceneController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameController gameController;
	private GameSceneManager gameSceneManager;
	private SaveLoadController saveLoadController;
	private MenuManager menuManager;

	private PlayerMovementController playerMovementController;
	private PlayerCameraController playerCameraController;
	private PlayerWeaponController playerWeaponController;

	private NPCStateMachineController npcController;

	private PlayableDirector director;

	private GameObject RealPlayer;
	private GameObject MainCamera;

	public bool IsCutscenePlaying { get; private set; }
	private bool WasCutsceneSkipped;
	private bool WasCutsceneCanceled;

	private bool shouldChangeNPCState = false;
	private bool shouldUnlockWeapon = false;

	private bool isInitialized;

	[Header("Cutscene")] [SerializeField] bool ShouldMovePlayer;
	[SerializeField] Vector3 newPlayerPosition;

	[Header("Cutscene")] [SerializeField] bool shouldLoadScene;
	[SerializeField] private GameScenesEnum sceneToLoadAfterCutscene;

	[Header("NPC Settings")] [SerializeField] private List<CutsceneDataNPC> npcStateChanges = new List<CutsceneDataNPC>();

	[Header("Weapon")] [SerializeField] private GameObject weaponPrefabToUnlock;
	
	private void Start()
	{
		RealPlayer = ServiceLocator.Resolve<GameObject>("Player");
		MainCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		playerCameraController = ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
		playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		
		director = GetComponent<PlayableDirector>();

		if (npcStateChanges != null && npcStateChanges.Count > 0)
		{
			foreach (var data in npcStateChanges)
			{
				if (data.npcObject != null && data.npcObject.GetComponent<NPCStateMachineController>() != null)
				{
					shouldChangeNPCState = true;
					break; 
				}
			}
		}

		if (weaponPrefabToUnlock != null)
		{
			shouldUnlockWeapon = true;
		}

		gameSceneManager.OnBeginLoadGameplayScene += CancelCutsceneOnLoad;
		gameSceneManager.OnBeginLoadMainMenuScene += CancelCutsceneOnLoad;
		director.stopped += OnTimelineStopped;
		menuManager.OnOpenPauseMenu += PauseCutscene;
		menuManager.OnClosePauseMenu += ResumeCutscene;

		RebindProxyObjects();

		isInitialized = true;

		//Debug.Log($"Cutscene {gameObject.name} initialized");
	}

	private void OnDestroy()
	{
		director.stopped -= OnTimelineStopped;

		menuManager.OnOpenPauseMenu -= PauseCutscene;
		menuManager.OnClosePauseMenu -= ResumeCutscene;

		gameSceneManager.OnBeginLoadGameplayScene -= CancelCutsceneOnLoad;
		gameSceneManager.OnBeginLoadMainMenuScene -= CancelCutsceneOnLoad;
	}


	private void Update()
	{
		if (!isInitialized)
			return;

		if (IsCutscenePlaying && inputDevice.GetKeySkipCutscene())
		{
			SkipCutscene();
		}
	}

	public void RebindProxyObjects()
	{
		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		director.SetGenericBinding(playerProxy, RealPlayer.transform);
		director.SetGenericBinding(cameraProxy, MainCamera.transform);
	}

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

	private void CancelCutsceneOnLoad()
	{
		Debug.Log($"Cutscene {gameObject.name} Cancelled");

		WasCutsceneCanceled = true;
		IsCutscenePlaying = false;
		director.Stop();
		director.stopped -= OnTimelineStopped;
		menuManager.CloseCutsceneMenu();
		Destroy(gameObject);
	}

	private void SkipCutscene()
	{
		WasCutsceneSkipped = true;
	
		ExecutePostCutsceneActions();
	}

	private void ExecutePostCutsceneActions()
	{
		IsCutscenePlaying = false;
		CutsceneResumeTime();
		menuManager.CloseCutsceneMenu();
		gameController.MakePlayerControllable();

		playerCameraController.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		
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

		if (shouldUnlockWeapon)
		{
			playerWeaponController.UnlockWeapon(weaponPrefabToUnlock);
		}
		
		if (ShouldMovePlayer)
		{
			playerMovementController.SetPlayerPosition(newPlayerPosition);
		}

		if (shouldLoadScene)
		{

			gameSceneManager.StartCoroutine(gameSceneManager.LoadGameplayScene(sceneToLoadAfterCutscene));
		}
			
		Destroy(gameObject);
		Debug.Log($"Post-cutscene {gameObject.name} actions executed.");
	}

	private void PauseCutscene()
	{
		if (!WasCutsceneSkipped)
		{
			IsCutscenePlaying = false;
			director.Pause();
			Debug.Log($"Cutscene {gameObject.name} paused");
		}	
	}

	public void TriggerCutscene()
	{
		Debug.Log("CUTSCENE!!!");
		director.Play();

		CutsceneStopTime();

		RebindProxyObjects();

		playerCameraController.SetPlayerCameraState(PlayerCameraStateTypes.Cutscene);
		menuManager.OpenCutsceneMenu();

		IsCutscenePlaying = true;

		gameController.MakePlayerNonControllable();

		Debug.Log($"Cutscene {gameObject.name} was triggered");
	}

	private void ResumeCutscene()
	{
		if (!WasCutsceneSkipped)
		{
			director.Resume();
			IsCutscenePlaying = true;
			gameController.MakePlayerNonControllable();
		
			Debug.Log($"Cutscene {gameObject.name} resumed");

		}
	}
}