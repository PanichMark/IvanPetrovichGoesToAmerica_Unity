using UnityEngine;

public class LootObjectWeapon : LootObjectAbstract
{
	public override int MoneyValue => 0;



	[SerializeField] private WeaponController weaponController;

	[SerializeField] private GameObject weaponObject;


	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}");
		weaponController.UnlockWeapon(weaponObject);
		Destroy(gameObject);
	}
}
