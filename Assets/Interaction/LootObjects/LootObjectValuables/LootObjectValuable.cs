using UnityEngine;

public class LootObjectValuable : LootObjectAbstract
{
	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}, получаете {MoneyValue} рублей");
		Destroy(gameObject);
		PlayerMoneyManager.Instance.AddMoney(MoneyValue);
		WasLootItemCollected = true;
	}
}
