using System.Collections;
using UnityEngine;

public class WeaponEugenicGenieBreath : WeaponEugenicAbstract
{
	public override WeaponNames WeaponName => WeaponNames.GenieBreath;
	public override WeaponTypes WeaponType => WeaponTypes.Eugenic;
	public override float WeaponDamage => 100;
	public override int ManaCost => 0;
	public override bool IsWeaponAuto => false;
	private Vector3 _flightDirection;
	private Vector3 _targetPosition;
	public override float WeaponAttackSpeedRate => 1.4f;
	private GameObject _playerGameObject;
	private GameObject _playerCameraGameObject;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private float _eugenicAttackRange = 3f;
	private float _eugenicGenieBreathKnockbackForce = 10f;
	private GameObject _VFXgenieBreath;
	private Transform _VFXspawnPoint;
	private GameObject _vfxInstance;

	protected override void InitializeWeaponEugenic()
	{
		_VFXgenieBreath = Resources.Load<GameObject>("VFXs/VFX_EugenicGenieBreath");
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
		_playerGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");

		_playerCameraGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
		//_playerWeaponController.OnAllWeaponsHidden += TurnEugenicVFXOff;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = _playerCameraGameObject.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = _playerGameObject.transform;
		}

		_playerCameraStateMachineController.OnCameraStateChanged += ChangeVFXSpawnPoint;
	}

	protected override IEnumerator SingleEugenicAttack()
	{
		_currentWeaponPlayerEugenicAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this));

		_playerResourcesManaManager.UseMana(ManaCost);

		if (_vfxInstance == null)
		{
			yield return new WaitForSeconds(0.52f);

			// Сохраняем данные для анимации ДО запуска корутины
			_flightDirection = _playerCameraGameObject.transform.forward.normalized;

			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				_vfxInstance = Instantiate(
				_VFXgenieBreath,
				_VFXspawnPoint.position,
				_VFXspawnPoint.rotation * Quaternion.Euler(0, 0, 0),
				_VFXspawnPoint.transform);
			}
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
			{
				_vfxInstance = Instantiate(
					_VFXgenieBreath,
					_VFXspawnPoint.position + Vector3.up * 1.2f,
					_playerCameraGameObject.transform.rotation * Quaternion.Euler(0, 0, 0),
					_VFXspawnPoint.transform);
			}

			_vfxInstance.transform.parent = null;

			Vector3 startPosition = _vfxInstance.transform.position;
			_targetPosition = startPosition + _flightDirection * _eugenicAttackRange;



			StartCoroutine(AnimateAndDestroyVFX());
		}

		Vector3 attackOrigin = _eugenicAttackDirection.transform.position + _eugenicAttackDirection.transform.forward * 1.5f;
		Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, _eugenicAttackRange);

		foreach (Collider hit in hitColliders)
		{
			IDamageable damageable = hit.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(WeaponDamage);
			}

			IBreakable breakable = hit.GetComponent<IBreakable>();
			if (breakable != null)
			{
				breakable.TakeDamage(WeaponDamage);
			}

			Rigidbody rb = hit.GetComponent<Rigidbody>();
			if (rb != null && !rb.isKinematic)
			{
				Vector3 knockbackDirection = _eugenicSourcePoint.transform.forward.normalized;
				rb.AddForce(knockbackDirection * _eugenicGenieBreathKnockbackForce, ForceMode.Impulse);
			}
		}

		yield return _currentWeaponPlayerEugenicAttackRoutine;

		_isAttacking = false;
	}

	private IEnumerator AnimateAndDestroyVFX()
	{
		float duration = 0.4f;
		float elapsedTime = 0f;

		Vector3 startScale = _vfxInstance.transform.localScale;
		// Возвращаем логику из вашего исходного кода для масштаба
		Vector3 targetScale = startScale * _eugenicAttackRange * 5;

		// Сохраняем стартовую позицию один раз здесь
		Vector3 startPosition = _vfxInstance.transform.position;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;

			// Линейный прогресс от 0 до 1
			float linearT = Mathf.Clamp01(elapsedTime / duration);

			// Применяем функцию SmoothStep для нелинейности (ease-in-out)
			// Это уберет "эффект резины" и сделает полет кинематографичным
			float smoothT = Mathf.SmoothStep(0f, 1f, linearT);

			// Позиция теперь вычисляется строго между двумя зафиксированными точками,
			// а коэффициент времени делает движение плавным.
			_vfxInstance.transform.position = Vector3.Lerp(
				startPosition,
				_targetPosition,
				smoothT
			);

			_vfxInstance.transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

			yield return null;
		}

		// Фиксация финальных значений на случай микро-ошибок точности
		_vfxInstance.transform.position = _targetPosition;
		_vfxInstance.transform.localScale = targetScale;

		Destroy(_vfxInstance);
		_vfxInstance = null;
	}

	private void ChangeVFXSpawnPoint()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = _playerCameraGameObject.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = _playerGameObject.transform;
		}
	}

	private void OnDestroy()
	{
		//Debug.Log("2222222222222222");
		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnCameraStateChanged -= ChangeVFXSpawnPoint;
		}

		if (_playerWeaponController != null)
		{
			//_playerWeaponController.OnAllWeaponsHidden -= TurnEugenicVFXOff;
		}
	}

	public override void TurnEugenicVFXOff()
	{
		if (_vfxInstance != null)
		{
			Destroy(_vfxInstance);
			_vfxInstance = null;
		}
	}
}