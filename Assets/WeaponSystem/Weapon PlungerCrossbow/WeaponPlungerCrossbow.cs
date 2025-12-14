using UnityEngine;

public class WeaponPlungerCrossbow : WeaponClass
{
    public WeaponPlungerCrossbow()
    {
        WeaponNameSystem = "PlungerCrossbow";
		WeaponNameUI = "Абордажный Арбалет";
	}

	public void Awake()
	{
		weaponModel = Resources.Load<GameObject>("WeaponPlungerCrossbow"); // Загружаем префаб револьвера
	//	weaponMeshRenderer = weapon
		//Debug.Log("Загружен префаб: " + weaponModel);
	}

	public override void WeaponAttack()
	{
		Debug.Log("CrossbowAttack");
	}
}

