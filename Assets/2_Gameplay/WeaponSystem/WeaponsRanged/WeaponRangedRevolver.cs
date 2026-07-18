using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponRangedRevolver : WeaponRangedAbstract
{
	public override WeaponNames WeaponName => WeaponNames.Revolver;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	public override float WeaponDamage => 34f;
	public override bool IsWeaponAuto => false;
	public override float WeaponAttackSpeedRate => 0.167f;

	protected override float _waitForAmmoRefill => _waitForAmmoRefillRevolver;

	public override bool IsReloadingAnimationSingle => false;

	private float _waitForAmmoRefillRevolver;

	private GameObject _cartridge1stPerson;
	private GameObject _cartridge3rdPerson;
	private GameObject _ejectedCartridge;

	private GameObject[] _bullets1stPerson = new GameObject[5];
	private GameObject[] _bullets3rdPerson = new GameObject[5];

	private Vector3 _cartridgeOriginalPosition;

	private SkinnedMeshRenderer _revolver1stPersonGunMesh;
	private SkinnedMeshRenderer _revolver3rdPersonGunMesh;

	public int CartgridgeSlidingStep {  get; set; }

	protected override void InitializeWeaponRanged()
	{
		_revolver1stPersonGunMesh = FirstPersonWeaponModelInstance.transform.Find("Gun").GetComponent<SkinnedMeshRenderer>();
		_revolver3rdPersonGunMesh = ThirdPersonWeaponModelInstance.transform.Find("Gun").GetComponent<SkinnedMeshRenderer>();

		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");

		_cartridge1stPerson = FirstPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;
		_cartridge3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;

		_cartridgeOriginalPosition = _cartridge1stPerson.transform.localPosition;

		for (int i = 0; i < PlayerMagazineAmmoMax; i++)
		{
			_bullets3rdPerson[i] = _cartridge3rdPerson.transform.Find($"Bullet{i + 1}").gameObject;
			_bullets1stPerson[i] = _cartridge1stPerson.transform.Find($"Bullet{i + 1}").gameObject;
		}

		ReapplyCartridgePosition();
	}

	protected override void ApplyWeaponRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(2, 0.02f, 0.08f);
	}

	private void ReapplyCartridgePosition()
	{
		if (PlayerMagazineAmmoCurrent == 0)
		{
			_cartridge1stPerson.SetActive(false);
			_cartridge3rdPerson.SetActive(false);
			return;
		}

		CartgridgeSlidingStep = PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent;

		if (CartgridgeSlidingStep > 0)
		{
			_cartridge1stPerson.transform.localPosition += new Vector3(0.025f * (CartgridgeSlidingStep), 0, 0);
			_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f * (CartgridgeSlidingStep), 0, 0);

			for (int i = 0; i <= CartgridgeSlidingStep; i++)
			{
				_bullets3rdPerson[i].SetActive(false);
				_bullets1stPerson[i].SetActive(false);
			}
		}
	}

	private void HideUsedHarmonicaBullet()
	{
		_bullets3rdPerson[CartgridgeSlidingStep].SetActive(false);
		_bullets1stPerson[CartgridgeSlidingStep].SetActive(false);

		_cartridge1stPerson.transform.localPosition += new Vector3(0.025f, 0, 0);
		_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f, 0, 0);

		CartgridgeSlidingStep++;

		if (PlayerMagazineAmmoCurrent == 0)
		{
			EjectCartridge();
		}
	}

	private void EjectCartridge()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_ejectedCartridge = Instantiate(_cartridge1stPerson, FirstPersonWeaponModelInstance.transform);
		}
		else
		{
			_ejectedCartridge = Instantiate(_cartridge3rdPerson, ThirdPersonWeaponModelInstance.transform);
		}

		_ejectedCartridge.layer = LayerMask.NameToLayer("Default");

		foreach (Transform child in _ejectedCartridge.transform)
		{
			child.gameObject.layer = LayerMask.NameToLayer("Default");
		}

		_ejectedCartridge.transform.localPosition = _cartridgeOriginalPosition;
		_ejectedCartridge.transform.localPosition += new Vector3(0.025f * 5, 0, 0);
		_ejectedCartridge.transform.rotation = _cartridge1stPerson.transform.rotation;

		_ejectedCartridge.transform.SetParent(null);

		SceneManager.MoveGameObjectToScene(_ejectedCartridge, SceneManager.GetSceneByBuildIndex(1));

		_ejectedCartridge.AddComponent<BoxCollider>();
		_ejectedCartridge.AddComponent<Rigidbody>();

		_cartridge1stPerson.SetActive(false);
		_cartridge3rdPerson.SetActive(false);
	}

	private void RefillHarmonicaCartridge(int ammoToAdd)
	{
		int count = Mathf.Min(ammoToAdd, PlayerMagazineAmmoMax);

		_cartridge1stPerson.SetActive(true);
		_cartridge3rdPerson.SetActive(true);

		for (int i = 0; i < PlayerMagazineAmmoMax; i++)
		{
			_bullets3rdPerson[i].SetActive(false);
			_bullets1stPerson[i].SetActive(false);
		}

		if (count == 5)
		{
			for (int i = 0; i < PlayerMagazineAmmoMax; i++)
			{
				_bullets3rdPerson[i].SetActive(true);
				_bullets1stPerson[i].SetActive(true);
			}
		}
		else
		{
			for (int i = 0; i < PlayerMagazineAmmoCurrent; i++)
			{
				_bullets3rdPerson[i].SetActive(true);
				_bullets1stPerson[i].SetActive(true);
			}
		}

		_cartridge1stPerson.transform.localPosition -= new Vector3(0.025f * CartgridgeSlidingStep, 0, 0);
		_cartridge3rdPerson.transform.localPosition -= new Vector3(0.025f * CartgridgeSlidingStep, 0, 0);

		CartgridgeSlidingStep = 0;
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

		//Debug.Log(_vfxInstance.transform.position);

		Destroy(_vfxInstance, 0.05f);

		StartCoroutine(RevolverShootMechanism());

		if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hitInfo, 100f))
		{
			damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null && hitInfo.transform.gameObject.layer != 9)
			{
				Debug.Log($"Попадание в объект: {hitInfo.transform.name}, Слой: {hitInfo.transform.gameObject.layer}");
				damageable.TakeDamage(weaponDamage);
			}
		}

		// Проверяем, есть ли вообще коллайдер и трансформ у объекта
		if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
		{
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

			// Добавляем четвертый параметр - Transform родителя
			// Мы всегда хотим, чтобы след был дочерним объектом
			_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
		}

		PlayerMagazineAmmoCurrent--;
		HideUsedHarmonicaBullet();

		Debug.Log($"Shoot {WeaponName}");

		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		
		ApplyWeaponRecoil();

		yield return _currentWeaponPlayerShootRoutine;

		_currentWeaponPlayerShootRoutine = null;
	}

	public override IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_weaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));

		if (PlayerMagazineAmmoCurrent == 0)
		{
			_waitForAmmoRefillRevolver = 1.2f;
		}
		else
		{
			_waitForAmmoRefillRevolver = 3.8f;

			StartCoroutine(EjectCartridgeDuringLongReload());
		}

		yield return new WaitForSeconds(_waitForAmmoRefill);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;
		PlayerMagazineAmmoCurrent += ammoToAdd;
		RefillHarmonicaCartridge(ammoToAdd);

		_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);
		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
	}

	private IEnumerator EjectCartridgeDuringLongReload()
	{
		yield return new WaitForSeconds(1.25f);

		EjectCartridge();

		yield return null;
	}

	private IEnumerator RevolverShootMechanism()
	{
		float totalDuration = 0.12f; // Общий цикл анимации: 0.01 сек вверх + 0.01 сек вниз
		int shapeIndex = 0;

		float timer = 0f;
		while (timer < totalDuration)
		{
			// Вычисляем нормализованное время t от 0 до 1 для всего цикла
			float cycleT = Mathf.Clamp01(timer / totalDuration);

			// Если мы в первой половине цикла (от 0 до 0.5) — открываем барабан
			if (cycleT <= 0.5f)
			{
				// Растягиваем первую половину с [0..0.5] до [0..1]
				float openT = Mathf.Clamp01(cycleT / 0.5f);
				float weight = Mathf.Lerp(0f, 100f, openT);

				_revolver1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, weight);
				_revolver3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, weight);
			}
			else // Если во второй половине (от 0.5 до 1) — закрываем
			{
				// Сжимаем вторую половину с [0.5..1] до [0..1]
				float closeT = Mathf.Clamp01((cycleT - 0.5f) / 0.5f);
				float weight = Mathf.Lerp(100f, 0f, closeT);

				_revolver1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, weight);
				_revolver3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, weight);
			}

			timer += Time.deltaTime;
			yield return null;
		}

		// Гарантированное обнуление веса после завершения корутины,
		// чтобы избежать расхождений из-за точности чисел с плавающей запятой.
		_revolver1stPersonGunMesh.SetBlendShapeWeight(shapeIndex, 0f);
		_revolver3rdPersonGunMesh.SetBlendShapeWeight(shapeIndex, 0f);
	}
}