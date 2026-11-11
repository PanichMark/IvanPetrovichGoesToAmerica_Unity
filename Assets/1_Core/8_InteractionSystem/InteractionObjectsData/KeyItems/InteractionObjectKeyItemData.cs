using UnityEngine;

[CreateAssetMenu(fileName = "KeyItemData", menuName = "InteractionObjects/KeyItems/KeyItemData")]
public class InteractionObjectKeyItemData : ScriptableObject
{
	public InteractionObjectsKeyItemsNamesEnum KeyItemID;

	public Sprite KeyItemSprite;
	public string KeyItemName;

	public TextAsset KeyItemDescription_RU;
	public TextAsset KeyItemDescription_EN;
}