using UnityEngine;

public class InteractionObjectLootAmmo : InteractionObjectLootAbstract
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	[SerializeField] Sprite _lootObjectIcon;
	public override Sprite LootObjectIcon => _lootObjectIcon;
	[SerializeField] private AmmoTypes _ammoTypes;
	[SerializeField] private int _ammoCapacity;

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Picked up {InteractionObjectNameUI}");
		_playerResourcesAmmoManager.AddAmmoToReserve(_ammoTypes, _ammoCapacity);
	}

	protected override void ThisMethodSetsActionName()
	{
		_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
	}
}