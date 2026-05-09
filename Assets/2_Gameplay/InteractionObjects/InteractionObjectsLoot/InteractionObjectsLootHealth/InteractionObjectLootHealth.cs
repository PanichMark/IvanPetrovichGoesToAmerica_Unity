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
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			playerResourcesHealthManager.AddHealingItem();
			isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		else
		{
			isAdditionalInteractionHintActive = true;
		}
	}

	protected override void ThisMethodSetsActionName()
	{
		playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}