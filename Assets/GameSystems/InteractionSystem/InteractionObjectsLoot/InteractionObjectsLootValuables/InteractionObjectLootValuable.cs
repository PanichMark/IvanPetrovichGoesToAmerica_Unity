using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	[SerializeField]
	private int _moneyValue;

	

	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}, получаете {_moneyValue} рублей");
		Destroy(gameObject);
		//PlayerMoneyManager.Instance.AddMoney(MoneyValue);
		WasLootItemCollected = true;
	}
}

