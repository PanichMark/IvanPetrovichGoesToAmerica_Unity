using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//[RequireComponent(typeof(PlayableDirector))]
//[RequireComponent(typeof(Sign))]

public class CutsceneController : MonoBehaviour
{
	//private Animator _playerAnimator1stPerson;
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
	[Header("Cutscene settings")]
	[SerializeField] private bool _showBlackLines = true;
	[SerializeField] private bool _showMissionGoalHint;
	[SerializeField] private GameObject _cutscenePlayerProxy;
	[SerializeField] private GameObject _cutscenePlayerCameraProxy;
	private string _currentActorName;
	private HUDmissionsController _HUDmissionsController;
	private MissionsManager _missionsManager;
	private ViewModelMenuCutscene _viewModelMenuCutscene;

	private GameObject _playerBootstrap;
	private GameObject _playerCameraBootstrap;

	private GameObject _blackLineUp;
	private GameObject _blackLineDown;

	public bool IsCutscenePlaying { get; private set; }
	public bool WasCutscenePlaying { get; private set; }
	private bool _wasCutsceneSkipped;
	private bool _wasCutsceneCanceled;

	private GameObject _currentInspectedWeaponModel;

	private bool _shouldChangeNPCState;
	private bool _shouldInteractWithObjects;
	private bool _isInitialized;
	private Animator _playerProxyAnimator;
	//private Animator _playerBootstrapAnimator;
	//private bool _isPlayerAnimated;
	private GameObject _textSkipCutscene;
	private TextMeshProUGUI _textComponentSkipCutscene;
	private GameObject _circleSkipCutscene;
	private float _holdDuration;
	private bool _isSkipRequested;
	private Image _imageCircleSkipCutscene;

	[Header("Cutscene dialogue data")]
	[SerializeField] private CutsceneDialogueData _cutsceneDialogueData;

	[Header("Animated NPCs")]
	[SerializeField] private List<Animator> _animatedNPCs = new List<Animator>();


	[Header("Disarm Player")]
	[SerializeField] private bool _ShouldDisarmPlayer;

	[Header("Dialogue actors mapping")]
	[SerializeField] private GameObject[] _dialogueActorsMapping;

