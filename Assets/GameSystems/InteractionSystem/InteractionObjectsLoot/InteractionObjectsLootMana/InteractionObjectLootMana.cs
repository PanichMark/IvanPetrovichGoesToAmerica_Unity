using UnityEngine;

public class InteractionObjectLootMana : InteractionObjectLootAbstract
{
	private bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;
	private PlayerResourcesManaManager playerResourcesManaManager;


	public override string InteractionHintMessageAdditional => $"Максимум {InteractionObjectNameUI}";




	public override void Interact()
	{
		if (playerResourcesManaManager.CurrentManaReplenishItemsNumber < 9)
		{
			Debug.Log($"Вы подняли {InteractionObjectNameUI}");
			Destroy(gameObject);
			playerResourcesManaManager.AddManaReplenishItem();
			isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		//else Debug.Log("Can't pick up more ManaReplenish Items");
		else isAdditionalInteractionHintActive = true;
	}

	protected override void ThisMethodSetsActionName()
	{
	
		playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}

