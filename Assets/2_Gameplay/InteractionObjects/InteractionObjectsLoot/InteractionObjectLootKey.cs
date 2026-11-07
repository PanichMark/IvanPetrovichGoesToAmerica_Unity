using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectKeyItemData _keyData;
	private KeyItemsManager _keysManager;
	private string _keyID;

	protected override bool _shouldShowGainedItem => true;


	protected override void InitializeLootObject()
	{
		_keysManager = ServiceLocator.Resolve<KeyItemsManager>();
		_keyID = _keyData.KeyItemID.ToString();
	}

	public override void Interact()
	{
		base.Interact();

		_keysManager.AddKeyItemToCollected(_keyID);
		Debug.Log($"Added key: {_keyID}");
	}
}