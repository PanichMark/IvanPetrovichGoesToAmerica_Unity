using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class InteractionObjectConsumable : MonoBehaviour, IInteractable, ISaveLoad
{
	[SerializeField] private string _ConsumableName;
	[SerializeField] private float _healthToRestore;

	[SerializeField] private InteractionObjectConsumableTypes _interactionObjectConsumableTypes;
	[SerializeField] private bool _isRotten;
	public GameObject GameObjectPlayer { get; protected set; }
	private LocalizationManager _localizationManager;
	public Collider ConsumableObjectCollider { get; protected set; }
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string InteractionObjectNameUI => throw new System.NotImplementedException();
	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	public string InteractionHintMessageMain => throw new System.NotImplementedException();
	private string _interactionObjectConsumableType;
	public string InteractionHintMessageAction {  get; protected set; }

	public string InteractionHintMessageFail => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageFailActive => throw new System.NotImplementedException();

	private void Start()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		ConsumableObjectCollider = GetComponent<Collider>();
		GameObjectPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_interactionObjectConsumableType = _interactionObjectConsumableTypes.ToString();
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void ChangeLanguage()
	{

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
				//_playerResourcesHealthManager.
				Destroy(gameObject);
				break;
			}

			currentSpeed += speedIncrease * Time.deltaTime;
			yield return null;
		}
	}




	public void InteractCutscene()
	{
		throw new System.NotImplementedException();
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
