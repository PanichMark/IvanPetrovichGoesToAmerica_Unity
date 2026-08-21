using UnityEngine;

public interface IWeaponRanged
{
	float WeaponRange { get; }
	AmmoTypes PlayerWeaponAmmoType { get; }
	bool LeavesBulletHole { get; }
	int PlayerMagazineAmmoCurrent { get; }
	int PlayerMagazineAmmoMax { get;  }

	GameObject WeaponRangedShootPoint { get; }
}