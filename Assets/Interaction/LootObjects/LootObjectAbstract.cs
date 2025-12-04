using UnityEngine;
using System;
using Unity.IO.LowLevel.Unsafe;

public abstract class LootObjectAbstract : MonoBehaviour, IInteractable, IDataPersistence
{
	[SerializeField]
	private string _interactionItemNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionItemNameSystem;


	[SerializeField]
	private string _interactionItemNameUI;
	public virtual string InteractionObjectNameUI => _interactionItemNameUI;

	public virtual string MainInteractionHint => $"Поднять {InteractionObjectNameUI}";
	public virtual string AdditionalInteractionHint => null;
	public virtual bool IsAdditionalInteractionHintActive => false;


	[SerializeField]
	private int _moneyValue;

	public virtual int MoneyValue => _moneyValue;

	public virtual bool WasLootItemCollected { get; protected set; }


	// Поле для внутреннего индекса и хранения типа предмета
	public int LootItemIndex { get; protected set; }

	internal void AssignLootItemIndex(int index)
	{
		LootItemIndex = index;
	}

	public abstract void Interact();

	public void SaveData(ref GameData data)
	{


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


	}

	public void LoadData(GameData data)
	{
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
	}
}