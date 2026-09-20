using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public abstract class NPClivingBeing : NPCabstract
{
	[SerializeField] private ConfigNPCBodyType _NPCconfigBodyType;
	[SerializeField] private InteractionObjectPickableData _pickableBodyData;

	private GameObject _canvasNPCstatus;
	private GameObject _imageDetectionSign;
	private List<Sprite> _detectionSignFrames;
	private GameObject _textNPCcurrentState;
	private GameObject _textNPCcurrentHealth;
	private GameObject _playerCameraGameObject;

	private NavMeshAgent _navMeshAgent;
	protected NPCmovementController _NPCmovementController;
	protected NPChealthController _NPChealthController;

	protected InteractionObjectPickableNonThrowableAbstract _pickable;
	protected NPCmovementAnimationController _NPCmovementAnimationController;
	protected NPCstateMachineController _NPCstateMachineController;

	protected NPCdetectionManager _NPCdetectionManager;
	protected NPCdetectionVisualController _NPCdetectionVisualController;
	protected NPCdetectionAudioController _NPCdetectionAudioController;
	protected NPCdetectionSignController _NPCdetectionSignController;

	protected NPCdebugHUDcontroller _NPCdebugHUDcontroller;

	protected override void InitializeNPCabstract()
	{
		_playerCameraGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);
		_detectionSignFrames = ServiceLocator.Resolve<List<Sprite>>();

		_navMeshAgent = GetComponent<NavMeshAgent>();

		_canvasNPCstatus = transform.Find("NPC_Canvas").gameObject;
		_imageDetectionSign = _canvasNPCstatus.transform.Find("DetectionSign").gameObject;
		_textNPCcurrentState = _canvasNPCstatus.transform.Find("DebugNPCcurrentState").gameObject;
		_textNPCcurrentHealth = _canvasNPCstatus.transform.Find("DebugNPCcurrentHealth").gameObject;
		_NPCmovementAnimationController = GetComponent<NPCmovementAnimationController>();
		_NPCstateMachineController = GetComponent<NPCstateMachineController>();

		_NPChealthController = GetComponent<NPChealthController>();

		_NPCmovementController = GetComponent<NPCmovementController>();
		_NPCdetectionManager = GetComponent<NPCdetectionManager>();
		_NPCdetectionVisualController = GetComponent<NPCdetectionVisualController>();
		_NPCdetectionAudioController = GetComponent<NPCdetectionAudioController>();
		_NPCdetectionSignController = GetComponent<NPCdetectionSignController>();
		_NPCdebugHUDcontroller = GetComponent<NPCdebugHUDcontroller>();

		_NPCmovementController.Initialize(_navMeshAgent);

		_NPCmovementAnimationController.Initialize(_NPCmovementController);

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
			_canvasNPCstatus,
			_imageDetectionSign,
			_detectionSignFrames,
			_playerCameraGameObject);

		_NPCdebugHUDcontroller.Initialize(
			_playerCameraGameObject,
			_NPChealthController,
			_NPCstateMachineController,
			_canvasNPCstatus,
			_textNPCcurrentState,
			_textNPCcurrentHealth);

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead && _NPCstateMachineController.CurrentNPCState != NPCstateTypes.Unconscious)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
		}
		else
		{
			_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
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

		gameObject.AddComponent<Rigidbody>();

		_pickable = InteractionObjectPickableNonThrowableUndestructable.CreateWithName(gameObject, _NPCname, _pickableBodyData);

		_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		//Destroy(this);
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
			NPCnextAnchorPoint = _NPCmovementController.AnchorData.Count,
			NPCstate = _NPCstateMachineController.CurrentNPCState.ToString(),
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

		gameObject.transform.position = savedState.NPCposition;
		gameObject.transform.rotation = savedState.NPCrotation;

		_NPChealthController.SetCurrentHealthFromLoad(savedState.NPChealth);

		int safeAnchorIndex = Mathf.Clamp(savedState.NPCnextAnchorPoint, 0, _NPCmovementController.AnchorData.Count > 0 ? _NPCmovementController.AnchorData.Count - 1 : 0);
		_NPCstateMachineController.SetNPCState((NPCstateTypes)Enum.Parse(typeof(NPCstateTypes), savedState.NPCstate));

		yield return null;
	}
}
