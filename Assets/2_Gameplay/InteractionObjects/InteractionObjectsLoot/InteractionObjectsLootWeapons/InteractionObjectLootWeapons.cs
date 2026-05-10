using UnityEngine;

public class InteractionObjectLootWeapon : InteractionObjectLootAbstract
{
	[SerializeField] private GameObject _weaponObject;

	private PlayerWeaponController _weaponController;

	private void Awake()
	{
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		var weaponComponent = _weaponObject.GetComponent<WeaponAbstract>();
		LootObjectImage = weaponComponent.WeaponIcon;
	}

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"You picked up {InteractionObjectNameUI}");
		_weaponController.UnlockWeapon(_weaponObject);
	}

	protected override void ThisMethodSetsActionName()
	{
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}