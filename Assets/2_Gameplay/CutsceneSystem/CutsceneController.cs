using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteAlways]
public class CutsceneController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;
	private MenuManager _menuManager;

	private PlayerMovementController _playerMovementController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _playerWeaponController;

	private NPCStateMachineController _NPCcontroller;

	private PlayableDirector _director;

	private GameObject _playerProxy;
	private GameObject _playerCameraProxy;

	public bool IsCutscenePlaying { get; private set; }
	private bool _wasCutsceneSkipped;
	private bool _wasCutsceneCanceled;

	private bool _shouldChangeNPCState = false;
	private bool _shouldUnlockWeapon = false;

	private bool _isInitialized;

	[Header("Cutscene")] [SerializeField] bool ShouldMovePlayer;
	[SerializeField] Vector3 newPlayerPosition;

	[Header("Cutscene")] [SerializeField] bool shouldLoadScene;
	[SerializeField] private GameScenesEnum sceneToLoadAfterCutscene;

	[Header("NPC Settings")] [SerializeField] private List<CutsceneDataNPC> npcStateChanges = new List<CutsceneDataNPC>();

	[Header("Weapon")] [SerializeField] private GameObject weaponPrefabToUnlock;
	
	private void Start()
	{
		_playerProxy = ServiceLocator.Resolve<GameObject>("PlayerGameObject");
		_playerCameraProxy = ServiceLocator.Resolve<GameObject>("PlayerCameraGameObject");
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_inputDevice = ServiceLocator.Resolve<IInputDevice>("InputDevice");
		_playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		
		_director = GetComponent<PlayableDirector>();

		if (npcStateChanges != null && npcStateChanges.Count > 0)
		{
			foreach (var data in npcStateChanges)
			{
				if (data.npcObject != null && data.npcObject.GetComponent<NPCStateMachineController>() != null)
				{
					_shouldChangeNPCState = true;
					break; 
				}
			}
		}

		if (weaponPrefabToUnlock != null)
		{
			_shouldUnlockWeapon = true;
		}

		_gameSceneManager.OnBeginLoadingGameplayScene += CancelCutsceneOnLoad;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CancelCutsceneOnLoad;
		_director.stopped += OnTimelineStopped;
		_menuManager.OnOpenPauseMenu += PauseCutscene;
		_menuManager.OnClosePauseMenu += ResumeCutscene;

		RebindProxyObjects();

		_isInitialized = true;

		//Debug.Log($"Cutscene {gameObject.name} initialized");
	}

	private void OnDestroy()
	{
		_director.stopped -= OnTimelineStopped;

		_menuManager.OnOpenPauseMenu -= PauseCutscene;
		_menuManager.OnClosePauseMenu -= ResumeCutscene;

		_gameSceneManager.OnBeginLoadingGameplayScene -= CancelCutsceneOnLoad;
		_gameSceneManager.OnBeginLoadingMainMenuScene -= CancelCutsceneOnLoad;
	}


	private void Update()
	{
		if (!_isInitialized)
			return;

		if (IsCutscenePlaying && _inputDevice.GetKeySkipCutscene())
		{
			SkipCutscene();
		}
	}

	public void RebindProxyObjects()
	{
		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		_director.SetGenericBinding(playerProxy, _playerProxy.transform);
		_director.SetGenericBinding(cameraProxy, _playerCameraProxy.transform);
	}

	private void OnTimelineStopped(PlayableDirector aDirector)
	{
		if (_wasCutsceneSkipped || _wasCutsceneCanceled)
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

		_wasCutsceneCanceled = true;
		IsCutscenePlaying = false;
		_director.Stop();
		_director.stopped -= OnTimelineStopped;
		_menuManager.CloseCutsceneMenu();
		Destroy(gameObject);
	}

	private void SkipCutscene()
	{
		_wasCutsceneSkipped = true;
	
		ExecutePostCutsceneActions();
	}

	private void ExecutePostCutsceneActions()
	{
		IsCutscenePlaying = false;
		CutsceneResumeTime();
		_menuManager.CloseCutsceneMenu();
		_gameController.MakePlayerControllable();

		_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
		
		if (_shouldChangeNPCState)
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

		if (_shouldUnlockWeapon)
		{
			_playerWeaponController.UnlockWeapon(weaponPrefabToUnlock);
		}
		
		if (ShouldMovePlayer)
		{
			_playerMovementController.SetPlayerPosition(newPlayerPosition);
		}

		if (shouldLoadScene)
		{

			_gameSceneManager.StartCoroutine(_gameSceneManager.LoadGameplayScene(sceneToLoadAfterCutscene));
		}
			
		Destroy(gameObject);
		Debug.Log($"Post-cutscene {gameObject.name} actions executed.");
	}

	private void PauseCutscene()
	{
		if (!_wasCutsceneSkipped)
		{
			IsCutscenePlaying = false;
			_director.Pause();
			Debug.Log($"Cutscene {gameObject.name} paused");
		}	
	}

	public void TriggerCutscene()
	{
		Debug.Log("CUTSCENE!!!");
		_director.Play();

		CutsceneStopTime();

		RebindProxyObjects();

		_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.Cutscene);
		_menuManager.OpenCutsceneMenu();

		IsCutscenePlaying = true;

		_gameController.MakePlayerNonControllable();

		Debug.Log($"Cutscene {gameObject.name} was triggered");
	}

	private void ResumeCutscene()
	{
		if (!_wasCutsceneSkipped)
		{
			_director.Resume();
			IsCutscenePlaying = true;
			_gameController.MakePlayerNonControllable();
		
			Debug.Log($"Cutscene {gameObject.name} resumed");

		}
	}
}