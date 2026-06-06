using UnityEngine;
using System.Collections;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public abstract int ManaCost {  get; }
	protected GameObject _eugenicAttackDirection;
	protected GameObject _eugenicSourcePoint;
	protected PlayerResourcesManaManager _playerResourcesManaManager;

	private void Start()
	{
		if (_isThisPlayerWeapon == true)
		{
			_eugenicAttackDirection = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
			_eugenicSourcePoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");

			_playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		}

		InitializeWeaponEugenic();
	}

	public override void StartAutoAttacking()
	{
		if (_isWeaponAutoAttacking) return;
		_isWeaponAutoAttacking = true;
		if (_weaponAutoAttackCourutine == null)
		{
			_weaponAutoAttackCourutine = StartCoroutine(AutoAttackCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponAutoAttacking = false;
		if (_weaponAutoAttackCourutine != null)
		{
			StopCoroutine(_weaponAutoAttackCourutine);
			_weaponAutoAttackCourutine = null;
		}
	}

	protected virtual void PerformSingleEugenicAttack()
	{

	}

	public override void WeaponAttack()
	{
		// Проверяем, достаточно ли маны для атаки
		if (_playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
		{
			// --- НОВАЯ ЛОГИКА ---
			// Если это автоматическое евгеническое оружие
			if (IsWeaponAuto)
			{
				StartAutoAttacking(); // Запускаем бесконечную очередь атак
			}
			else // Если это одиночное евгеническое заклинание
			{
				// Просто выполняем атаку один раз
				PerformSingleEugenicAttack();
			}
		}
	}

	public override IEnumerator AutoAttackCourutine()
	{
		while (true)
		{
			if (!_isWeaponAutoAttacking)
			{
				break;
			}

			WeaponAttack();

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				_isWeaponAutoAttacking = false;
				break;
			}
		}
		_weaponAutoAttackCourutine = null;
	}

	protected abstract void InitializeWeaponEugenic();
}