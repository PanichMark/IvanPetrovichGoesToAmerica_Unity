using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPChealthController))]
[RequireComponent(typeof(NPCstateMachineController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NPCdebugHUDcontroller))]

public abstract class NPCabstract : MonoBehaviour, IInteractable
{

	//[SerializeField] private bool _isHuman;
	[SerializeField] protected string _NPCname;

	[SerializeField] private ConfigNPCBodyType _NPCconfigBodyType;
	

	[SerializeField] private InteractionObjectPickableData _pickableBodyData;

	public event IInteractable.InteractableObjectHandler OnInteract;

	protected NPCphrasesController _NPCphrasesController;
	protected NPChealthController _NPChealthController;
	protected NPCdebugHUDcontroller _NPCdebugHUDcontroller;
	protected NPCdialogueController _NPCdialogueController;
	protected NPCweaponController _NPCweaponController;
	private NavMeshAgent _navMeshAgent;

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
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");
		_navMeshAgent = GetComponent<NavMeshAgent>();

		_NPCstateMachineController = GetComponent<NPCstateMachineController>();
		_NPChealthController = GetComponent<NPChealthController>();
		_NPCphrasesController = GetComponent<NPCphrasesController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();
		_NPCweaponController = GetComponent<NPCweaponController>();	
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

		if (_NPCdebugHUDcontroller != null)
		{
			_NPCdebugHUDcontroller.Initialize(
				_NPChealthController,
				_NPCstateMachineController);
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
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");
	}

	public void ConvertToPickableObject()
	{
		//Debug.Log("CONVERT!!!");

		gameObject.tag = "Interactable";
		enabled = false;

		var capsuleCollider = GetComponent<CapsuleCollider>();
		if (capsuleCollider != null)
		{
			Destroy(capsuleCollider);
		}

		gameObject.AddComponent<Rigidbody>();

		InteractionObjectPickableNonThrowable.CreateWithName(gameObject, _NPCname, _pickableBodyData);
		Destroy(this);
	}
}