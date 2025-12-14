using UnityEngine;

public class WeaponEugenicGenie : WeaponClass
{
    WeaponEugenicGenie()
    {
        WeaponNameSystem = "EugenicGenie";
		WeaponNameUI = "Евгеник Дыхание Джинна";
	}

	public void Awake()
	{
		weaponModel = Resources.Load<GameObject>("WeaponEugenicsGenie"); // Загружаем префаб револьвера
	}

	public override void WeaponAttack()
	{
		Debug.Log("EugenicAttack");
	}
}

