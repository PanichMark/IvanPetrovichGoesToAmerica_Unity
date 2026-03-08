using UnityEngine;

public class WeaponEugenicGenie : WeaponAbstract
{
 
	public override string WeaponNameSystem => "EugenicGenie";
	public override string WeaponNameUI => "Дыхание Джинна";

	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Jinny icon");


	public override void WeaponAttack()
	{
		Debug.Log("EugenicAttack");
	}
}


