using UnityEngine;

[CreateAssetMenu(fileName = "KeyItemsList", menuName = "InteractionObjects/KeyItems/KeyItemsList")]
public class InteractionObjectsKeyItemsList : ScriptableObject
{
	public InteractionObjectKeyItemData[] KeyItemsInOrder;
}
