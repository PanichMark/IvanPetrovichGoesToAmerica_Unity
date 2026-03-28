using UnityEngine;

public class WeaponSawedoffShotgun : RangedWeaponAbstract
{
	public override void WeaponAttack()
	{
		Debug.Log("Shotgun Attack");
	}

	protected override void InitializeWeapon()
	{
		
	}

	public override string WeaponNameSystem => "SawedOffShotgun";
	public override string WeaponNameUI => "Дробовик Обрез";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Shotgun Icon");

}

