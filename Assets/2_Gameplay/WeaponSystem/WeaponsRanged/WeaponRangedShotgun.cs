using System.Collections;
using UnityEngine;

public class WeaponRangedShotgun : WeaponRangedAbstract
{
	public override WeaponNames WeaponName => WeaponNames.Shotgun;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo12gauge;
	public override float WeaponDamage => 100f;
	protected override float _waitForAmmoRefill => 3.125f;
	public override bool IsWeaponAuto => false;
	public override bool IsReloadingAnimationSingle => true;
	public override float WeaponAttackSpeedRate => 0.15f;

	private GameObject _shotgunBarrel1stPerson;
	private GameObject _shotgunBarrel3rdPerson;

	protected override void InitializeWeaponRanged()
	{
		_shotgunBarrel1stPerson = FirstPersonWeaponModelInstance.transform.Find("Barrel").gameObject;
		_shotgunBarrel3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Barrel").gameObject;

		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");
	}

	protected override IEnumerator ShootWeaponPlayer(float weaponDamage)
	{
		_currentWeaponPlayerShootRoutine = StartCoroutine(_weaponAnimationController.WeaponShootAnimation(this));

		int pelletCount = 10;
		float spreadAngle = 7f;
		float range = 100f;

		_vfxInstance = Instantiate(
			_VFXshottEffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation,
			_VFXspawnPoint.transform);
		_vfxInstance.transform.localScale = Vector3.one * 2.5f;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		Destroy(_vfxInstance, 0.05f);

		for (int i = 0; i < pelletCount; i++)
		{
			RaycastHit hitInfo;

			Quaternion randomRotation = Random.rotationUniform;
			Quaternion spreadRotation = Quaternion.Slerp(Quaternion.identity, randomRotation, spreadAngle / 90f);
			Vector3 finalDirection = spreadRotation * _shootPoint.transform.forward;

			Color rayColor = Physics.Raycast(_shootPoint.transform.position, finalDirection, out hitInfo, range) ? Color.red : Color.yellow;
			Debug.DrawRay(_shootPoint.transform.position, finalDirection * range, rayColor, 2f);

			if (hitInfo.collider != null)
			{
				IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
				if (damageable != null && hitInfo.transform.gameObject.layer != 9)
				{
					damageable.TakeDamage(weaponDamage / pelletCount);
				}

				IBreakable breakable = hitInfo.transform.GetComponent<IBreakable>();
				if (breakable != null)
				{
					breakable.TakeDamage(weaponDamage / pelletCount);
				}

				if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
				{
					Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
					_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
				}
			}
		}

		PlayerMagazineAmmoCurrent--;
		Debug.Log($"Shoot {WeaponName}");

		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		ApplyWeaponRecoil();

		yield return _currentWeaponPlayerShootRoutine;

		_currentWeaponPlayerShootRoutine = null;
	}

	protected override void ApplyWeaponRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(15, 0.05f, 0.5f);
	}

	public override IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_weaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));

		StartCoroutine(ShotgunReloadBreakActionOpen());

		yield return new WaitForSeconds(_waitForAmmoRefill);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;
		PlayerMagazineAmmoCurrent += ammoToAdd;

		if (System.Enum.TryParse(WeaponName.ToString(), out WeaponNames parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		StartCoroutine(ShotgunReloadBreakActionClose());

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
	}

	private IEnumerator ShotgunReloadBreakActionOpen()
	{
		yield return new WaitForSeconds(1.250f);

		float duration = 0.167f;
		float targetAngle = 75f; // Угол поворота по локальной оси X

		Transform barrel1stTrans = _shotgunBarrel1stPerson.transform;
		Transform barrel3rdTrans = _shotgunBarrel3rdPerson.transform;

		// Сохраняем начальные повороты
		Quaternion startRot1st = barrel1stTrans.localRotation;
		Quaternion startRot3rd = barrel3rdTrans.localRotation;

		// Вычисляем целевой поворот
		Quaternion endRot1st = startRot1st * Quaternion.Euler(targetAngle, 0f, 0f);
		Quaternion endRot3rd = startRot3rd * Quaternion.Euler(targetAngle, 0f, 0f);

		float timer = 0f;

		while (timer < duration)
		{
			float t = Mathf.Clamp01(timer / duration);

			// Обращаемся к .transform.localRotation
			barrel1stTrans.localRotation = Quaternion.Lerp(startRot1st, endRot1st, t);
			barrel3rdTrans.localRotation = Quaternion.Lerp(startRot3rd, endRot3rd, t);

			timer += Time.deltaTime;
			yield return null;
		}

		barrel1stTrans.localRotation = endRot1st;
		barrel3rdTrans.localRotation = endRot3rd;
	}

	private IEnumerator ShotgunReloadBreakActionClose()
	{
		yield return new WaitForSeconds(0.417f);

		float duration = 0.208f;
		float targetAngle = -75f; // Изменено на отрицательное значение

		Transform barrel1stTrans = _shotgunBarrel1stPerson.transform;
		Transform barrel3rdTrans = _shotgunBarrel3rdPerson.transform;

		Quaternion startRot1st = barrel1stTrans.localRotation;
		Quaternion startRot3rd = barrel3rdTrans.localRotation;

		// Целевой поворот будет "текущий угол МИНУС 75"
		Quaternion endRot1st = startRot1st * Quaternion.Euler(targetAngle, 0f, 0f);
		Quaternion endRot3rd = startRot3rd * Quaternion.Euler(targetAngle, 0f, 0f);

		float timer = 0f;

		while (timer < duration)
		{
			float t = Mathf.Clamp01(timer / duration);

			barrel1stTrans.localRotation = Quaternion.Lerp(startRot1st, endRot1st, t);
			barrel3rdTrans.localRotation = Quaternion.Lerp(startRot3rd, endRot3rd, t);

			timer += Time.deltaTime;
			yield return null;
		}

		barrel1stTrans.localRotation = endRot1st;
		barrel3rdTrans.localRotation = endRot3rd;
	}
}