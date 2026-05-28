using UnityEngine;

// [CreateAssetMenu] позволяет создать этот объект через меню Assets -> Create
[CreateAssetMenu(fileName = "KeyData", menuName = "InteractionObjects/Keys/KeyData")]
public class InteractionObjectKeyData : ScriptableObject
{
	public string keyID; // Уникальный ID (например: "key_house_01")
}