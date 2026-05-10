using UnityEngine;

public class InteractionObjectsLootAmmo : InteractionObjectLootAbstract
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;

	[SerializeField] private AmmoTypes _ammoTypes;
	[SerializeField] private int _AmmoCapacity;

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}");
		_playerResourcesAmmoManager.ModifyReserveAmmo(_ammoTypes, _AmmoCapacity);
	}

	protected override void ThisMethodSetsActionName()
	{
		_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
	}
}