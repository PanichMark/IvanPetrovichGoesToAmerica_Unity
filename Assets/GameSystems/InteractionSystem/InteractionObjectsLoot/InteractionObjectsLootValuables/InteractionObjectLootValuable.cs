using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}, получаете {MoneyValue} рублей");
		Destroy(gameObject);
		//PlayerMoneyManager.Instance.AddMoney(MoneyValue);
		WasLootItemCollected = true;
	}
}

