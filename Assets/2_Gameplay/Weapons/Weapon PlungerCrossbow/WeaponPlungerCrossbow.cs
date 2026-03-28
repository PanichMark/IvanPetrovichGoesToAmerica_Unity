using UnityEngine;

public class WeaponPlungerCrossbow : WeaponAbstract
{
    
	public override string WeaponNameSystem => "PlungerCrossbow";
	public override string WeaponNameUI => "Абордажный Арбалет";
	
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/CrossBow icon");



	public override void WeaponAttack()
	{
		Debug.Log("CrossbowAttack");
	}

	
}


