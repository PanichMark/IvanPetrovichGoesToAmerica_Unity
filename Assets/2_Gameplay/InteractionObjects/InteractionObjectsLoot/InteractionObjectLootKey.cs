using UnityEngine;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectKeyData _keyData;
	private KeysManager _keysManager;
	private string _keyID;


	protected override void SetUpLootObjectReferences()
	{
		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
		_keyID = _keyData.keyID.ToString();
	}

	public override void Interact()
	{
		_keysManager.AddKey(_keyID);
		Debug.Log($"Added key: {_keyID}");

		WasLootItemCollected = true;
		
		base.Interact(); 
	}
}