using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectLootKeyItem : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectKeyItemData _keyItemData;
	private KeyItemsManager _keysItemsManager;
	private string _keyItemID;

	protected override bool _shouldShowGainedItem => true;


	protected override void InitializeLootObject()
	{
		_keysItemsManager = ServiceLocator.Resolve<KeyItemsManager>();
		_keyItemID = _keyItemData.KeyItemID.ToString();
	}

	public override void Interact()
	{
		base.Interact();

		_keysItemsManager.AddKeyItemToCollected(_keyItemID);
		Debug.Log($"Added key: {_keyItemID}");
	}
}