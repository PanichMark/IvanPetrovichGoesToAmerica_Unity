using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyItemsManager: MonoBehaviour, IJsonSaveLoad
{
	private JournalMenuController _journalMenuController;
	private JournalMenuSectionInventoryController _journalMenuSectionInventoryController;

	private List<string> _collectedKeyItems = new List<string>();

	public List<string> CollectedKeyItems => _collectedKeyItems;

	public bool HasKey(string keyItemID) => _collectedKeyItems.Contains(keyItemID);

	public void Initialize(
		JournalMenuController journalMenuController,
		JournalMenuSectionInventoryController journalMenuSectionInventoryController)
	{
		_journalMenuController = journalMenuController;
		_journalMenuSectionInventoryController = journalMenuSectionInventoryController;

		_journalMenuController.OnOpenJournaMenuSectionInventory += SendKeyItemsDataToInventory;

		Debug.Log("KeyItemsManager Initialized");
	}

	private void SendKeyItemsDataToInventory()
	{
		if (_journalMenuController == null) return;

		// Находим контроллер инвентаря
		_journalMenuSectionInventoryController = _journalMenuController.GetComponent<JournalMenuSectionInventoryController>();
		if (_journalMenuSectionInventoryController == null) return;

		// Сортируем собранные ID по порядку, заданному в enum
		var sortedItems = _collectedKeyItems
			.OrderBy(id => (int)System.Enum.Parse(typeof(InteractionObjectsKeyItemsNamesEnum), id))
			.ToList();

		// Отправляем данные в инвентарь
		_journalMenuSectionInventoryController.ReceiveKeyItemsData(sortedItems);
	}

	public void AddKeyItemToCollected(string keyItemID)
	{
		if (!_collectedKeyItems.Contains(keyItemID))
		{
			_collectedKeyItems.Add(keyItemID);
		}

		//Debug.Log(_collectedKeys.Count);
	}

	public void RemoveKeyItemFromCollected(string keyItemID)
	{
		_collectedKeyItems.Remove(keyItemID);
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.PlayerKeyItems = _collectedKeyItems;

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		_collectedKeyItems.Clear();

		_collectedKeyItems = data.PlayerKeyItems;

		yield return null;
	}
}