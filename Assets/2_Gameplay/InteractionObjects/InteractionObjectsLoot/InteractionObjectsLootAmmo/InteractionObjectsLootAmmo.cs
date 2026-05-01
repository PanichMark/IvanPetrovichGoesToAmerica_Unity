using UnityEngine;

public class InteractionObjectsLootAmmo : InteractionObjectLootAbstract
{
	PlayerResourcesAmmoManager playerResourcesAmmoManager;
	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}");
		playerResourcesAmmoManager.ModifyReserveAmmo(ammoTypes, AmmoCapacity);
	}
	[SerializeField] AmmoTypes ammoTypes;
	[SerializeField] int AmmoCapacity;
	protected override void ThisMethodSetsActionName()
	{
		playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("playerResourcesAmmoManager");
	}

}
