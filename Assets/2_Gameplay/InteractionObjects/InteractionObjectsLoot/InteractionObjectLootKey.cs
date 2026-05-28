using UnityEngine;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	// [SerializeField] позволяет перетащить ассет KeyData прямо в инспектор для этого объекта.
	[SerializeField] private InteractionObjectKeyData _keyData;

	// Свойства теперь берутся из _keyData
	//public override Sprite LootObjectIcon => _keyData.icon;
	public override string InteractionObjectNameSystem => _keyData.keyID; // Используем ID как системное имя

	protected override void SetUpLootObjectReferences()
	{
		WasLootItemCollected = false;

		// Если иконка не задана в базовом классе, можно попробовать взять её отсюда
		if (LootObjectIcon != null)
		{
			//base.LootObjectIcon = _keyData.icon;
		}
	}

	public override void Interact()
	{
		if (!WasLootItemCollected && _keyData != null) // Проверяем, что данные есть
		{
			// Передаем в менеджер ID ключа из ScriptableObject
			InteractionObjectsKeysManager.Instance.AddKey(_keyData.keyID);

			WasLootItemCollected = true;
		}
		base.Interact(); // Запускает анимацию движения к игроку и уничтожение объекта
	}
}