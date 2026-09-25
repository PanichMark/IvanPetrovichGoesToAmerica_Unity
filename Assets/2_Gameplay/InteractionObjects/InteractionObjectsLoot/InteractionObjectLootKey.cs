using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectKeyData _keyData;
	private KeysManager _keysManager;
	private string _keyID;

	protected override bool _shouldShowGainedItem => true;


	protected override void InitializeLootObject()
	{
		_keysManager = ServiceLocator.Resolve<KeysManager>();
		_keyID = _keyData.keyID.ToString();
	}

	public override void Interact()
	{
		base.Interact();

		_keysManager.AddKey(_keyID);
		Debug.Log($"Added key: {_keyID}");
	}
}