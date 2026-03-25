using UnityEngine;

public class InteractionObjectsLootAmmo : InteractionObjectLootAbstract
{

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}");

	}

}
