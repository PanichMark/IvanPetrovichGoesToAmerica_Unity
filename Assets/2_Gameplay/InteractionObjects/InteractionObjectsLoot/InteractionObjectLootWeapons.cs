using UnityEngine;

public class InteractionObjectLootWeapon : InteractionObjectLootAbstract
{
	[SerializeField] Sprite _lootObjectIcon;
	public override Sprite LootObjectIcon => _lootObjectIcon;

	[SerializeField] private GameObject _weapon;

	private PlayerWeaponController _playerWeaponController;

	private void Awake()
	{
		_playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		var weaponComponent = _weapon.GetComponent<WeaponAbstract>();
		LootObjectIcon = weaponComponent.WeaponIcon;
	}

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"You picked up {InteractionObjectNameUI}");
		_playerWeaponController.UnlockWeapon(_weapon);
	}

	protected override void ThisMethodSetsActionName()
	{
		//InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}