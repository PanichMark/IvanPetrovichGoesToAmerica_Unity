using UnityEngine;

public class InteractionObjectLootValuable : InteractionObjectLootAbstract
{
	
	[SerializeField] private int moneyValue;

	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	


	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}, recieved {moneyValue} rubles");
	
		playerResourcesMoneyManager.AddMoney(moneyValue);
		WasLootItemCollected = true;
	}

	protected override void ThisMethodSetsActionName()
	{
	
		playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}

