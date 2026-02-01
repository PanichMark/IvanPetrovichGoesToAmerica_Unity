using UnityEngine;

public class InteractionObjectLootHealth : InteractionObjectLootAbstract
{
	private PlayerResourcesHealthManager playerResourcesHealthManager;
	private bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;



	

	public override string InteractionHintMessageAdditional => $"Максимум {InteractionObjectNameUI}";

	

	public override void Interact()
	{
		if (playerResourcesHealthManager.CurrentHealingItemsNumber < 9)
		{
			Debug.Log($"Вы подняли {InteractionObjectNameUI}");
			Destroy(gameObject);
			playerResourcesHealthManager.AddHealingItem();
			isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		//else Debug.Log("Can't pick up more Healing Items");
		else isAdditionalInteractionHintActive = true;
	}

	protected override void ThisMethodSetsActionName()
	{
		playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");
	
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}

