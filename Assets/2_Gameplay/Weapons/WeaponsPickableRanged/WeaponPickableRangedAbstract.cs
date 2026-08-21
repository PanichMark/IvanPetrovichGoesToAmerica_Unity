using UnityEngine;

public abstract class WeaponPickableRangedAbstract : InteractionObjectPickableNonThrowable, IWeaponRanged
{
	[Header("Object Damage")]
	[SerializeField] private float _damage;
	[SerializeField] private float _attackSpeedRate;

	public float WeaponRange => throw new System.NotImplementedException();

	public AmmoTypes PlayerWeaponAmmoType => throw new System.NotImplementedException();

	public bool LeavesBulletHole => throw new System.NotImplementedException();

	public int PlayerMagazineAmmoCurrent => throw new System.NotImplementedException();

	public int PlayerMagazineAmmoMax => throw new System.NotImplementedException();

	public GameObject WeaponRangedShootPoint => throw new System.NotImplementedException();
}
