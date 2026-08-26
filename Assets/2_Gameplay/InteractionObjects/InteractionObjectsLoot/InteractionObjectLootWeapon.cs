using UnityEngine;

public class InteractionObjectLootWeapon : InteractionObjectLootAbstract
{
	[SerializeField] Sprite _lootObjectIcon;
	public override Sprite LootObjectIcon => _lootObjectIcon;

	[SerializeField] private GameObject _weapon;
	[SerializeField] private InteractionObjectNote _noteObject;

	private PlayerWeaponController _playerWeaponController;


	public override void Interact()
	{
		base.Interact();
		Debug.Log($"You picked up {InteractionObjectNameUI}");
		_playerWeaponController.UnlockWeapon(_weapon);
	}

	public override void InteractCutscene()
	{
		base.InteractCutscene();
		Debug.Log($"You picked up {InteractionObjectNameUI}");
		_playerWeaponController.UnlockWeapon(_weapon);
	}

	protected override void OnAfterLooted()
	{
		_noteObject.Interact();
	}

	protected override void InitializeLootObject()
	{
		_playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>();

		var weaponComponent = _weapon.GetComponent<WeaponAbstract>();
		LootObjectIcon = weaponComponent.WeaponIconBig;
	}
}