using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	[SerializeField] Sprite _lootObjectIcon;
	[SerializeField] private int _moneyValue;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;

	public override Sprite LootObjectIcon => _lootObjectIcon;
	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}, received {_moneyValue} rubles");

		_playerResourcesMoneyManager.AddMoney(_moneyValue);
		WasLootItemCollected = true;
	}

	public override void InteractCutscene()
	{
		base.InteractCutscene();
		Debug.Log($"Picked up {InteractionObjectNameUI}, received {_moneyValue} rubles");

		_playerResourcesMoneyManager.AddMoney(_moneyValue);
		WasLootItemCollected = true;
	}

	protected override void SetUpLootObjectReferences()
	{
		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
	}
}