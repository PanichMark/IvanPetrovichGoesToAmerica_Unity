using UnityEngine;

public class InteractionObjectLootMana : InteractionObjectLootAbstract
{
	private bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;
	private PlayerResourcesManaManager playerResourcesManaManager;

	public override string InteractionHintMessageAdditional => $"Maximum {InteractionObjectNameUI}";

	public override void Interact()
	{
		if (playerResourcesManaManager.CurrentManaReplenishItemsNumber < 9)
		{
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			playerResourcesManaManager.AddManaReplenishItem();
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
		playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}