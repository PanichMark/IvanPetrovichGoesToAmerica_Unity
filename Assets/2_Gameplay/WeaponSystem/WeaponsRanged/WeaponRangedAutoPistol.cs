using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponRangedAutoPistol : WeaponRangedAbstract
{
	public override WeaponNames WeaponName => WeaponNames.AutoPistol;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	protected override float _waitForAmmoRefill => 3.25f;
	public override float WeaponAttackSpeedRate => 0.133f;
	public override float WeaponDamage => 20f;
	public override bool IsWeaponAuto => true;
	public override bool IsReloadingAnimationSingle => true;

	private SkinnedMeshRenderer _bergmann1stPersonGunMesh;
	private SkinnedMeshRenderer _bergmann3rdPersonGunMesh;

	private Transform _ejectedBullet1stPersonSpawnPoint;
	private Transform _ejectedBullet3rdPersonSpawnPoint;

	private GameObject _ejectedBullet;

	private GameObject _magazine1stPersonOld;
	private GameObject _magazine3rdPersonOld;

	private GameObject _magazine1stPersonNew;
	private GameObject _magazine3rdPersonNew;

	protected override void InitializeWeaponRanged()
	{
		_bergmann1stPersonGunMesh = FirstPersonWeaponModelInstance.transform.Find("Weapon_Ranged_BergmannBayard_Gun").GetComponent<SkinnedMeshRenderer>();
		_bergmann3rdPersonGunMesh = ThirdPersonWeaponModelInstance.transform.Find("Weapon_Ranged_BergmannBayard_Gun").GetComponent<SkinnedMeshRenderer>();

		_ejectedBullet1stPersonSpawnPoint = FirstPersonWeaponModelInstance.transform.Find("SpawnBullet").GetComponent<Transform>();
		_ejectedBullet3rdPersonSpawnPoint = ThirdPersonWeaponModelInstance.transform.Find("SpawnBullet").GetComponent<Transform>();

		_ejectedBullet = ThirdPersonWeaponModelInstance.transform.Find("Bullet").gameObject;

		_magazine1stPersonOld = FirstPersonWeaponModelInstance.transform.Find("Magazine").gameObject;
		_magazine3rdPersonOld = ThirdPersonWeaponModelInstance.transform.Find("Magazine").gameObject;

		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");
	}

	protected override void ApplyWeaponRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilAuto();
	}

	protected override IEnumerator ShootWeaponPlayer(float weaponDamage)
	{
		_currentWeaponPlayerShootRoutine = StartCoroutine(_weaponAnimationController.WeaponShootAnimation(this));

		RaycastHit hitInfo;
		IDamageable damageable = null;

		_vfxInstance = Instantiate(
			_VFXshottEffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation,
			_VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		Destroy(_vfxInstance, 0.05f);

		StartCoroutine(BergmannBayardShootMechanism());

		if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hitInfo, 100f))
		{
			damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null && hitInfo.transform.gameObject.layer != 9)
			{
				Debug.Log($"Попадание в объект: {hitInfo.transform.name}, Слой: {hitInfo.transform.gameObject.layer}");
				damageable.TakeDamage(weaponDamage);
			}
		}

		if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
		{
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

			_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
		}

		PlayerMagazineAmmoCurrent--;

		Debug.Log($"Shoot {WeaponName}");

		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		ApplyWeaponRecoil();

		yield return _currentWeaponPlayerShootRoutine;

		_currentWeaponPlayerShootRoutine = null;
	}

	private IEnumerator BergmannBayardShootMechanism()
	{
		float totalDuration = 0.08f;
		int shapeIndex = 0;

		float timer = 0f;
		while (timer < totalDuration)
		{
			if (timer < 0.02f)
			{
				// Фаза открытия (0 - 0.02 сек)
				float t = Mathf.Clamp01(timer / 0.02f);
				_bergmann1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, Mathf.Lerp(0f, 100f, t));
				_bergmann3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, Mathf.Lerp(0f, 100f, t));
			}
			else if (timer >= 0.02f && timer < 0.06f)
			{
				// Пауза с открытым затвором (0.02 - 0.06 сек)
				_bergmann1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, 100f);
				_bergmann3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, 100f);

				if (timer >= 0.04f && timer < 0.04f + Time.deltaTime)
				{
					EjectBullet();
				}
			}
			else if (timer >= 0.06f)
			{
				// Фаза закрытия (0.06 - 0.08 сек)
				float t = Mathf.Clamp01((timer - 0.06f) / 0.02f);
				_bergmann1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, Mathf.Lerp(100f, 0f, t));
				_bergmann3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, Mathf.Lerp(100f, 0f, t));
			}

			timer += Time.deltaTime;
			yield return null;
		}

		_bergmann1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, 0f);
		_bergmann3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, 0f);
	}

	private void EjectBullet()
	{
		GameObject ejectedBullet = null;
		Transform spawnPoint;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			spawnPoint = _ejectedBullet1stPersonSpawnPoint;
		}
		else
		{
			spawnPoint = _ejectedBullet3rdPersonSpawnPoint;
		}

		ejectedBullet = Instantiate(_ejectedBullet, spawnPoint.position, spawnPoint.rotation);

		ejectedBullet.transform.SetParent(null);
		ejectedBullet.layer = LayerMask.NameToLayer("Default");
		SceneManager.MoveGameObjectToScene(ejectedBullet, SceneManager.GetSceneByBuildIndex(1));

		Renderer renderer = ejectedBullet.GetComponent<Renderer>();
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

		Rigidbody rb = ejectedBullet.AddComponent<Rigidbody>();
		rb.useGravity = true;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			rb.AddForce(_ejectedBullet1stPersonSpawnPoint.up * 3f, ForceMode.Impulse);
		}
		else
		{
			rb.AddForce(_ejectedBullet3rdPersonSpawnPoint.up * 3f, ForceMode.Impulse);
		}

		StartCoroutine(AddColliderDelayed(ejectedBullet));
	}

	private IEnumerator AddColliderDelayed(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		obj.AddComponent<BoxCollider>();
	}

	public override IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_weaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));

		StartCoroutine(ReloadMagazineVisibility());

		yield return new WaitForSeconds(_waitForAmmoRefill);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;
		PlayerMagazineAmmoCurrent += ammoToAdd;

		if (System.Enum.TryParse(WeaponName.ToString(), out WeaponNames parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
	}

	private IEnumerator ReloadMagazineVisibility()
	{
		if (WeaponHandType == WeaponHandsEnum.Right)
		{
			_magazine1stPersonNew = Instantiate(_magazine3rdPersonOld, _firstPersonLeftHandWeaponSlotTransform);
			_magazine3rdPersonNew = Instantiate(_magazine3rdPersonOld, _thirdPersonLeftHandWeaponSlotTransform);
		}
		else
		{
			_magazine1stPersonNew = Instantiate(_magazine3rdPersonOld, _firstPersonRightHandWeaponSlotTransform);
			_magazine3rdPersonNew = Instantiate(_magazine3rdPersonOld, _thirdPersonRightHandWeaponSlotTransform);

			_magazine1stPersonNew.transform.localScale = new Vector3(-1, 1, 1);
			_magazine3rdPersonNew.transform.localScale = new Vector3(-1, 1, 1);
		}

		_magazine1stPersonNew.transform.localPosition = Vector3.zero;
		_magazine1stPersonNew.transform.localRotation = Quaternion.identity;
		_magazine3rdPersonNew.transform.localPosition = Vector3.zero;
		_magazine3rdPersonNew.transform.localRotation = Quaternion.identity;

		MeshRenderer meshRenderer1st = _magazine1stPersonNew.GetComponent<MeshRenderer>();
		MeshRenderer meshRenderer3rd = _magazine3rdPersonNew.GetComponent<MeshRenderer>();

		_magazine1stPersonNew.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		_magazine1stPersonNew.SetActive(false);
		meshRenderer1st.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		_magazine3rdPersonNew.SetActive(false);

		yield return new WaitForSeconds(1.09f);

		_magazine1stPersonOld.SetActive(false);
		_magazine3rdPersonOld.SetActive(false);

		_magazine1stPersonNew.SetActive(true);
		_magazine3rdPersonNew.SetActive(true);

		yield return new WaitForSeconds(0.7f);

		GameObject magazineTrownAway = Instantiate(_magazine3rdPersonNew, _magazine3rdPersonNew.transform.position, _magazine3rdPersonNew.transform.rotation);
		magazineTrownAway.transform.SetParent(null);
		magazineTrownAway.layer = LayerMask.NameToLayer("Default");
		SceneManager.MoveGameObjectToScene(magazineTrownAway, SceneManager.GetSceneByBuildIndex(1));

		Renderer renderer = magazineTrownAway.GetComponent<Renderer>();
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

		Rigidbody rb = magazineTrownAway.AddComponent<Rigidbody>();
		rb.useGravity = true;

		magazineTrownAway.AddComponent<BoxCollider>();

		yield return new WaitForSeconds(1.46f);

		_magazine1stPersonOld.SetActive(true);
		_magazine3rdPersonOld.SetActive(true);

		Destroy(_magazine1stPersonNew);
		Destroy(_magazine3rdPersonNew);

		yield return null;
	}
}