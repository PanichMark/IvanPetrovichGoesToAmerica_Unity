using UnityEngine;

public class WeaponHarmonicaRevolver : WeaponRangedAbstract
{
	public override string WeaponNameSystem => "HarmonicaRevolver";
	public override string WeaponNameUI => "Револьвер Гармоника";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Pistol icon");

	public override float WeaponDamage => 30f;

	protected override void InitializeWeaponRanged()
	{
		WeaponAmmoType = AmmoTypes.Ammo9mm;

		MagazineAmmoMax = 5; 
		MagazineAmmoCurrent = 5; 
	}
}