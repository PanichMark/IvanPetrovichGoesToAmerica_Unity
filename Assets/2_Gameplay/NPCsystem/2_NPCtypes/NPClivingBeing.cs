using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public abstract class NPClivingBeing : NPCabstract
{
	//[SerializeField] private ConfigNPCBodyType _NPCBodyType;
	[SerializeField] private InteractionObjectPickableData _NPCPickableBodyType;
	private Bootstrap _bootstrap;
	[SerializeField] protected GameObject _NPCLootObject;

	//[SerializeField] private NPCphrasesData _NPCPhrasesDefeated;

	// ... (ваши существующие поля) ...

	private NPCmovementController _npcMovementController_cached;
	protected NPCmovementController _NPCmovementController
	{
		get
		{
			if (_npcMovementController_cached == null || !_npcMovementController_cached)
			{
				_npcMovementController_cached = GetComponent<NPCmovementController>();
			}
			return _npcMovementController_cached;
		}
		private set { _npcMovementController_cached = value; }
	}

	private NPCstateMachineController _npcStateMachineController_cached;
	protected NPCstateMachineController _NPCstateMachineController
	{
		get
		{
			if (_npcStateMachineController_cached == null || !_npcStateMachineController_cached)
			{
				_npcStateMachineController_cached = GetComponent<NPCstateMachineController>();
			}
			return _npcStateMachineController_cached;
		}
		private set { _npcStateMachineController_cached = value; }
	}

	private NPChealthController _npcHealthController_cached;
	protected NPChealthController _NPChealthController
	{
		get
		{
			if (_npcHealthController_cached == null || !_npcHealthController_cached)
			{
				_npcHealthController_cached = GetComponent<NPChealthController>();
			}
			return _npcHealthController_cached;
		}
		private set { _npcHealthController_cached = value; }
	}
	protected InteractionObjectLootAbstract _NPClootObjectComponent;

	protected bool _canNPCbeRobbed;
	protected bool _wasNPCrobbed;


	private MeshRenderer[] _NPCvfxEffectMeshRenderers;
	private Transform _NPCarmatureTransform;

	protected TransferSkinnedMeshRendererArmatureBones _transferArmatureBones;
	private NPCvfxEffectsController _npcvfxController;

	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private GameObject _canvasNPC;
	private GameObject _imageDetectionSign;
	private List<Sprite> _detectionSignFrames;
	private GameObject _textNPCcurrentState;
	private GameObject _textNPCcurrentHealth;
	private GameObject _playerCameraGameObject;

	protected EventController _NPCeventController;
	protected EventNPCanimationController _NPCeventAnimationController;

	private NavMeshAgent _navMeshAgent;

	protected InteractionObjectPickableNonThrowableAbstract _pickable;
	protected NPCmovementAnimationController _NPCmovementAnimationController;

	protected NPCdetectionManager _NPCdetectionManager;
	protected NPCdetectionVisualController _NPCdetectionVisualController;
	protected NPCdetectionAudioController _NPCdetectionAudioController;
	protected NPCdetectionSignController _NPCdetectionSignController;

	protected NPCdebugHUDcontroller _NPCdebugHUDcontroller;
	protected MenuManager _menuManager;
	protected override void InitializeNPCabstract()
	{
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_bootstrap = ServiceLocator.Resolve<Bootstrap>();
		_playerCameraGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);
		_detectionSignFrames = ServiceLocator.Resolve<List<Sprite>>();
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>();

		_transferArmatureBones = gameObject.transform.Find("NPC_3Dmodel").GetComponent<TransferSkinnedMeshRendererArmatureBones>();

		_navMeshAgent = GetComponent<NavMeshAgent>();

		_NPCarmatureTransform = transform.Find("NPC_3Dmodel/ArmatureHitbox/Armature_Humanoid");

		_NPCvfxEffectMeshRenderers = _NPCarmatureTransform.GetComponentsInChildren<MeshRenderer>(true);
		//Debug.Log($"НАШЛОСЬ {_NPCeffectMeshRenderers.Length} MeshRenderer компонентов.");

		_canvasNPC = transform.Find("NPC_Canvas").gameObject;
		_imageDetectionSign = _canvasNPC.transform.Find("DetectionSign").gameObject;
		_textNPCcurrentState = _canvasNPC.transform.Find("DebugNPCcurrentState").gameObject;
		_textNPCcurrentHealth = _canvasNPC.transform.Find("DebugNPCcurrentHealth").gameObject;

		if (_bootstrap.DebugNPC)
		{
			_textNPCcurrentState.SetActive(true);
			_textNPCcurrentHealth.SetActive(true);
		}
		else
		{
			_textNPCcurrentState.SetActive(false);
			_textNPCcurrentHealth.SetActive(false);
		}

		_NPCmovementAnimationController = GetComponent<NPCmovementAnimationController>();
		_NPCstateMachineController = GetComponent<NPCstateMachineController>();
		_NPCeventController = GetComponent<EventController>();
		_NPCeventAnimationController = GetComponent<EventNPCanimationController>();
		_NPChealthController = GetComponent<NPChealthController>();

		_NPCmovementController = GetComponent<NPCmovementController>();
		_NPCdetectionManager = GetComponent<NPCdetectionManager>();
		_NPCdetectionVisualController = GetComponent<NPCdetectionVisualController>();
		_NPCdetectionAudioController = GetComponent<NPCdetectionAudioController>();
		_NPCdetectionSignController = GetComponent<NPCdetectionSignController>();
		_npcvfxController = GetComponent<NPCvfxEffectsController>();
		_NPCdebugHUDcontroller = GetComponent<NPCdebugHUDcontroller>();

		_NPCmovementController.Initialize(
			_menuManager,
			_NPCstateMachineController,
			_navMeshAgent);

		_NPCmovementAnimationController.Initialize(
			_NPCmovementController);

		_NPCstateMachineController.Initialize(
			this,
			_NPCmovementController);

		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		_NPCdetectionManager.Initialize(_NPCstateMachineController);

		_NPCdetectionVisualController.Initialize(_NPCdetectionManager);

		_NPCdetectionAudioController.Initialize();

		_NPCdetectionSignController.Initialize(
			_NPCdetectionManager,
			_canvasNPC,
			_imageDetectionSign,
			_detectionSignFrames,
			_playerCameraGameObject);

		_NPCdebugHUDcontroller.Initialize(
			_playerCameraGameObject,
			_NPChealthController,
			_NPCstateMachineController,
			_textNPCcurrentState,
			_textNPCcurrentHealth);

		if (_NPCeventController != null)
		{
			_NPCeventController.Initialize();
		}

		if (_NPCeventAnimationController != null)
		{
			_NPCeventAnimationController.Initialize(_NPCeventController);
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead && _NPCstateMachineController.CurrentNPCState != NPCstateTypes.Unconscious)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
		}
		else
		{
			_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		}

		_npcvfxController.Initialize(
			_NPCstateMachineController,
			_NPCvfxEffectMeshRenderers);


		if (_NPCLootObject != null)
		{
			_NPClootObjectComponent = _NPCLootObject.GetComponent<InteractionObjectLootAbstract>();
			_NPClootObjectComponent.MakeLootObjectNPC();

			_playerMovementStateMachineController.OnChangeMovementState += HandleNPClooteableState;
		}

		InitializeNPClivingBeing();
	}

	protected virtual void InitializeNPClivingBeing()
	{

	}

	public void ConvertToPickableObject()
	{
		//Debug.Log("CONVERT!!!");

		gameObject.tag = "Interactable";
		enabled = false;
		_NPChealthController.enabled = false;

		_NPCphrasesController.enabled = false;

		DisableDialogueController();

		var capsuleCollider = GetComponent<CapsuleCollider>();
		if (capsuleCollider != null)
		{
			Destroy(capsuleCollider);
		}

		var Rigidbody =  gameObject.GetComponent<Rigidbody>();
		Rigidbody.isKinematic = false;

		_pickable = InteractionObjectPickableNonThrowableUndestructable.CreateWithName(gameObject, _NPCName, _NPCPickableBodyType);

		_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		//Destroy(this);
	}

	private void HandleNPClooteableState(PlayerMovementStateTypes playerStateType)
	{
		//Debug.Log("BRUH!!!!");
		if (!_wasNPCrobbed)
		{
			if (!_NPCdetectionVisualController.IsPlayerVisible && (playerStateType == PlayerMovementStateTypes.PlayerIdleCrouhcing || playerStateType == PlayerMovementStateTypes.PlayerWalkingCrouching))
			{
				MakeNPCrobbable();
			}
			else
			{
				MakeNPCunreobbable();
			}
		}
	}

	private void MakeNPCrobbable()
	{ 
		_canNPCbeRobbed = true;

		gameObject.tag = "Interactable";

		_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Rob");
	}

	private void MakeNPCunreobbable()
	{
		_canNPCbeRobbed = false;

		if (this is NPCaggressive)
		{
			gameObject.tag = "Untagged";
		}

		_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
	}

	protected void NPCwasRobbed()
	{
		_canNPCbeRobbed = false;
		_wasNPCrobbed = true;
		_NPCLootObject = null;
		_NPClootObjectComponent = null;

		if (this is NPCaggressive)
		{
			gameObject.tag = "Untagged";
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead && _NPCstateMachineController.CurrentNPCState != NPCstateTypes.Unconscious)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
		}
	}

	protected virtual void DisableDialogueController()
	{

	}

	protected override void ChangeLangauge(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;


		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
		}
		else
		{
			_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		}

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_CantTalkToPlayerRightNow");
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.NPCsData == null || !data.NPCsData.ContainsKey(currentScene))
			yield break;

		var targetList = data.NPCsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.NPCindex == GameplayObjectIndex);

		Debug.Log(GameplayObjectIndex);
		Debug.Log(_NPCmovementController);

		var updatedItem = new NPCdata
		{
			NPCindex = GameplayObjectIndex,
			NPCnameSystem = InteractionObjectNameSystem,
			NPCposition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			NPCrotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			NPCstate = _NPCstateMachineController.CurrentNPCState.ToString(),
			NPCcurrentGoalAnchorPoint = _NPCmovementController.CurrentGoalAnchorPointIndex,
			NPChealth = Mathf.Round(_NPChealthController.CurrentHealth * 100f) / 100f
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.NPCsData == null || !data.NPCsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.NPCindex == GameplayObjectIndex);

		if (savedState.Equals(default(NPCdata))) yield break;

		_NPCstateMachineController.SetNPCState((NPCstateTypes)Enum.Parse(typeof(NPCstateTypes), savedState.NPCstate));
		_NPCmovementController.SetLoadedCurrentAnchorPoint(savedState.NPCcurrentGoalAnchorPoint);

		gameObject.transform.position = savedState.NPCposition;
		gameObject.transform.rotation = savedState.NPCrotation;

		_NPChealthController.SetCurrentHealthFromLoad(savedState.NPChealth);

		
		

		yield return null;
	}
}
