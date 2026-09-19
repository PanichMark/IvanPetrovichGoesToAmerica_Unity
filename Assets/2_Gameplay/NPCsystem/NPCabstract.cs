using UnityEngine;


public abstract class NPCabstract : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] protected string _NPCname;

	protected LocalizationManager _localizationManager;

	protected NPCphrasesController _NPCphrasesController;

	public event IInteractable.InteractableObjectHandler OnInteract;

	public string InteractionObjectNameSystem => _NPCname;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_NPCname);
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	protected string _interactionHintMessageFail;

	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	protected string _interactionHintMessageAction;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_CantTalkToPlayerRightNow");
		_NPCphrasesController = GetComponent<NPCphrasesController>();
		_NPCphrasesController.Initialize(this);
	
		InitializeNPCabstract();

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	protected virtual void InitializeNPCabstract()
	{

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

	protected virtual void ChangeLangauge(LocalizationManager localizationManager)
	{

	}
}