using UnityEngine;

public class WeaponHarmonicaRevolver : WeaponAbstract
{
	public override float WeaponDamage => 30f; // Устанавливаем постоянное значение урона для револьвера

	WeaponHarmonicaRevolver()
    {
        WeaponNameSystem = "HarmonicaRevolver";
		WeaponNameUI = "Револьвер Гармоника";
	}

	public void Awake()
	{
		weaponModel = Resources.Load<GameObject>("WeaponHarmonicaRevolver"); // Загружаем префаб револьвера
		//Debug.Log("Загружен префаб: " + weaponModel);
	}

	public override void WeaponAttack()
	{
		Debug.Log("Revolver Attack");
		//PlayerAmmoManager.Instance.Shoot(WeaponDamage);
	}

}


