using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeysList", menuName = "InteractionObjects/Keys/KeysList")]
public class InteractionObjectsKeysList : ScriptableObject
{
	public List<InteractionObjectKeyData> allKeysInGame = new List<InteractionObjectKeyData>();

	/// <summary>
	/// Метод для поиска ключа по ID в этом списке.
	/// </summary>
	public InteractionObjectKeyData GetKeyByID(string idToFind)
	{
		return allKeysInGame.Find(key => key.keyID == idToFind);
	}
}