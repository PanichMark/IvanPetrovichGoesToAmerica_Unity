using UnityEngine;

public class InteractionObjectLootWeapon : InteractionObjectLootAbstract
{
	[SerializeField] private GameObject weaponObject;

	private PlayerWeaponController weaponController;

	private void Awake()
	{
		weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		var weaponComponent = weaponObject.GetComponent<WeaponAbstract>();
		LootObjectImage = weaponComponent.WeaponIcon;
	}

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"You picked up {InteractionObjectNameUI}");
		weaponController.UnlockWeapon(weaponObject);
	}

	protected override void ThisMethodSetsActionName()
	{
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}
}