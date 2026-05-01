using UnityEngine;

public class InteractionObjectLootWeapon : InteractionObjectLootAbstract
{
	[SerializeField] private GameObject weaponObject;


	private PlayerWeaponController weaponController;

	private void Awake()
	{
		// Запрашиваем оружие-контроллер, используя уникальный ключ
		weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		// Получаем компонент конкретного оружия с нашего объекта
		var weaponComponent = weaponObject.GetComponent<WeaponAbstract>();

		// Сохраняем ссылку на иконку оружия в поле GainedItemImage
		
			LootObjectImage = weaponComponent.WeaponIcon;
		
	}

	public override void Interact()
	{
		base.Interact();
		Debug.Log($"Вы подняли {InteractionObjectNameUI}");
		weaponController.UnlockWeapon(weaponObject);
		
	}

	protected override void ThisMethodSetsActionName()
	{

		
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
	}

}

