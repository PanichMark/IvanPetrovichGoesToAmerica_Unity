using UnityEngine;

public class LootObjectManaReplenishItem : LootObjectAbstract
{
	private bool isAdditionalInteractionHintActive;
	public override bool IsAdditionalInteractionHintActive => isAdditionalInteractionHintActive;
	public override int MoneyValue => 0;

	public override string InteractionObjectNameSystem => "ManaReplenishItem";
	public override string InteractionObjectNameUI => "Предмет восстаналивает ману";

	public override string AdditionalInteractionHint => $"Максимум {InteractionObjectNameUI}";

	public override void Interact()
	{
		if (PlayerManaManager.Instance.CurrentManaReplenishItemsNumber < 9)
		{
			Debug.Log($"Вы подняли {InteractionObjectNameUI}");
			Destroy(gameObject);
			PlayerManaManager.Instance.AddManaReplenishItem();
			isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		//else Debug.Log("Can't pick up more ManaReplenish Items");
		else isAdditionalInteractionHintActive = true;
	}

	
}