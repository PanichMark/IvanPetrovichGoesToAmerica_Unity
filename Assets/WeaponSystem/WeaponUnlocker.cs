using UnityEngine;

public class WeaponUnlocker : MonoBehaviour
{
	public WeaponController weaponController; // Ссылка на контроллер оружия

	void Update()
	{
		// Временная разблокировка оружия через клавиши
		if (Input.GetKeyDown(KeyCode.Alpha1)) // Клавиша '1'
		{
			weaponController.UnlockPoliceBatonWeapon();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) // Клавиша '2'
		{
			weaponController.UnlockHarmonicaRevolverWeapon();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3)) // Клавиша '3'
		{
			weaponController.UnlockPlungerCrossbowWeapon();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4)) // Клавиша '4'
		{
			weaponController.UnlockEugenicGenieWeapon();
		}
	}
}