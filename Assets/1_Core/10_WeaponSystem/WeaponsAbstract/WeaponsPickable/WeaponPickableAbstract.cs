using UnityEngine;

public abstract class WeaponPickableAbstract : InteractionObjectPickableNonThrowable, IPickableWeapon
{
	[Header("Weapon Attack Info")]
	[SerializeField] private string _weaponRightMouseButtonAttackMessage;
	public string WeaponRightMouseButtonAttackMessage => _weaponRightMouseButtonAttackMessage;

	[SerializeField] private string _weaponLeftMouseButtonAttackMessage;

	public string WeaponLeftMouseButtonAttackMessage => _weaponLeftMouseButtonAttackMessage;

	[Header("Weapon Attack Data")]
	[SerializeField] private bool _isWeaponAuto;
	[SerializeField] private float _damage;
	[SerializeField] private float _attackSpeedRate;

	public void AttackRight()
	{
		Debug.Log("PickableWeapon RIGHT attack");
	}

	public void AttackLeft()
	{
		Debug.Log("PickableWeapon LEFT attack");
	}
}
