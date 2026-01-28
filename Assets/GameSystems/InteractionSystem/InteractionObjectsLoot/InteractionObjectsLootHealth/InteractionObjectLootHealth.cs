using UnityEngine;

public class InteractionObjectLootHealth : InteractionObjectLootAbstract
{
	private bool isAdditionalInteractionHintActive;
	public override bool IsAdditionalInteractionHintActive => isAdditionalInteractionHintActive;


	public override string InteractionObjectNameSystem => "HealingItem";
	public override string InteractionObjectNameUI => "Лечащий предмет";

	public override string AdditionalInteractionHint => $"Максимум {InteractionObjectNameUI}";


	public override void Interact()
	{
		if (PlayerHealthManager.Instance.CurrentHealingItemsNumber < 9)
		{
			Debug.Log($"Вы подняли {InteractionObjectNameUI}");
			Destroy(gameObject);
			PlayerHealthManager.Instance.AddHealingItem();
			isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		//else Debug.Log("Can't pick up more Healing Items");
		else isAdditionalInteractionHintActive = true;
	}

	
}

