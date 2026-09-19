using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public abstract class NPCabstract : GameplayObjectJsonSaveLoad, IInteractable
{

	//[SerializeField] private bool _isHuman;
	[SerializeField] protected string _NPCname;

	[SerializeField] private ConfigNPCBodyType _NPCconfigBodyType;

	[SerializeField] private InteractionObjectPickableData _pickableBodyData;
	private GameObject _playerCameraGameObject;
	private GameObject _canvasNPCstatus;
	private GameObject _imageDetectionSign;
	private GameObject _textNPCcurrentState;
	private GameObject _textNPCcurrentHealth;

	public event IInteractable.InteractableObjectHandler OnInteract;


	protected NPChealthController _NPChealthController;
	protected NPCdebugHUDcontroller _NPCdebugHUDcontroller;



	protected NPCdetectionManager _NPCdetectionManager;

	protected NPCdetectionSignController _NPCdetectionSignController;

	protected InteractionObjectPickableNonThrowableAbstract _pickable;

	private NavMeshAgent _navMeshAgent;
	private List<Sprite> _detectionSignFrames;
	private LocalizationManager _localizationManager;
	protected NPCstateMachineController _NPCstateMachineController;
	protected NPCdetectionVisualController _NPCdetectionVisualController;
	public string InteractionObjectNameSystem => _NPCname;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_NPCname);
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;

	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	private string _interactionHintMessageAction;

	protected virtual void InitializeNPC()
	{

	}

	private void Start()
	{
		_playerCameraGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);
		_detectionSignFrames = ServiceLocator.Resolve<List<Sprite>>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_CantTalkToPlayerRightNow");
		_navMeshAgent = GetComponent<NavMeshAgent>();

		_canvasNPCstatus = transform.Find("NPC_Canvas").gameObject;
		_imageDetectionSign = _canvasNPCstatus.transform.Find("DetectionSign").gameObject;
		_textNPCcurrentState = _canvasNPCstatus.transform.Find("DebugNPCcurrentState").gameObject;
		_textNPCcurrentHealth = _canvasNPCstatus.transform.Find("DebugNPCcurrentHealth").gameObject;
		

		_NPCstateMachineController = GetComponent<NPCstateMachineController>();
		_NPChealthController = GetComponent<NPChealthController>();

	
		_NPCdetectionManager = GetComponent<NPCdetectionManager>();
		_NPCdetectionVisualController = GetComponent<NPCdetectionVisualController>();
		_NPCdetectionSignController = GetComponent<NPCdetectionSignController>();
		_NPCdebugHUDcontroller = GetComponent<NPCdebugHUDcontroller>();
		
		_NPCstateMachineController.Initialize(
			this,
			_navMeshAgent);

		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		_NPCdetectionManager.Initialize(_NPCstateMachineController);

		_NPCdetectionVisualController.Initialize(_NPCdetectionManager);

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
		
		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TalkTo");
		}
		else
		{
			_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		}

		InitializeNPC();

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	public virtual void Interact()
	{
		//Debug.Log("bruh");
	}

	public void InteractCutscene()
	{
		//Debug.Log("LMAO!");
		Interact();
	}

	private void ChangeLangauge(LocalizationManager localizationManager)
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

	protected virtual void DisableInteractiveNPCscripts()
	{

	}

	public void ConvertToPickableObject()
	{
		//Debug.Log("CONVERT!!!");

		gameObject.tag = "Interactable";
		enabled = false;
		_NPChealthController.enabled = false;

		DisableInteractiveNPCscripts();

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
			NPCnextAnchorPoint = _NPCstateMachineController.AnchorData.Count,
			NPCstate = _NPCstateMachineController.CurrentNPCState,
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

		int safeAnchorIndex = Mathf.Clamp(savedState.NPCnextAnchorPoint, 0, _NPCstateMachineController.AnchorData.Count > 0 ? _NPCstateMachineController.AnchorData.Count - 1 : 0);
		_NPCstateMachineController.SetNPCState(savedState.NPCstate);

		yield return null;
	}
}