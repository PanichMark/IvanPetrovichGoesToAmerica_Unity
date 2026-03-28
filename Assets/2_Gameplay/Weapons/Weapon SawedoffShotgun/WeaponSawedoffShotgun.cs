using UnityEngine;

public class WeaponSawedoffShotgun : RangedWeaponAbstract
{

	public override float WeaponDamage => 100f;
	protected override void InitializeWeapon()
	{
		WeaponAmmoType = AmmoTypes.Ammo12gauge;
		// Здесь мы задаем параметры КОНКРЕТНО для этого револьвера
		PlayerAmmoMagazineMax = 2; // Барабан на 6 патронов
		PlayerAmmoMagazineCurrent = 2; // Начинаем с полным барабаном

		// Тип патронов тоже задается здесь (или можно через инспектор)
	}

	public override string WeaponNameSystem => "SawedOffShotgun";
	public override string WeaponNameUI => "Дробовик Обрез";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Shotgun Icon");

}

