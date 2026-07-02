using UnityEngine;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NPCStateMachineController))]

public abstract class NPCAbstract : MonoBehaviour, IInteractable, IDamageable
{
	private GameObject _canvasNPCstatus;
	private GameObject _textNPCcurrentState;
	private TextMeshProUGUI _textComponentNPCcurrentState;
	private GameObject _textNPCcurrentHealth;
	private TextMeshProUGUI _textComponentNPCcurrentHealth;
	[SerializeField] private bool _isHuman;
	[SerializeField] protected string _NPCname;

	[SerializeField] ConfigNPCBodyType _NPCconfigBodyType;
	
	public bool IsHuman => _isHuman;
	[SerializeField] ConfigNPCHealth _NPCconfigHealth;
	public bool IsNPCdead => _currentHealth <= 0;
	public event IInteractable.InteractableObjectHandler OnInteract;

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
	public bool IsObjectDestroyed => false;

	private float _currentHealth;
	public float CurrentHealth => _currentHealth;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Talk");
		_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_CantTalk");

		_canvasNPCstatus = transform.Find("CanvasNPCstatus").gameObject;
		_textNPCcurrentState = _canvasNPCstatus.transform.Find("TextNPCcurrentState").gameObject;
		_textComponentNPCcurrentState = _textNPCcurrentState.GetComponent<TextMeshProUGUI>();
		_textNPCcurrentHealth = _canvasNPCstatus.transform.Find("TextNPCcurrentHealth").gameObject;
		_textComponentNPCcurrentHealth = _textNPCcurrentHealth.GetComponent<TextMeshProUGUI>();

		_currentHealth = _NPCconfigHealth.NPCcurrentHealth;
		_textComponentNPCcurrentHealth.text = _NPCconfigHealth.NPCcurrentHealth.ToString();

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

		_textComponentNPCcurrentHealth.text = _currentHealth.ToString();

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
			_textNPCcurrentHealth.SetActive(false);
			_NPCstateMachineController.SetNPCState(NPCStateTypes.Dead);
		}
	}

	public void ShowNPCcurrentState(string newState)
	{
		_textComponentNPCcurrentState.text = newState;
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{_NPCname} is Dead");
	
		_currentHealth = 0;
		StopAllCoroutines();
		ConvertToPickableObject();
		gameObject.AddComponent<DamageableCorpse>();
		_NPCphrasesController.ClearPhrases();
	}
}