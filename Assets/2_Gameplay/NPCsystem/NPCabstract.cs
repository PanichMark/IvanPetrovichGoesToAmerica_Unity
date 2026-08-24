using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NPChealthController))]
[RequireComponent(typeof(NPCstateMachineController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NPCdebugHUDcontroller))]

public abstract class NPCabstract : NPCcore, IInteractable
{

	//[SerializeField] private bool _isHuman;
	[SerializeField] protected string _NPCname;

	[SerializeField] private ConfigNPCBodyType _NPCconfigBodyType;

	[SerializeField] private InteractionObjectPickableData _pickableBodyData;
	private GameObject _playerCameraGameObject;
	private Camera _playerCamera;
	private GameObject _canvasNPCstatus;
	private GameObject _imageDetectionSign;
	private GameObject _textNPCcurrentState;
	private GameObject _textNPCcurrentHealth;

	public event IInteractable.InteractableObjectHandler OnInteract;

	protected NPCphrasesController _NPCphrasesController;
	protected NPChealthController _NPChealthController;
	protected NPCdebugHUDcontroller _NPCdebugHUDcontroller;
	protected NPCdialogueController _NPCdialogueController;
	protected NPCweaponController _NPCweaponController;
	protected NPCdetectionManager _NPCdetectionManager;
	protected NPCdetectionSignController _NPCdetectionSignController;
	protected InteractionObjectPickableNonThrowable _pickable;
	private NavMeshAgent _navMeshAgent;
	private List<Sprite> _detectionSignFrames;
	private LocalizationManager _localizationManager;
	protected NPCstateMachineController _NPCstateMachineController;

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
		_playerCameraGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
		_playerCamera = _playerCameraGameObject.GetComponent<Camera>();
		_detectionSignFrames = ServiceLocator.Resolve<List<Sprite>>("NPCdetectionSignFrames");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");
		_navMeshAgent = GetComponent<NavMeshAgent>();

		_canvasNPCstatus = transform.Find("CanvasNPCstatus").gameObject;
		_textNPCcurrentState = _canvasNPCstatus.transform.Find("TextNPCcurrentState").gameObject;
		_textNPCcurrentHealth = _canvasNPCstatus.transform.Find("TextNPCcurrentHealth").gameObject;
		_imageDetectionSign = _canvasNPCstatus.transform.Find("DetectionSign").gameObject;

		_NPCstateMachineController = GetComponent<NPCstateMachineController>();
		_NPChealthController = GetComponent<NPChealthController>();
		_NPCphrasesController = GetComponent<NPCphrasesController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();
		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCdetectionManager = GetComponent<NPCdetectionManager>();	
		_NPCdetectionSignController = GetComponent<NPCdetectionSignController>();
		_NPCdebugHUDcontroller = GetComponent<NPCdebugHUDcontroller>();
		
		_NPCstateMachineController.Initialize(
			this,
			_navMeshAgent);

		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		if (_NPCphrasesController != null)
		{
			_NPCphrasesController.Initialize();
		}

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize();
		}

		if (_NPCweaponController != null)
		{
			_NPCweaponController.Initialize();
		}

		_NPCdetectionManager.Initialize();

		_NPCdetectionSignController.Initialize(
			_NPCdetectionManager,
			_canvasNPCstatus,
			_imageDetectionSign,
			_detectionSignFrames,
			_playerCamera);

		if (_NPCdebugHUDcontroller != null)
		{
			_NPCdebugHUDcontroller.Initialize(
				_NPChealthController,
				_NPCstateMachineController,
				_canvasNPCstatus,
				_textNPCcurrentState,
				_textNPCcurrentHealth);
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Dead)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
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
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
		}
		else
		{
			_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		}

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");
	}

	public void ConvertToPickableObject()
	{
		//Debug.Log("CONVERT!!!");

		gameObject.tag = "Interactable";
		enabled = false;
		_NPChealthController.enabled = false;

		if (_NPCphrasesController != null)
		{
			_NPCphrasesController.enabled = false;
		}
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}

		var capsuleCollider = GetComponent<CapsuleCollider>();
		if (capsuleCollider != null)
		{
			Destroy(capsuleCollider);
		}

		gameObject.AddComponent<Rigidbody>();

		_pickable = InteractionObjectPickableNonThrowable.CreateWithName(gameObject, _NPCname, _pickableBodyData);

		_interactionHintMessageAction = _pickable.InteractionHintMessageAction;
		//Destroy(this);
	}

	public override IEnumerator SaveData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.NPCsData == null || !data.NPCsData.ContainsKey(currentScene))
			yield break;

		var targetList = data.NPCsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.NPCindex == NPCindex);

		var updatedItem = new NPCdata
		{
			NPCindex = NPCindex,
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

	public override IEnumerator LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.NPCsData == null || !data.NPCsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.NPCindex == NPCindex);

		if (savedState.Equals(default(NPCdata))) yield break;

		gameObject.transform.position = savedState.NPCposition;
		gameObject.transform.rotation = savedState.NPCrotation;

		_NPChealthController.SetCurrentHealthFromLoad(savedState.NPChealth);

		int safeAnchorIndex = Mathf.Clamp(savedState.NPCnextAnchorPoint, 0, _NPCstateMachineController.AnchorData.Count > 0 ? _NPCstateMachineController.AnchorData.Count - 1 : 0);
		_NPCstateMachineController.SetNPCState(savedState.NPCstate);

		yield return null;
	}
}