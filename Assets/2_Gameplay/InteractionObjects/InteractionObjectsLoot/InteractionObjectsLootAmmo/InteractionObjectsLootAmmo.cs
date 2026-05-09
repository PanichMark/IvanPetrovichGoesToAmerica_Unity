using UnityEngine;

public class InteractionObjectsLootAmmo : InteractionObjectLootAbstract
{
	PlayerResourcesAmmoManager playerResourcesAmmoManager;

	[SerializeField] AmmoTypes ammoTypes;
	[SerializeField] int AmmoCapacity;

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}");
		playerResourcesAmmoManager.ModifyReserveAmmo(ammoTypes, AmmoCapacity);
	}

	protected override void ThisMethodSetsActionName()
	{
		playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
	}
}