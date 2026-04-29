using UnityEngine;

public class WeaponHarmonicaRevolver : WeaponRangedAbstract
{
	public override float WeaponDamage => 30f; // Устанавливаем постоянное значение урона для револьвера


	public override string WeaponNameSystem => "HarmonicaRevolver";
	public override string WeaponNameUI => "Револьвер Гармоника";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Pistol icon");


	protected override void InitializeWeapon()
	{
		WeaponAmmoType = AmmoTypes.Ammo9mm;
		// Здесь мы задаем параметры КОНКРЕТНО для этого револьвера
		MagazineAmmoMax = 5; // Барабан на 6 патронов
		MagazineAmmoCurrent = 5; // Начинаем с полным барабаном

		// Тип патронов тоже задается здесь (или можно через инспектор)
		

		
	}

	

}


