using UnityEngine;


public abstract class NPCabstract : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] protected string _NPCName;

	protected LocalizationManager _localizationManager;
	protected Animator _NPCanimator;
	protected NPCphrasesController _NPCphrasesController;
	public event IInteractable.InteractableObjectHandler OnInteract;


	public string InteractionObjectNameSystem => _NPCName;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_NPCName);
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	protected string _interactionHintMessageFail;

	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	protected string _interactionHintMessageAction;

	protected override void InitializeGameplayObjectJsonSaveLoad()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_NPCanimator = GetComponent<Animator>();

		_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_CantTalkToPlayerRightNow");
		_NPCphrasesController = GetComponent<NPCphrasesController>();
	
		InitializeNPCabstract();

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}



	protected virtual void InitializeNPCabstract()
	{

	}

	public virtual void Interact()
	{

	}

	public void CallOnInteract()
	{
		OnInteract?.Invoke();
	}

	public void InteractAfterDialogueOrCutscene()
	{
		//Debug.Log("LMAO!");
		Interact();
	}

	protected virtual void ChangeLangauge(LocalizationManager localizationManager)
	{

	}
}