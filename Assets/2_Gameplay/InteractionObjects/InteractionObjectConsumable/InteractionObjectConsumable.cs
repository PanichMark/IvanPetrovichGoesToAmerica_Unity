using UnityEngine;
using System.Collections;

public class InteractionObjectConsumable : MonoBehaviour, IInteractable, ISaveLoad
{
	[Header("Object Info")]
	[SerializeField] private string _ConsumableName;
	[SerializeField] private InteractionObjectConsumableTypes _interactionObjectConsumableTypes;

	[Header("Object Effects")]
	[SerializeField] private float _healthAffected;
	[SerializeField] private bool _isRotten;

	public event IInteractable.InteractableObjectHandler OnInteract;
	public GameObject GameObjectPlayer { get; protected set; }
	private LocalizationManager _localizationManager;
	public Collider ConsumableObjectCollider { get; protected set; }
	public string InteractionObjectNameSystem => _ConsumableName;

	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(InteractionObjectNameSystem);
	private PlayerHealthController _playerResourcesHealthManager;
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	private string _interactionHintMessageAction;
	public string InteractionHintMessageFail => null;

	public bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerHealthController>("PlayerResourcesHealthManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		ConsumableObjectCollider = GetComponent<Collider>();
		GameObjectPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (_interactionObjectConsumableTypes == InteractionObjectConsumableTypes.Food)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Eat");
		}
		else if (_interactionObjectConsumableTypes == InteractionObjectConsumableTypes.Drink)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drink");
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_interactionObjectConsumableTypes == InteractionObjectConsumableTypes.Food)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Eat");
		}
		else if (_interactionObjectConsumableTypes == InteractionObjectConsumableTypes.Drink)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drink");
		}
	}

	public void Interact()
	{
		ConsumableObjectCollider.enabled = false;
		gameObject.tag = "Untagged";
		StartCoroutine(MoveTowardsPlayer());
	}

	IEnumerator MoveTowardsPlayer()
	{
		float currentSpeed = 3.5f;
		float speedIncrease = 5;

		while (true)
		{
			Vector3 targetPosition = GameObjectPlayer.transform.position + Vector3.up * 1f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				if (!_isRotten)
				{
					_playerResourcesHealthManager.ReceiveHealth(_healthAffected);
				}
				else
				{
					_playerResourcesHealthManager.TakeDamage(_healthAffected);
				}

				Destroy(gameObject);
				break;
			}

			currentSpeed += speedIncrease * Time.deltaTime;
			yield return null;
		}
	}

	public void InteractCutscene()
	{
		if (!_isRotten)
		{
			_playerResourcesHealthManager.ReceiveHealth(_healthAffected);
		}
		else
		{
			_playerResourcesHealthManager.TakeDamage(_healthAffected);
		}

		Destroy(gameObject);
	}

	public void LoadData(GameData data)
	{
	//	throw new System.NotImplementedException();
	}

	public void SaveData(ref GameData data)
	{
		//throw new System.NotImplementedException();
	}
}