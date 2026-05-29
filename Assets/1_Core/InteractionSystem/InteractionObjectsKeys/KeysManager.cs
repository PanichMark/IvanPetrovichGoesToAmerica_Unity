using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyInventory", menuName = "Managers/Key Inventory Manager")]
public class KeysManager
{ 
	private List<string> _collectedKeys = new List<string>();

	public List<string> CollectedKeys => _collectedKeys;

	public bool HasKey(string keyId) => _collectedKeys.Contains(keyId);

	public KeysManager()
	{
		Debug.Log("KeysManager Initialized");
	}

	public void AddKey(string keyId)
	{
		if (!_collectedKeys.Contains(keyId))
		{
			_collectedKeys.Add(keyId);
		}

		Debug.Log(_collectedKeys.Count);
	}

	public void RemoveKey(string keyId)
	{
		_collectedKeys.Remove(keyId);
	}
}