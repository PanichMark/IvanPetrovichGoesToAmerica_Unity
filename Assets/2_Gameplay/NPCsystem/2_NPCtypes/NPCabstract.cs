using UnityEngine;
using UnityEngine.AI;
using TMPro;

//[RequireComponent(typeof(CapsuleCollider))]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NPCstateMachineController))]


public abstract class NPCabstract : MonoBehaviour, IInteractable, IDamageable
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

	protected NPCphrasesController _NPCphrasesController;

	protected NPCdialogueController _NPCdialogueController;
	protected NPCweaponController _NPCweaponController;


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
	public bool IsObjectDestroyed => false;

	private float _currentHealth;
	public float CurrentHealth => _currentHealth;

	public bool CanObjectBeDamaged => throw new System.NotImplementedException();

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

		_NPCphrasesController = GetComponent<NPCphrasesController>();
		_NPCstateMachineController = GetComponent<NPCstateMachineController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		_NPCstateMachineController.Initialize();
		_NPCphrasesController.Initialize();
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize();
		}
		if (_NPCweaponController != null)
		{
			_NPCweaponController.Initialize();
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

		var capsuleCollider = GetComponent<CapsuleCollider>();
		if (capsuleCollider != null)
		{
			Destroy(capsuleCollider);
		}

		gameObject.AddComponent<Rigidbody>();

		InteractionObjectPickableNonThrowable.CreateWithName(gameObject, _NPCname);
		Destroy(this);
	}

	public void TakeDamage(float amount)
	{
		if (!IsNPCdead)
		{
			Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");
			_currentHealth -= amount;

			_textComponentNPCcurrentHealth.text = _currentHealth.ToString();

			if (IsNPCdead)
			{
				_textNPCcurrentHealth.SetActive(false);
				_NPCstateMachineController.SetNPCState(NPCstateTypes.Dead);
			}
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
		//gameObject.AddComponent<NPCdamageableBody>();
		_NPCphrasesController.ClearPhrases();
	}
}