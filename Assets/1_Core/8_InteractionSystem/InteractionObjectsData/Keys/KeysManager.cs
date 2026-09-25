using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysManager: MonoBehaviour, IJsonSaveLoad
{ 
	private List<string> _collectedKeys = new List<string>();

	public List<string> CollectedKeys => _collectedKeys;

	public bool HasKey(string keyId) => _collectedKeys.Contains(keyId);

	public void Initialize()
	{
		Debug.Log("KeysManager Initialized");
	}

	public void AddKey(string keyId)
	{
		if (!_collectedKeys.Contains(keyId))
		{
			_collectedKeys.Add(keyId);
		}

		//Debug.Log(_collectedKeys.Count);
	}

	public void RemoveKey(string keyId)
	{
		_collectedKeys.Remove(keyId);
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.PlayerKeys = _collectedKeys;

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		_collectedKeys.Clear();

		_collectedKeys = data.PlayerKeys;

		yield return null;
	}
}