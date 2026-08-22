using UnityEngine;
using System.Collections;

public abstract class WeaponPickableAbstract : InteractionObjectPickableNonThrowable, IPickableWeapon
{
	[Header("Weapon Attack Info")]
	[SerializeField] private string _weaponRightMouseButtonAttackMessage;
	public string WeaponRightMouseButtonAttackMessage => _weaponRightMouseButtonAttackMessage;

	[SerializeField] private string _weaponLeftMouseButtonAttackMessage;

	public string WeaponLeftMouseButtonAttackMessage => _weaponLeftMouseButtonAttackMessage;

	public abstract float WeaponDamage { get; }
	public abstract bool IsWeaponAuto { get; }
	public abstract float WeaponAttackSpeedRate { get; }

	protected AudioSource _weaponAudioSource;

	protected PlayerInteractionAnimationController _playerInteractionAnimationController;

	public virtual void AttackRight()
	{
		Debug.Log("PickableWeapon RIGHT attack");
	}

	public virtual void AttackLeft()
	{
		Debug.Log("PickableWeapon LEFT attack");
	}

	public abstract void InitializeWeapon();

	public abstract void StartAutoAttacking();
	public abstract void StopAutoAttacking();
	public abstract IEnumerator AutoAttackWeaponCourutine();
}
