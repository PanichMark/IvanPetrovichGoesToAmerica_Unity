using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
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

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
	}

	protected virtual void SetUpLootObjectReferences()
	{

	}

	public void AssignLootObjectsIndexes(int index)
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
		List<LootObjectData> targetList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			targetList = data.LootObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Church))
		{
			targetList = data.LootObjects_Scene_1_Church;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			targetList = data.LootObjects_Scene_1_Street;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_RevenueHouse))
		{
			targetList = data.LootObjects_Scene_1_RevenueHouse;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_InnerYard))
		{
			targetList = data.LootObjects_Scene_1_InnerYard;
		}

		if (targetList != null)
		{
			int indexInList = targetList.FindIndex(item => item.LootObjectIndex == LootObjectIndex);

			if (indexInList != -1)
			{
				LootObjectData updatedItem = new LootObjectData
				{
					LootObjectIndex = LootObjectIndex,
					LootObjectNameSystem = InteractionObjectNameSystem,
					WasLootObjectCollected = WasLootItemCollected 
				};

				targetList[indexInList] = updatedItem;
			}
			else
			{
				targetList.Add(new LootObjectData
				{
					LootObjectIndex = LootObjectIndex,
					LootObjectNameSystem = InteractionObjectNameSystem,
					WasLootObjectCollected = WasLootItemCollected
				});
			}
		}
	}

	public void LoadData(GameData data)
	{
		List<LootObjectData> sourceList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			sourceList = data.LootObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Church))
		{
			sourceList = data.LootObjects_Scene_1_Church;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			sourceList = data.LootObjects_Scene_1_Street;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_RevenueHouse))
		{
			sourceList = data.LootObjects_Scene_1_RevenueHouse;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_InnerYard))
		{
			sourceList = data.LootObjects_Scene_1_InnerYard;
		}

		if (sourceList != null && sourceList.Count > 0)
		{
			LootObjectData savedState = sourceList.Find(item => item.LootObjectIndex == LootObjectIndex);

			if (savedState.LootObjectIndex != 0 && savedState.WasLootObjectCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
	}
}