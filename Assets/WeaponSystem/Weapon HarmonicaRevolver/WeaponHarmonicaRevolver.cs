using UnityEngine;

public class WeaponHarmonicaRevolver : WeaponClass
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

		PlayerAmmoManager.Instance.Shoot(WeaponDamage);
	}

}

