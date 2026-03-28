using UnityEngine;

public class WeaponSawedoffShotgun : RangedWeaponAbstract
{
	public override void WeaponAttack()
	{
		Debug.Log("Shotgun Attack");
	}
	public override string WeaponNameSystem => "SawedOffShotgun";
	public override string WeaponNameUI => "Дробовик Обрез";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Shotgun Icon");

}

