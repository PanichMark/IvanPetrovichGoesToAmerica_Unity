using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponRangedBergmannBayard : WeaponRangedAbstract
{
	public override WeaponNames WeaponName => WeaponNames.BergmannBayard;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	protected override float _waitForAmmoRefill => 1;
	public override float WeaponAttackSpeedRate => 0.1f;
	public override float WeaponDamage => 20f;
	public override bool IsWeaponAuto => true;

	private SkinnedMeshRenderer _bergmann1stPersonGunMesh;
	private SkinnedMeshRenderer _bergmann3rdPersonGunMesh;

	private Transform _ejectedBullet1stPersonSpawnPoint;
	private Transform _ejectedBullet3rdPersonSpawnPoint;

	private GameObject _ejectedBullet;

	protected override void InitializeWeaponRanged()
	{
		_bergmann1stPersonGunMesh = FirstPersonWeaponModelInstance.transform.Find("Weapon_Ranged_BergmannBayard_Gun").GetComponent<SkinnedMeshRenderer>();
		_bergmann3rdPersonGunMesh = ThirdPersonWeaponModelInstance.transform.Find("Weapon_Ranged_BergmannBayard_Gun").GetComponent<SkinnedMeshRenderer>();

		_ejectedBullet1stPersonSpawnPoint = FirstPersonWeaponModelInstance.transform.Find("SpawnBullet").GetComponent<Transform>();
		_ejectedBullet3rdPersonSpawnPoint = ThirdPersonWeaponModelInstance.transform.Find("SpawnBullet").GetComponent<Transform>();

		_ejectedBullet = ThirdPersonWeaponModelInstance.transform.Find("Bullet").gameObject;

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
		_bergmann1stPersonGunMesh.SetBlendShapeWeight(0, 100);
		_bergmann3rdPersonGunMesh.SetBlendShapeWeight(0, 100);

		yield return new WaitForSeconds (0.04f);

		EjectBullet();

		yield return new WaitForSeconds(0.04f);

		_bergmann1stPersonGunMesh.SetBlendShapeWeight(0, 0);
		_bergmann3rdPersonGunMesh.SetBlendShapeWeight(0, 0);
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
}