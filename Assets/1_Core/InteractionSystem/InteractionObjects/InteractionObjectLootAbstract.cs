using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IGainedItem, ISaveLoad
{
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
	public virtual string InteractionObjectNameUI { get; protected set; }

	public Sprite LootObjectIcon { get; protected set; }

	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		LootObjectCollider = GetComponent<Collider>();
		GameObjectPlayer = ServiceLocator.Resolve<GameObject>("PlayerGameObject");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
		_localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
	}

	protected virtual void ThisMethodSetsActionName()
	{

	}

	internal void AssignLootObjectsIndex(int index)
	{
		LootObjectIndex = index;
	}

	public virtual void Interact()
	{
		if (GameObjectPlayer != null)
		{
			LootObjectCollider.enabled = false;
			gameObject.tag = "Untagged";
			StartCoroutine(MoveTowardsPlayer());
		}
		else
		{
			Debug.Log("Player not found!");
		}
	}

	IEnumerator MoveTowardsPlayer()
	{
		while (true)
		{
			Vector3 targetPosition = GameObjectPlayer.transform.position + Vector3.up * 1f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				Destroy(gameObject);
				break;
			}

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

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
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

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
		{
			if (data.LootObjects_Scene_1_StreetMain[LootObjectIndex].WasLootItemCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
	}
}