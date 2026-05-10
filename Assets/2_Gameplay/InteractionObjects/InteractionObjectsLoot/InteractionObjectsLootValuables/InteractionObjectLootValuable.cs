using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	[SerializeField] private int _moneyValue;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}, received {_moneyValue} rubles");

		_playerResourcesMoneyManager.AddMoney(_moneyValue);
		WasLootItemCollected = true;
	}

	protected override void ThisMethodSetsActionName()
	{
		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}