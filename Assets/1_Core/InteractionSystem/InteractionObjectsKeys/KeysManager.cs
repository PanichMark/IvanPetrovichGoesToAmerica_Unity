using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyInventory", menuName = "Managers/Key Inventory Manager")]
public class KeysManager
{ 
	[SerializeField] private List<string> _collectedKeys = new List<string>();

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
	}

	public void RemoveKey(string keyId)
	{
		_collectedKeys.Remove(keyId);
	}
}