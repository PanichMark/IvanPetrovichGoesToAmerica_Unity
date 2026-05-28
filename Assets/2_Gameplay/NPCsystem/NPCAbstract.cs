using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NPCStateMachineController))]
[RequireComponent(typeof(NPCPhrasesController))]

public abstract class NPCAbstract : MonoBehaviour, IInteractable, IDamageable
{
	[SerializeField] protected string _NPCname;

	[SerializeField] ConfigNPCBodyType _NPCconfigBodyType;

	[SerializeField] ConfigNPCHealth _NPCconfigHealth;
	public bool IsNPCdead => _currentHealth <= 0;

	protected NPCPhrasesController _NPCphrasesController;

	protected NPCDialogueController _NPCdialogueController;


	private LocalizationManager _localizationManager;
	protected NPCStateMachineController _NPCstateMachineController;

	public string InteractionObjectNameSystem => _NPCname;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_NPCname);
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;

	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	private string _interactionHintMessageAction;
	public bool WasObjectDestroyed => false;

	private float _currentHealth;
	public float CurrentHealth => _currentHealth;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");

		_currentHealth = _NPCconfigHealth.NPCcurrentHealth;

		_NPCphrasesController = GetComponent<NPCPhrasesController>();
		_NPCstateMachineController = GetComponent<NPCStateMachineController>();
		_NPCdialogueController = GetComponent<NPCDialogueController>();

		_NPCphrasesController.Initialize();
		_NPCstateMachineController.Initialize();
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize();
		}

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	public virtual void Interact()
	{
		Debug.Log("bruh");
	}

	public void InteractCutscene()
	{
		Debug.Log("LMAO!");
		Interact();
	}

	private void ChangeLangauge()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TalkTo");
		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");
	}

	public void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";
		enabled = false;
		gameObject.AddComponent<Rigidbody>();
		InteractionObjectPickable.CreateWithName(gameObject, _NPCname);
		Destroy(this);
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");
		_currentHealth -= amount;

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
			_NPCstateMachineController.SetNPCState(NPCStateTypes.Dead);
		}
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{_NPCname} is Dead");
	
		_currentHealth = 0;
		StopAllCoroutines();
		ConvertToPickableObject();
		_NPCphrasesController.ClearPhrases();
	}
}