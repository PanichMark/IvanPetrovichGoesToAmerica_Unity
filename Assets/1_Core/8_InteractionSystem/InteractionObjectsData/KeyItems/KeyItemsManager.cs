using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemsManager: MonoBehaviour, IJsonSaveLoad
{ 
	private List<string> _collectedKeyItems = new List<string>();

	public List<string> CollectedKeyItems => _collectedKeyItems;

	public bool HasKey(string keyItemID) => _collectedKeyItems.Contains(keyItemID);

	public void Initialize()
	{
		Debug.Log("KeyItemsManager Initialized");
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