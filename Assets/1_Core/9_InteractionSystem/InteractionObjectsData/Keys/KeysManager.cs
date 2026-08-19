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
		// Используем Scene_0_Test как единый глобальный контейнер для ключей
		if (!data.KeysByScene.ContainsKey(GameScenesEnum.Scene_0_Test))
		{
			data.KeysByScene[GameScenesEnum.Scene_0_Test] = new List<KeyData>();
		}

		var targetList = data.KeysByScene[GameScenesEnum.Scene_0_Test];
		targetList.Clear();

		foreach (string keyId in _collectedKeys)
		{
			targetList.Add(new KeyData { KeyName = keyId });
		}
	}

	public void LoadData(GameData data)
	{
		//throw new System.NotImplementedException();
	}
}