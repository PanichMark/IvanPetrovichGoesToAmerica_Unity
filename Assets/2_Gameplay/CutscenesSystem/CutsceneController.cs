using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
//[RequireComponent(typeof(Sign))]

public class CutsceneController : MonoBehaviour, ICutscene
{
	private Animator _playerAnimator1stPerson;
	private PlayerWeaponFirstPersonRenderer _playerWeaponFirstPersonRenderer;
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviourController;
	private GameController _gameController;
	private PlayerCameraController _playerCameraController;
	private GameScenesManager _gameSceneManager;
	private JsonSaveLoadController _saveLoadController;
	private MenuManager _menuManager;
	private GameObject _textCutsceneDialogue;
	private TextMeshProUGUI _textComponentCutsceneDialogue;
	private PlayerMovementController _playerMovementController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _playerWeaponController;
	private PlayerWeaponAnimationController _playerWeaponAnimationController;
	private LocalizationManager _localizationManager;
	private NPCstateMachineController _NPCcontroller;
	private AudioSource _audioSource;
	private PlayableDirector _director;
	private bool _isCutsceneDialogueActorPlayer;
	private GameObject _playerProxy;
	private GameObject _playerCameraProxy;
	private ViewModelMenuCutscene _viewModelMenuCutscene;


	public bool IsCutscenePlaying { get; private set; }
	public bool WasCutscenePlaying { get; private set; }
	private bool _wasCutsceneSkipped;
	private bool _wasCutsceneCanceled;

	private GameObject _currentInspectedWeaponModel;

	private bool _shouldChangeNPCState;
	private bool _shouldInteractWithObjects;
	private bool _isInitialized;

	[Header("Cutscene dialogue data")]
	[SerializeField] private CutsceneDialogueData _cutsceneDialogueData;

	[Header("Disarm Player")]
	[SerializeField] private bool _ShouldDisarmPlayer;

	[Header("Dialogue actors mapping")]
	[SerializeField] private List<CutsceneDialogueLinesRoles> _dialogueActorsMapping = new List<CutsceneDialogueLinesRoles>();

