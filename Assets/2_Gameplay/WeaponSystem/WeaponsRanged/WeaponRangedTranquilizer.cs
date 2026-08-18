using System.Collections;
using UnityEngine;

public class WeaponRangedTranquilizer : WeaponRangedAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Tranquilizer;
	public override WeaponTypes WeaponType => WeaponTypes.Ranged;
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.AmmoTranquilizerDart;
	public override float WeaponDamage => 0;
	public override bool IsWeaponAuto => false;
	public override float WeaponAttackSpeedRate => 0.3f;

	public override bool IsReloadingAnimationSingle => true;
	protected override float _weaponRange => 100;
	public override bool LeavesBulletHole => false;
	protected override float _waitForAmmoRefill => 3.15f;

	private GameObject _loadingGate1stPerson;
	private GameObject _loadingGate3rdPerson;

	private GameObject _dart1stPerson;
	private GameObject _dart3rdPerson;

	protected override void InitializeWeaponRanged()
	{
		_loadingGate1stPerson = FirstPersonWeaponModelInstance.transform.Find("LoadingGate").gameObject;
		_loadingGate3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("LoadingGate").gameObject;

		_dart1stPerson = _loadingGate1stPerson.transform.Find("Dart").gameObject;
		_dart3rdPerson = _loadingGate3rdPerson.transform.Find("Dart").gameObject;
	}

	public override IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = 1;
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_playerWeaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));

		StartCoroutine(LoadingGateAnimation());
		StartCoroutine(LoadTranquilizerDart());

		yield return new WaitForSeconds(_waitForAmmoRefill);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;
		PlayerMagazineAmmoCurrent += ammoToAdd;

		_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);
		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
	}

	private IEnumerator LoadingGateAnimation()
	{
		_dart1stPerson.SetActive(false);
		_dart3rdPerson.SetActive(false);

		yield return new WaitForSeconds(0.875f);

		float elapsed = 0f;
		Quaternion startRotation1st = _loadingGate1stPerson.transform.localRotation;
		Quaternion startRotation3rd = _loadingGate3rdPerson.transform.localRotation;
		Quaternion targetRotation1st = startRotation1st * Quaternion.Euler(0, 30, 0);
		Quaternion targetRotation3rd = startRotation3rd * Quaternion.Euler(0, 30, 0);
		while (elapsed < 0.417f)
		{
			float t = elapsed / 0.417f;
			_loadingGate1stPerson.transform.localRotation = Quaternion.Slerp(startRotation1st, targetRotation1st, t);
			_loadingGate3rdPerson.transform.localRotation = Quaternion.Slerp(startRotation3rd, targetRotation3rd, t);
			elapsed += Time.deltaTime;
			yield return null;
		}
		_loadingGate1stPerson.transform.localRotation = targetRotation1st;
		_loadingGate3rdPerson.transform.localRotation = targetRotation3rd;

		yield return new WaitForSeconds(2.583f);

		elapsed = 0f;
		Quaternion backTargetRotation1st = startRotation1st;
		Quaternion backTargetRotation3rd = startRotation3rd;
		while (elapsed < 0.208f)
		{
			float t = elapsed / 0.208f;
			_loadingGate1stPerson.transform.localRotation = Quaternion.Slerp(targetRotation1st, backTargetRotation1st, t);
			_loadingGate3rdPerson.transform.localRotation = Quaternion.Slerp(targetRotation3rd, backTargetRotation3rd, t);
			elapsed += Time.deltaTime;
			yield return null;
		}
		_loadingGate1stPerson.transform.localRotation = backTargetRotation1st;
		_loadingGate3rdPerson.transform.localRotation = backTargetRotation3rd;
	}

	private IEnumerator LoadTranquilizerDart()
	{
		_dart1stPerson.SetActive(false);
		_dart3rdPerson.SetActive(false);

		yield return new WaitForSeconds(3.15f);

		_dart1stPerson.SetActive(true);
		_dart3rdPerson.SetActive(true);
	}
}
