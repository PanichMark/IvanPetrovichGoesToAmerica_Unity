using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IGainedItem, ISaveLoad
{
	public event IInteractable.InteractableObjectHandler OnInteract;
	[SerializeField] protected string _interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;

	public GameObject GameObjectPlayer { get; protected set; }
	protected LocalizationManager _localizationManager;
	public Collider LootObjectCollider { get; protected set; }
	public string InteractionHintMessageAction { get; protected set; }

	public virtual bool WasLootItemCollected { get; protected set; }

	public int LootObjectIndex { get; protected set; }

	public TextMeshProUGUI NameGainedItem => null;

	public virtual Sprite IconGainedItem => LootObjectIcon;
	public virtual string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameSystem)}";

	public virtual Sprite LootObjectIcon { get; protected set; }

	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public virtual string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Max")} {InteractionObjectNameUI}!";
	public virtual bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		LootObjectCollider = GetComponent<Collider>();
		GameObjectPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
		SetUpLootObjectReferences();
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public virtual void Interact()
	{
		LootObjectCollider.enabled = false;
		gameObject.tag = "Untagged";
		StartCoroutine(MoveTowardsPlayer());
		OnInteract?.Invoke();
	}

	public virtual void InteractCutscene()
	{
		Destroy(gameObject);
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
	}

	protected virtual void SetUpLootObjectReferences()
	{

	}

	internal void AssignLootObjectsIndex(int index)
	{
		LootObjectIndex = index;
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
				Destroy(gameObject);
				break;
			}

			currentSpeed += speedIncrease * Time.deltaTime;
			yield return null;
		}
	}

	public void SaveData(ref GameData data)
	{
		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			data.LootObjects_Scene_0_Test[LootObjectIndex].LootItemIndex = LootObjectIndex;
			data.LootObjects_Scene_0_Test[LootObjectIndex].LootItemName = InteractionObjectNameSystem;
			data.LootObjects_Scene_0_Test[LootObjectIndex].WasLootItemCollected = WasLootItemCollected;
		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			data.LootObjects_Scene_1_StreetMain[LootObjectIndex].LootItemIndex = LootObjectIndex;
			data.LootObjects_Scene_1_StreetMain[LootObjectIndex].LootItemName = InteractionObjectNameSystem;
			data.LootObjects_Scene_1_StreetMain[LootObjectIndex].WasLootItemCollected = WasLootItemCollected;
		}
	}

	public void LoadData(GameData data)
	{
		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			if (data.LootObjects_Scene_0_Test[LootObjectIndex].WasLootItemCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			if (data.LootObjects_Scene_1_StreetMain[LootObjectIndex].WasLootItemCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
	}
}