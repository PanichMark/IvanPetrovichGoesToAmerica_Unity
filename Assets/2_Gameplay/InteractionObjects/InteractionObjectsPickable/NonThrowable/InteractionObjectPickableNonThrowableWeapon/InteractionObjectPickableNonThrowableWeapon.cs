using UnityEngine;

public class InteractionObjectPickableNonThrowableWeapon : InteractionObjectPickableNonThrowable
{
	[Header("Object Damage")]
	[SerializeField] private float _damage;
	[SerializeField] private float _attackSpeedRate;
}
