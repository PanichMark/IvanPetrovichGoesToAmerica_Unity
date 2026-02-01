using UnityEngine;
using System;
using Unity.IO.LowLevel.Unsafe;
using TMPro;
using UnityEngine.UI;

public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IInteractGainedItem, ISaveLoad
{
	
	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;
	protected LocalizationManager localizationManager;

	
	public virtual string InteractionObjectNameUI {  get; protected set; }

	public Sprite LootObjectImage;

	public virtual string InteractionHintMessageMain => $"{InteractionHintAction} {InteractionObjectNameUI}";
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	private void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		
		
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
	}


	protected virtual void ThisMethodSetsActionName()
	{

	}
	public string InteractionHintAction { get; protected set; }

	public virtual bool WasLootItemCollected { get; protected set; }


	// Поле для внутреннего индекса и хранения типа предмета
	public int LootItemIndex { get; protected set; }

	public TextMeshProUGUI GainedItemtext => null;

	public virtual Sprite GainedItemImage => LootObjectImage;

	internal void AssignLootItemIndex(int index)
	{
		LootItemIndex = index;
	}

	public abstract void Interact();

	public void SaveData(ref GameData data)
	{

		/*
		if (GameSceneManager.Instance.CurrentSceneSystemName == "SceneTEST")
		{
			data.LootItemsSceneTEST[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootItemsSceneTEST[LootItemIndex].LootItemName = InteractionObjectNameSystem;

			if (WasLootItemCollected == true)
			{
				data.LootItemsSceneTEST[LootItemIndex].WasLootItemCollected = true;
			}
			else data.LootItemsSceneTEST[LootItemIndex].WasLootItemCollected = false;

		}

		if (GameSceneManager.Instance.CurrentSceneSystemName == "Scene1")
		{
			data.LootItemsScene1[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootItemsScene1[LootItemIndex].LootItemName = InteractionObjectNameSystem;

			if (WasLootItemCollected == true)
			{
				data.LootItemsScene1[LootItemIndex].WasLootItemCollected = true;
			}
			else data.LootItemsScene1[LootItemIndex].WasLootItemCollected = false;

		}
		*/

	}


	public void LoadData(GameData data)
	{
		/*
		if (GameSceneManager.Instance.CurrentSceneSystemName == "SceneTEST")
		{
			if (data.LootItemsSceneTEST[LootItemIndex].WasLootItemCollected == true)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}

		if (GameSceneManager.Instance.CurrentSceneSystemName == "Scene1")
		{
			if (data.LootItemsScene1[LootItemIndex].WasLootItemCollected == true)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
		*/
	}
}

