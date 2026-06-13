using UnityEngine;
using System.Collections;

public class WeaponEugenicGenieBreath : WeaponEugenicAbstract
{
	public override string WeaponName => "GenieBreath";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 100;
	public override int ManaCost => 20;
	public override bool IsWeaponAuto => false;

	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private float _eugenicAttackRange = 5f;
	private float _eugenicGenieBreathKnockbackForce = 10f;
	private GameObject _VFXgenieBreath;
	private Transform _VFXspawnPoint;
	private GameObject _vfxInstance;
	private PlayerWeaponController _playerWeaponController;

	protected override void InitializeWeaponEugenic()
	{
		_VFXgenieBreath = Resources.Load<GameObject>($"VFXs/VFX_EugenicGenieBreath");
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
		_playerWeaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		_playerWeaponController.OnWeaponHidden += TurnEugenicVFXOff;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform;
		}

		_playerCameraStateMachineController.OnCameraStateChanged += ChangeVFXSpawnPoint;
	}

	protected override void SingleEugenicAttack()
	{
		if (_vfxInstance == null)
		{
			_vfxInstance = Instantiate(
				_VFXgenieBreath,
				_VFXspawnPoint.position,
				_VFXspawnPoint.rotation * Quaternion.Euler(90, 0, 0),
				_VFXspawnPoint.transform);

			// Запускаем корутину, которая и анимирует, и удалит объект
			StartCoroutine(AnimateAndDestroyVFX());
		}

		_playerResourcesManaManager.UseMana(ManaCost);

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
	}

	private IEnumerator AnimateAndDestroyVFX()
	{
		float duration = 0.4f;
		float elapsedTime = 0f;
		Vector3 startScale = _vfxInstance.transform.localScale;
		Vector3 targetScale = _vfxInstance.transform.localScale * 4f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			// Плавно интерполируем (сглаживаем) изменение масштаба
			_vfxInstance.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
			yield return null; // Ждем следующего кадра
		}

		// Убеждаемся, что в конце масштаб был точно равен целевому (на случай погрешностей)
		_vfxInstance.transform.localScale = targetScale;

		// Уничтожаем объект после завершения анимации
		Destroy(_vfxInstance);
		_vfxInstance = null; // Обнуляем ссылку

		yield break; // Выходим из корутины
	}

	private void ChangeVFXSpawnPoint()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform;
		}

		if (_vfxInstance != null)
		{
			TurnEugenicVFXOff();
			SingleEugenicAttack();
		}
	}

	private void OnDestroy()
	{
		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnCameraStateChanged -= ChangeVFXSpawnPoint;
		}

		if (_playerWeaponController != null)
		{
			_playerWeaponController.OnWeaponHidden -= TurnEugenicVFXOff;
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