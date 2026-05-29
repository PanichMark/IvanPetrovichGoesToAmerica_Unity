using UnityEngine;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	// [SerializeField] позволяет перетащить ассет KeyData прямо в инспектор для этого объекта.
	[SerializeField] private InteractionObjectKeyData _keyData;
	private KeysManager _keysManager;
	// Свойства теперь берутся из _keyData
	//public override Sprite LootObjectIcon => _keyData.icon;
	public override string InteractionObjectNameSystem => _keyData.keyID; // Используем ID как системное имя

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
	}

	public override void Interact()
	{
		_keysManager.AddKey(_keyData.keyID);


		WasLootItemCollected = true;
		
		base.Interact(); // Запускает анимацию движения к игроку и уничтожение объекта
	}
}