	private Dictionary<LanguagesEnum, List<string>> _localizedCutsceneDialogues = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};

	private int _currentCutsceneDialogueLineIndex;

	[Header("Move player")]
	[SerializeField] private CutsceneMovePlayerData _newPlayerTransform;

	[Header("Load scene")] [SerializeField] bool _shouldLoadScene;
	[SerializeField] private GameScenesGameplayEnum _sceneToLoadAfterCutscene;

	[Header("NPC state")] [SerializeField] private List<CutsceneDataNPC> _NPCstateChanges = new List<CutsceneDataNPC>();

	[Header("Interaction objects")] [SerializeField] private List<GameObject> _interactionObjects = new List<GameObject>();

	private void Start()
	{

		//foreach (var npc in _animatedNPCs)
		//{
		//if (npc.gameObject.name == "CutscenePlayerProxy")
		//{
		//_playerAnimator = npc.GetComponentInChildren<Animator>();
		//_isPlayerAnimated = true;
		//}
		//}
		//_playerBootstrapAnimator = _cutscenePlayerProxy.GetComponentInChildren<Animator>();
		_playerProxyAnimator = _cutscenePlayerProxy.GetComponent<Animator>();
		_HUDmissionsController = ServiceLocator.Resolve<HUDmissionsController>();
		_missionsManager = ServiceLocator.Resolve<MissionsManager>();
		_playerWeaponFirstPersonRenderer = ServiceLocator.Resolve<PlayerWeaponFirstPersonRenderer>();
		_playerBehaviourController = ServiceLocator.Resolve<PlayerBehaviourController>();
		_viewModelMenuCutscene = ServiceLocator.Resolve<ViewModelMenuCutscene>();
		_playerWeaponAnimationController = ServiceLocator.Resolve<PlayerWeaponAnimationController>();
		_playerCameraBootstrap = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);
		_playerBootstrap = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
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
		//_playerAnimator1stPerson = _playerCameraProxy.GetComponent<Animator>();
		_director = GetComponent<PlayableDirector>();

		_blackLineUp = _viewModelMenuCutscene.BlackLineUp;
		_blackLineDown = _viewModelMenuCutscene.BlackLineDown;
		_textSkipCutscene = _viewModelMenuCutscene.TextSkipCutscene;
		_textComponentSkipCutscene = _textSkipCutscene.GetComponent<TextMeshProUGUI>();
		_circleSkipCutscene = _viewModelMenuCutscene.CircleSkipCutscene;
		_imageCircleSkipCutscene = _circleSkipCutscene.GetComponent<Image>();
		_imageCircleSkipCutscene.fillAmount = 0f;
		_circleSkipCutscene.SetActive(false);
		_textSkipCutscene.SetActive(false);


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
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CancelCutsceneOnLoad;
		_director.stopped += OnTimelineStopped;
		_menuManager.OnOpenPauseMenu += PauseCutscene;
		_menuManager.OnClosePauseMenu += ResumeCutscene;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		//BindProxyObjects();
		ChangeLanguage(_localizationManager);
		_isInitialized = true;

		//Debug.Log($"Cutscene {gameObject.name} initialized");
	}

	private void OnDestroy()
	{
		_director.stopped -= OnTimelineStopped;

		_menuManager.OnOpenPauseMenu -= PauseCutscene;
		_menuManager.OnClosePauseMenu -= ResumeCutscene;

		_gameSceneManager.OnBeginLoadingGameplayScene -= CancelCutsceneOnLoad;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CancelCutsceneOnLoad;

		_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}


	private void Update()
	{
		
	
		


		if (!_isInitialized) return;

		

		ProcessCutsceneSkipping();
	
		if (IsCutscenePlaying)
		{
			

			if (_isSkipRequested)
			{
				SkipCutscene();
			}
		}
	}

	private void ProcessCutsceneSkipping()
	{
		_isSkipRequested = _inputDevice.GetKeySkipCutscene();
		_holdDuration = _inputDevice.GetCurrentSkipCutsceneHoldDuration();

		if (_holdDuration > 0f)
		{
			_textSkipCutscene.SetActive(true);
			_circleSkipCutscene.SetActive(true);
			_imageCircleSkipCutscene.fillAmount = Mathf.Clamp01(_holdDuration / _inputDevice.TimeToHoldKeySkipCutscene);
		}
		else
		{
			_textSkipCutscene.SetActive(false);
			_circleSkipCutscene.SetActive(false);
			_imageCircleSkipCutscene.fillAmount = 0f;
		}
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

		_isCutsceneDialogueActorPlayer = false;

		NPClivingBeing NPCactor = null;
		if (_dialogueActorsMapping[_currentCutsceneDialogueLineIndex] != null)
		{
			NPCactor = _dialogueActorsMapping[_currentCutsceneDialogueLineIndex].GetComponent<NPClivingBeing>();
		}

		if (NPCactor != null)
		{
			_currentActorName = _localizationManager.GetLocalizedString(NPCactor.InteractionObjectNameSystem);
			_textComponentCutsceneDialogue.text = ($"{_currentActorName}: {_localizedCutsceneDialogues[currentLanguage][_currentCutsceneDialogueLineIndex]}");
		}
		else
		{
			_isCutsceneDialogueActorPlayer = true;
			_currentActorName = _localizationManager.GetLocalizedString("NPC_MainCharacter_IvanPetrovich");
			_textComponentCutsceneDialogue.text = ($"{_currentActorName}: {_localizedCutsceneDialogues[currentLanguage][_currentCutsceneDialogueLineIndex]}");
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

		if (currentLanguageVoiceLines.Length > 0)
		{
			if (_isCutsceneDialogueActorPlayer == false)
			{
				_audioSource.PlayOneShot(currentLanguageVoiceLines[_currentCutsceneDialogueLineIndex]);
			}
		}
		

		_currentCutsceneDialogueLineIndex++;
	}

	public void HidePreviousTextCutsceneDialogue()
	{
		_textCutsceneDialogue.SetActive(false);
		_textComponentCutsceneDialogue.text = string.Empty;
	}

	/*
	public void BindProxyObjects()
	{
		var playerProxy = transform.Find("Player_Proxy")?.gameObject;
		var cameraProxy = transform.Find("MainCamera_Proxy")?.gameObject;

		_director.SetGenericBinding(playerProxy, _playerProxy.transform);
		_director.SetGenericBinding(cameraProxy, _playerCameraProxy.transform);
	}
	*/

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
		//_menuManager.OnCloseMenuBackground();

		Debug.Log($"Cutscene {gameObject.name} Cancelled");

		_playerBootstrap.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(_playerBootstrap, SceneManager.GetSceneAt(0));


		_playerCameraBootstrap.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(_playerCameraBootstrap, SceneManager.GetSceneAt(0));

		_gameController.MakeGameSavable();
		_wasCutsceneCanceled = true;
		IsCutscenePlaying = false;
		_director.Stop();
		_director.stopped -= OnTimelineStopped;
		_menuManager.CloseCutsceneMenu();
		//Destroy(gameObject);
	}

	private void SkipCutscene()
	{
		_wasCutsceneSkipped = true;
		Debug.Log("CUTSCENE SKIPPED");
		ExecutePostCutsceneActions();
	}

	private void ExecutePostCutsceneActions()
	{
		_director.stopped -= OnTimelineStopped;

		_playerBootstrap.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(_playerBootstrap, SceneManager.GetSceneAt(0));


		_playerCameraBootstrap.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(_playerCameraBootstrap, SceneManager.GetSceneAt(0));

		_director.time = _director.playableAsset.duration;
		_director.Evaluate();


		//if (wasCutsceneSkipped)
		//{
		_director.Stop();
		//}

		//_playerBootstrapAnimator.updateMode = AnimatorUpdateMode.Normal;
		_playerProxyAnimator.updateMode = AnimatorUpdateMode.Normal;
		if (_animatedNPCs.Count > 0)
		{
			foreach (var npc in _animatedNPCs)
			{
				//Debug.Log(npc.gameObject.name);
				//if (npc.gameObject.name != "CutscenePlayerProxy")
				//{
				npc.updateMode = AnimatorUpdateMode.Normal;
				//}
			}
		}

			if (_playerBehaviourController.WasPlayerArmed)
			{
				_playerBehaviourController.ArmPlayer();
			}

			if (_showMissionGoalHint)
			{
				_HUDmissionsController.ShowNewMissionGoalHUDnotification(_missionsManager.LocalizedGoalText, true);
			}

			//_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.Normal;
			WasCutscenePlaying = false;
			IsCutscenePlaying = false;
			CutsceneResumeTime();
			_menuManager.CloseCutsceneMenu();


			_playerCameraStateMachineController.SetPlayerCameraState(_playerCameraController.PreviousPlayerCameraGameplayType);

			_gameController.MakePlayerControllable();
			_gameController.MakeGameSavable();

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


			_playerMovementController.SetPlayerPosition(_newPlayerTransform.PlayerPosition);
			_playerCameraController.SetCameraRotationY(_newPlayerTransform.PlayerRotationY);


			if (_shouldLoadScene)
			{
				_gameSceneManager.StartCoroutine(_gameSceneManager.LoadGameplayScene((GameScenesGameplayEnum)_sceneToLoadAfterCutscene));
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

			//Destroy(gameObject);





			Debug.Log($"Post-cutscene {gameObject.name} actions executed.");
	}
	



	public void TriggerCutscene(GameObject inspectedWeapon)
	{

		Debug.Log("CUTSCENE!!!");
		_director.Play();
		_gameController.MakeGameUnsavable();
		HidePreviousTextCutsceneDialogue();

		_currentCutsceneDialogueLineIndex = 0;

		CutsceneStopTime();

		if (_animatedNPCs.Count > 0)
		{
			foreach (var npc in _animatedNPCs)
			{
				//Debug.Log(npc.gameObject.name);
				//if (npc.gameObject.name != "CutscenePlayerProxy")
				//{
				npc.updateMode = AnimatorUpdateMode.UnscaledTime;
				//}
			}


		}
		

		//RebindProxyObjects();

		if (_showBlackLines)
		{
			_blackLineUp.SetActive(true);
			_blackLineDown.SetActive(true);
		}
		else
		{
			_blackLineUp.SetActive(false);
			_blackLineDown.SetActive(false);
		}

		if (_ShouldDisarmPlayer)
		{
			_playerBehaviourController.DisarmPlayer();
		}

		if (inspectedWeapon == null)
		{
			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.Cutscene);

			SceneManager.MoveGameObjectToScene(_playerBootstrap, SceneManager.GetSceneAt(1));
			_playerBootstrap.transform.SetParent(_cutscenePlayerProxy.transform);
			_playerBootstrap.transform.localPosition = Vector3.zero;
			_playerBootstrap.transform.localRotation = Quaternion.identity;

			SceneManager.MoveGameObjectToScene(_playerCameraBootstrap, SceneManager.GetSceneAt(1));
			_playerCameraBootstrap.transform.SetParent(_cutscenePlayerCameraProxy.transform);
			_playerCameraBootstrap.transform.localPosition = Vector3.zero;
			_playerCameraBootstrap.transform.localRotation = Quaternion.identity;

			//_playerBootstrapAnimator = _cutscenePlayerProxy.transform.GetChild(0).GetComponent<Animator>();
			//_playerBootstrapAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

			_playerProxyAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;


		}
		else
		{
			//_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.UnscaledTime;

			var inspectedWeaponModel = Instantiate(inspectedWeapon);
			WeaponAbstract inspectedWeaponComponent = inspectedWeaponModel.GetComponent<WeaponAbstract>();
			//inspectedWeapon.InstantiateWeaponInspect();

			_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);


			inspectedWeaponComponent.SetUpWeaponInspect();


			_playerWeaponFirstPersonRenderer.ShowBothHandsForWeaponInspectionCutscene();
			StartCoroutine(inspectedWeaponComponent.InspectWeaponAnimation());
		}

		//Debug.Log(_playerBootstrap.transform.localPosition);

		_menuManager.OpenCutsceneMenu();

		IsCutscenePlaying = true;
		WasCutscenePlaying = true;
		_gameController.MakePlayerNonControllable();

		Debug.Log($"Cutscene {gameObject.name} was triggered");
	}

	private void PauseCutscene()
	{
		if (!_wasCutsceneSkipped && IsCutscenePlaying)
		{
			IsCutscenePlaying = false;

			
			if (_animatedNPCs.Count > 0)
			{
				foreach (var npc in _animatedNPCs) npc.updateMode = AnimatorUpdateMode.Normal;
			}

			_director.playableGraph.GetRootPlayable(0).SetSpeed(0f);

			// Evaluate нужен, чтобы зафиксировать визуальный кадр в этом моменте.
		
			_director.Evaluate();

			// ХАК: Принудительно просим Animator применить текущий кадр графика еще раз.
			// Это перезаписывает возможный сброс в T-pose, который сделал Evaluate.
			_playerProxyAnimator.Update(0f);

			Debug.Log($"Cutscene {gameObject.name} paused.");
		}
	}

	private void ResumeCutscene()
	{
		if (WasCutscenePlaying)
		{
			if (!_wasCutsceneSkipped)
			{
		
				IsCutscenePlaying = true;
				_gameController.MakePlayerNonControllable();
				
				//_playerBootstrapAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
				if (_animatedNPCs.Count > 0)
				{
					foreach (var npc in _animatedNPCs)
					{
						//Debug.Log(npc.gameObject.name);
						//if (npc.gameObject.name != "CutscenePlayerProxy")
						//{
						npc.updateMode = AnimatorUpdateMode.UnscaledTime;
						//}
					}
				}

				_director.playableGraph.GetRootPlayable(0).SetSpeed(1f);

				// Evaluate нужен, чтобы зафиксировать визуальный кадр в этом моменте.

				//_director.Evaluate();
				//_playerAnimator1stPerson.updateMode = AnimatorUpdateMode.UnscaledTime;

				Debug.Log($"Cutscene {gameObject.name} resumed");
			}
		}
	}

	private void ChangeLanguage(LocalizationManager	localizationManager)
	{
		_localizationManager = localizationManager;

		if (IsCutscenePlaying)
		{
			_textComponentCutsceneDialogue.text = _localizedCutsceneDialogues[_localizationManager.CurrentLanguage][_currentCutsceneDialogueLineIndex];
		}

		_textComponentSkipCutscene.text = _localizationManager.GetLocalizedString("UI_Menu_Cutscene_SkipToHold");
	}
}