using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyInventory", menuName = "Managers/Key Inventory Manager")]
public class InteractionObjectsKeysManager : ScriptableObject
{
	private static InteractionObjectsKeysManager _instance;
	public static InteractionObjectsKeysManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = Resources.Load<InteractionObjectsKeysManager>("Managers/KeyInventory");
			return _instance;
		}
	}

	[SerializeField]
	private List<string> _collectedKeys = new List<string>();

	public bool HasKey(string keyId) => _collectedKeys.Contains(keyId);

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