using System.Collections.Generic;
using UnityEngine;

public class KeysManager: ISaveLoad
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

	public void SaveData(ref GameData data)
	{
		//data.CollectedKeys.Clear();

		foreach (string keyId in _collectedKeys)
		{
			data.CollectedKeys.Add(new KeyData { KeyName = keyId, WasKeyCollected = true });
		}
	}

	public void LoadData(GameData data)
	{
		//throw new System.NotImplementedException();
	}
}