using UnityEngine;

public class WeaponSawedOffShotgun : WeaponRangedAbstract
{
	public override string WeaponNameSystem => "SawedOffShotgun";
	public override string WeaponNameUI => "Дробовик Обрез";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponSawedOffShotgunIcon");
	public override float WeaponDamage => 100f;

	protected override void InitializeWeaponRanged()
	{
		WeaponAmmoType = AmmoTypes.Ammo12gauge;

		MagazineAmmoMax = 2; 
		PlayerMagazineAmmoCurrent = 2; 
	}
}