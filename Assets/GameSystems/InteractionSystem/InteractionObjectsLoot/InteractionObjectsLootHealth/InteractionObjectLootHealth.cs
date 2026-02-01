using UnityEngine;

public class InteractionObjectLootHealth : InteractionObjectLootAbstract
{
	private PlayerResourcesHealthManager playerResourcesHealthManager;
	private bool isAdditionalInteractionHintActive;
	public override bool IsAdditionalInteractionHintMessageActive => isAdditionalInteractionHintActive;


	public override string InteractionObjectNameSystem => "HealingItem";
	public override string InteractionObjectNameUI => "Лечащий предмет";

	public override string AdditionalInteractionHintMessage => $"Максимум {InteractionObjectNameUI}";

	private void Awake()
	{
		playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");
	}

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

	
}

