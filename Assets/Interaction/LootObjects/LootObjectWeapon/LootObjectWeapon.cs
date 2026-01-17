using UnityEngine;

public class LootObjectWeapon : LootObjectAbstract
{
	[SerializeField] private GameObject weaponObject;

	private WeaponController weaponController;

	private void Awake()
	{
		// Запрашиваем оружие-контроллер, используя уникальный ключ
		weaponController = ServiceLocator.Resolve<WeaponController>("WeaponController");
	}

	public override void Interact()
	{
		Debug.Log($"Вы подняли {InteractionObjectNameUI}");
		weaponController.UnlockWeapon(weaponObject);
		Destroy(gameObject);
	}
}

