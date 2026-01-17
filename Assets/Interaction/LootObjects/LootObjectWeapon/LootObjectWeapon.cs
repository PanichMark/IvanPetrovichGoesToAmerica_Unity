using UnityEngine;

public class LootObjectWeapon : LootObjectAbstract
{
	[SerializeField] private GameObject weaponObject;

	private WeaponController weaponController;

	private void Awake()
	{
		// Обращаемся к Service Locator и получаем контроллер оружия
		weaponController = ServiceLocator.Resolve<WeaponController>();
		
	}

	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}");
		weaponController.UnlockWeapon(weaponObject);
		Destroy(gameObject);
	}
}
