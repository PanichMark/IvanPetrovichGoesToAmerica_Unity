using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponRangedShotgun : WeaponRangedAbstract
{
	public override WeaponNames WeaponName => WeaponNames.Shotgun;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo12gauge;
	public override float WeaponDamage => 20f;
	protected override float _waitForAmmoRefill => 3.125f;
	public override bool IsWeaponAuto => false;
	public override bool IsReloadingAnimationSingle => true;
	public override float WeaponAttackSpeedRate => 0.15f;
	[SerializeField] protected AudioClip _weaponSoundBreakAction;
	[SerializeField] protected AudioClip _weaponSoundInsertShell;
	private GameObject _shotgunBarrel1stPerson;
	private GameObject _shotgunBarrel3rdPerson;
	protected override float _weaponRange => 15f;
	private GameObject _shellRight1stPerson;
	private GameObject _shellLeft1stPerson;
	private GameObject _shellRight3rdPerson;
	private GameObject _shellLeft3rdPerson;

	private int _pelletCount = 10;
	private float _spreadAngle = 7f;


	protected override void InitializeWeaponRanged()
	{
		_shotgunBarrel1stPerson = FirstPersonWeaponModelInstance.transform.Find("Barrel").gameObject;
		_shotgunBarrel3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Barrel").gameObject;

		_shellRight1stPerson = _shotgunBarrel1stPerson.transform.Find("ShellRight").gameObject;
		_shellLeft1stPerson = _shotgunBarrel1stPerson.transform.Find("ShellLeft").gameObject;
		_shellRight3rdPerson = _shotgunBarrel3rdPerson.transform.Find("ShellRight").gameObject;
		_shellLeft3rdPerson = _shotgunBarrel3rdPerson.transform.Find("ShellLeft").gameObject;

		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");
	}

	protected override IEnumerator OnSpecificShootMechanics()
	{
		for (int i = 0; i < _pelletCount; i++)
		{
			Quaternion randomRotation = Random.rotationUniform;
			Quaternion spreadRotation = Quaternion.Slerp(Quaternion.identity, randomRotation, _spreadAngle / 90f);
			Vector3 finalDirection = spreadRotation * _shootPoint.transform.forward;

			RaycastHit[] hits = Physics.RaycastAll(_shootPoint.transform.position, finalDirection, _weaponRange);
			System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

			if (hits.Length > 0)
			{
				SpawnBulletHoleDecal(hits);
				ProcessDamage(hits, WeaponDamage, 10);
			}
		}

		yield return null;
	}

	protected override void ApplyWeaponRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(15, 0.05f, 0.5f);
	}

	public override IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_playerWeaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));

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

		_weaponAudioSource.PlayOneShot(_weaponSoundInsertShell);
		if (ammoToAdd == 2)
		{
			yield return new WaitForSeconds(0.1f);

			_weaponAudioSource.PlayOneShot(_weaponSoundInsertShell);
		}

		_shellRight1stPerson.SetActive(true);
		_shellLeft1stPerson.SetActive(true);

		_shellRight3rdPerson.SetActive(true);
		_shellLeft3rdPerson.SetActive(true);

		StartCoroutine(ShotgunReloadBreakActionClose());

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
	}

	private IEnumerator ShotgunReloadBreakActionOpen()
	{
		yield return new WaitForSeconds(1.250f);

		_weaponAudioSource.PlayOneShot(_weaponSoundBreakAction);

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

			if (timer > duration/2)
			{
				EjectShell();
			}

			timer += Time.deltaTime;
			yield return null;
		}

		barrel1stTrans.localRotation = endRot1st;
		barrel3rdTrans.localRotation = endRot3rd;
	}

	private IEnumerator ShotgunReloadBreakActionClose()
	{
		yield return new WaitForSeconds(0.417f);

		_weaponAudioSource.PlayOneShot(_weaponSoundBreakAction);

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

	private void EjectShell()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			GameObject ejectedShellRight = Instantiate(_shellRight1stPerson, _shellRight1stPerson.transform.position, _shellRight1stPerson.transform.rotation);

			ejectedShellRight.transform.SetParent(null);
			ejectedShellRight.layer = LayerMask.NameToLayer("Default");
			SceneManager.MoveGameObjectToScene(ejectedShellRight, SceneManager.GetSceneByBuildIndex(1));

			Renderer rendererRight = ejectedShellRight.GetComponent<Renderer>();
			rendererRight.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

			Rigidbody rbRight = ejectedShellRight.AddComponent<Rigidbody>();
			rbRight.useGravity = true;

			rbRight.AddForce(_shellRight1stPerson.transform.up * 2f, ForceMode.Impulse);

			_shellRight1stPerson.SetActive(false);

			StartCoroutine(AddColliderDelayed(ejectedShellRight));

			if (PlayerMagazineAmmoCurrent == 0)
			{
				GameObject ejectedShellLeft = Instantiate(_shellLeft1stPerson, _shellLeft1stPerson.transform.position, _shellLeft1stPerson.transform.rotation);

				ejectedShellLeft.transform.SetParent(null);
				ejectedShellLeft.layer = LayerMask.NameToLayer("Default");
				SceneManager.MoveGameObjectToScene(ejectedShellLeft, SceneManager.GetSceneByBuildIndex(1));

				Renderer rendererLeft = ejectedShellLeft.GetComponent<Renderer>();
				rendererLeft.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

				Rigidbody rbLeft = ejectedShellLeft.AddComponent<Rigidbody>();
				rbLeft.useGravity = true;

				rbLeft.AddForce(_shellLeft1stPerson.transform.up * 2f, ForceMode.Impulse);

				_shellLeft1stPerson.SetActive(false);

				StartCoroutine(AddColliderDelayed(ejectedShellLeft));
			}
		}
		else
		{
			GameObject ejectedShellRight = Instantiate(_shellRight3rdPerson, _shellRight3rdPerson.transform.position, _shellRight3rdPerson.transform.rotation);
			ejectedShellRight.transform.SetParent(null);
			ejectedShellRight.layer = LayerMask.NameToLayer("Default");
			SceneManager.MoveGameObjectToScene(ejectedShellRight, SceneManager.GetSceneByBuildIndex(1));

			Renderer rendererRight = ejectedShellRight.GetComponent<Renderer>();
			rendererRight.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

			Rigidbody rbRight = ejectedShellRight.AddComponent<Rigidbody>();
			rbRight.useGravity = true;
			rbRight.AddForce(_shellRight3rdPerson.transform.up * 2f, ForceMode.Impulse);

			_shellRight3rdPerson.SetActive(false);

			StartCoroutine(AddColliderDelayed(ejectedShellRight));

			if (PlayerMagazineAmmoCurrent == 0)
			{
				GameObject ejectedShellLeft = Instantiate(_shellLeft3rdPerson, _shellLeft3rdPerson.transform.position, _shellLeft3rdPerson.transform.rotation);
				ejectedShellLeft.transform.SetParent(null);
				ejectedShellLeft.layer = LayerMask.NameToLayer("Default");
				SceneManager.MoveGameObjectToScene(ejectedShellLeft, SceneManager.GetSceneByBuildIndex(1));

				Renderer rendererLeft = ejectedShellLeft.GetComponent<Renderer>();
				rendererLeft.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

				Rigidbody rbLeft = ejectedShellLeft.AddComponent<Rigidbody>();
				rbLeft.useGravity = true;
				rbLeft.AddForce(_shellLeft3rdPerson.transform.up * 2f, ForceMode.Impulse);

				_shellLeft3rdPerson.SetActive(false);

				StartCoroutine(AddColliderDelayed(ejectedShellLeft));
			}
		}
	}

	private IEnumerator AddColliderDelayed(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		obj.AddComponent<BoxCollider>();
	}
}