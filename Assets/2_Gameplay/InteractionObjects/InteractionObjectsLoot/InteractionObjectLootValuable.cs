using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	[SerializeField] Sprite _lootObjectIcon;
	[SerializeField] private int _moneyValue;

	private PlayerMoneyController _playerResourcesMoneyManager;

	public override Sprite LootObjectIcon => _lootObjectIcon;
	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}, received {_moneyValue} rubles");

		_playerResourcesMoneyManager.AddMoney(_moneyValue);
	}

	public override void InteractCutscene()
	{
		base.InteractCutscene();
		Debug.Log($"Picked up {InteractionObjectNameUI}, received {_moneyValue} rubles");

		_playerResourcesMoneyManager.AddMoney(_moneyValue);
	}

	protected override void InitializeLootObject()
	{
_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerMoneyController>();
	}
}