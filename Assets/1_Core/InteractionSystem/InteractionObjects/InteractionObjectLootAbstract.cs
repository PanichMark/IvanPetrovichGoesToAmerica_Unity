using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IInteractGainedItem, ISaveLoad
{
	public delegate void InteractionDelegate();

	[SerializeField] protected string _interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;
	public Collider Collider { get; protected set; }

	public virtual string InteractionObjectNameUI { get; protected set; }

	public Sprite LootObjectImage;

	public virtual string InteractionHintMessageMain => $"{InteractionHintAction} {InteractionObjectNameUI}";
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		Collider = GetComponent<Collider>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("PlayerGameObject");

		InteractionHintAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
		_localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionHintAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
	}

	protected virtual void ThisMethodSetsActionName()
	{
	}

	public string InteractionHintAction { get; protected set; }

	public virtual bool WasLootItemCollected { get; protected set; }

	public int LootItemIndex { get; protected set; }

	public TextMeshProUGUI GainedItemtext => null;

	public virtual Sprite ImageGainedItem => LootObjectImage;

	internal void AssignLootItemIndex(int index)
	{
		LootItemIndex = index;
	}

	public virtual void Interact()
	{
		if (CachedPlayer != null)
		{
			Collider.enabled = false;
			gameObject.tag = "Untagged";
			StartCoroutine(MoveTowardsTarget());
		}
		else
		{
			Debug.Log("Player not found!");
		}
	}

	public GameObject CachedPlayer { get; protected set; }

	IEnumerator MoveTowardsTarget()
	{
		while (true)
		{
			Vector3 targetPosition = CachedPlayer.transform.position + Vector3.up * 1f;
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
			data.LootObjects_Scene_0_Test[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootObjects_Scene_0_Test[LootItemIndex].LootItemName = InteractionObjectNameSystem;
			data.LootObjects_Scene_0_Test[LootItemIndex].WasLootItemCollected = WasLootItemCollected;
		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
		{
			data.LootObjects_Scene_1_StreetMain[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootObjects_Scene_1_StreetMain[LootItemIndex].LootItemName = InteractionObjectNameSystem;
			data.LootObjects_Scene_1_StreetMain[LootItemIndex].WasLootItemCollected = WasLootItemCollected;
		}
	}

	public void LoadData(GameData data)
	{
		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			if (data.LootObjects_Scene_0_Test[LootItemIndex].WasLootItemCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
		{
			if (data.LootObjects_Scene_1_StreetMain[LootItemIndex].WasLootItemCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
	}
}