	private Dictionary<LanguagesEnum, List<string>> _localizedCutsceneDialogues = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};

	private int _currentCutsceneDialogueLineIndex;

	[Header("Move player")] [SerializeField] private bool _shouldMovePlayer;
	[SerializeField] private Vector3 _newPlayerPosition;

	[Header("Load scene")] [SerializeField] bool _shouldLoadScene;
	[SerializeField] private GameScenesSystemEnum _sceneToLoadAfterCutscene;

	[Header("NPC state")] [SerializeField] private List<CutsceneDataNPC> _NPCstateChanges = new List<CutsceneDataNPC>();

	[Header("Interaction objects")] [SerializeField] private List<GameObject> _interactionObjects = new List<GameObject>();

	private void Start()
	{
		_playerWeaponFirstPersonRenderer = ServiceLocator.Resolve<PlayerWeaponFirstPersonRenderer>();
		_playerBehaviourController = ServiceLocator.Resolve<PlayerBehaviourController>();
		_viewModelMenuCutscene = ServiceLocator.Resolve<ViewModelMenuCutscene>();
		_playerProxy = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
		_playerWeaponAnimationController = ServiceLocator.Resolve<PlayerWeaponAnimationController>();
		_playerCameraProxy = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>();
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>();
		_gameController = ServiceLocator.Resolve<GameController>();
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>();
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_inputDevice = ServiceLocator.Resolve<IInputDevice>();
		_playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>();
		_saveLoadController = ServiceLocator.Resolve<JsonSaveLoadController>();
		_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>();
		_textCutsceneDialogue = _viewModelMenuCutscene.TextCutsceneDialogue;
		_textComponentCutsceneDialogue = _textCutsceneDialogue.GetComponent<TextMeshProUGUI>();
		_playerAnimator1stPerson = _playerCameraProxy.GetComponent<Animator>();
		_director = GetComponent<PlayableDirector>();

		LoadCutsceneDialoguesTextFiles();

		if (_NPCstateChanges != null && _NPCstateChanges.Count > 0)
		{
			foreach (var data in _NPCstateChanges)
			{
				if (data.npcObject != null && data.npcObject.GetComponent<NPCstateMachineController>() != null)
				{
					_shouldChangeNPCState = true;
					break; 
				}
			}
		}

		if (_interactionObjects != null && _interactionObjects.Count > 0)
		{
			foreach (var data in _interactionObjects)
			{
				if (data != null && data.GetComponent<IInteractable>() != null)
				{
					_shouldInteractWithObjects = true;
					break;
				}
			}
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

		//Debug.Log($"Was: {WasCutscenePlaying}");
		//Debug.Log($"Is:     {IsCutscenePlaying}");
	}

	private void LoadCutsceneDialoguesTextFiles()
	{
		if (_cutsceneDialogueData != null)
		{
			if (_cutsceneDialogueData.CutsceneDialogueFileRussian != null)
			{
				using (var reader = new StringReader(_cutsceneDialogueData.CutsceneDialogueFileRussian.text))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (!string.IsNullOrWhiteSpace(line))
						{
							_localizedCutsceneDialogues[LanguagesEnum.Russian].Add(line.Trim());
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("Russian phrase file is not assigned!");
			}

			if (_cutsceneDialogueData.CutsceneDialogueFileEnglish != null)
			{
				using (var reader = new StringReader(_cutsceneDialogueData.CutsceneDialogueFileEnglish.text))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (!string.IsNullOrWhiteSpace(line))
						{
							_localizedCutsceneDialogues[LanguagesEnum.English].Add(line.Trim());
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("English phrase file is not assigned!");
			}
		}
	}

	public void ShowNextCutsceneDialogueLine()
	{
		var currentLanguage = _localizationManager.CurrentLanguage;
		//Debug.Log("SHHOWWW");
		//Debug.Log(currentLanguage);
		//Debug.Log(_currentCutsceneDialogueLineIndex);
		_textCutsceneDialogue.SetActive(true);
		_textComponentCutsceneDialogue.text = _localizedCutsceneDialogues[currentLanguage][_currentCutsceneDialogueLineIndex];

		_isCutsceneDialogueActorPlayer = false;

		foreach (var role in _dialogueActorsMapping)
		{
			//Debug.Log(role.DialogueStepIndex);
			//Debug.Log(_currentCutsceneDialogueLineIndex + 1);

			if (role.DialogueStepIndex == _currentCutsceneDialogueLineIndex + 1)
			{
				if (role.DialogueLineActor != null)
				{
					_audioSource = role.DialogueLineActor.GetComponent<AudioSource>();
					break;
				}
				else
				{
					_isCutsceneDialogueActorPlayer = true;
					break;
				}
			}
		}

		AudioClip[] currentLanguageVoiceLines = null;

		if (currentLanguage == LanguagesEnum.Russian)
		{
			currentLanguageVoiceLines = _cutsceneDialogueData.CutsceneVoicelinesRussian;
		}
		else
		{
			currentLanguageVoiceLines = _cutsceneDialogueData.CutsceneVoicelinesEnglish;
		}

		if (_isCutsceneDialogueActorPlayer == false)
		{
			_audioSource.PlayOneShot(currentLanguageVoiceLines[_currentCutsceneDialogueLineIndex]);
		}

		_currentCutsceneDialogueLineIndex++;
	}

	public void HideTextCutsceneDialogue()
	{
		_textCutsceneDialogue.SetActive(false);
		_textComponentCutsceneDialogue.text = string.Empty;
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
		_gameController.MakeGameSavable();
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
		Debug.Log("CUTSCENE SKIPPED");
		ExecutePostCutsceneActions();
	}

	private void ExecutePostCutsceneActions()
	{
		if (_playerBehaviourController.WasPlayerArmed)
		{
			_playerBehaviourController.ArmPlayer();
		}

		_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.Normal;
		WasCutscenePlaying = false;	
		IsCutscenePlaying = false;
		CutsceneResumeTime();
		_menuManager.CloseCutsceneMenu();
		_gameController.MakePlayerControllable();
		_gameController.MakeGameSavable();

		_playerCameraStateMachineController.SetPlayerCameraState(_playerCameraController.PreviousPlayerCameraGameplayType);
		
		if (_shouldChangeNPCState)
		{
			foreach (var data in _NPCstateChanges)
			{
				if (data.npcObject != null)
				{
					var controller = data.npcObject.GetComponent<NPCstateMachineController>();
					if (controller != null)
					{
						controller.SetNPCState(data.stateToSet);
						Debug.Log($"Катсцена {gameObject.name} изменила состояние NPC {data.npcObject.name} на {data.stateToSet}.");
					}
				}
			}
		}
		
		if (_shouldMovePlayer)
		{
			_playerMovementController.SetPlayerPosition(_newPlayerPosition);
		}

		if (_shouldLoadScene)
		{
			_gameSceneManager.StartCoroutine(_gameSceneManager.LoadGameplayScene((GameScenesGameplayDataEnum)_sceneToLoadAfterCutscene));
		}

		if (_shouldInteractWithObjects)
		{
			foreach (var data in _interactionObjects)
			{
				if (data != null && data.GetComponent<IInteractable>() != null)
				{
					data.GetComponent<IInteractable>().InteractCutscene();
				}
			}
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

			_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.Normal;
		}	
	}

	public void TriggerCutscene(GameObject inspectedWeapon)
	{
		Debug.Log("CUTSCENE!!!");
		_director.Play();
		_gameController.MakeGameUnsavable();

		_currentCutsceneDialogueLineIndex = 0;

		CutsceneStopTime();

		RebindProxyObjects();

		if (_ShouldDisarmPlayer)
		{
			_playerBehaviourController.DisarmPlayer();
		}

		if (inspectedWeapon == null)
		{
			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.Cutscene);
		}
		else
		{
			_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.UnscaledTime;

			var inspectedWeaponModel = Instantiate(inspectedWeapon);
			WeaponAbstract inspectedWeaponComponent = inspectedWeaponModel.GetComponent<WeaponAbstract>();
			//inspectedWeapon.InstantiateWeaponInspect();

			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);


			inspectedWeaponComponent.SetUpWeaponInspect();


			_playerWeaponFirstPersonRenderer.ShowBothHandsForWeaponInspectionCutscene();
			StartCoroutine(inspectedWeaponComponent.InspectWeaponAnimation());
		}

		_menuManager.OpenCutsceneMenu();

		IsCutscenePlaying = true;
		WasCutscenePlaying = true;
		_gameController.MakePlayerNonControllable();

		Debug.Log($"Cutscene {gameObject.name} was triggered");
	}

	private void ResumeCutscene()
	{
		if (WasCutscenePlaying)
		{
			if (!_wasCutsceneSkipped)
			{
				_director.Resume();
				IsCutscenePlaying = true;
				_gameController.MakePlayerNonControllable();

				_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.UnscaledTime;

				Debug.Log($"Cutscene {gameObject.name} resumed");
			}
		}
	}
}