using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public abstract int ManaCost {  get; }
	protected GameObject _eugenicSourcePoint;
	protected GameObject _eugenicAttackDirection;

	protected PlayerManaController _playerResourcesManaManager;
	protected Coroutine _currentWeaponPlayerEugenicAttackRoutine;

	protected GameObject _eugenicEffectRightHand;
	protected GameObject _eugenicEffectLeftHand;
	protected GameObject _eugenicBottle;
	protected GameObject _eugenicBottleCap;

	[SerializeField] protected GameObject _VFXeffect;
	protected Transform _VFXspawnPoint;
	protected GameObject _vfxInstanceAttack;

	protected GameObject _vfxInstanceInspectRight;
	protected GameObject _vfxInstanceInspectLeft;

	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon == true)
		{
			_eugenicAttackDirection = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
			_eugenicSourcePoint = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);

			_playerResourcesManaManager = ServiceLocator.Resolve<PlayerManaController>();
		}

		InitializeWeaponEugenic();
	}

	public override void WeaponAttack()
	{
		if (_playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
		{
			if (_isAttacking)
			{
				Debug.Log("Already attacking eugenic");
				return;
			}

			if (IsWeaponAuto)
			{
				_isAttacking = true;
				StartAutoAttackingWeaponPlayer();
			}
			else
			{
				_isAttacking = true;
				StartCoroutine(SingleEugenicAttack());
			}
		}

	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		//Debug.Log("SART!!!");

		if (IsWeaponPlayerAutoAttacking) return;
		IsWeaponPlayerAutoAttacking = true;

		//Debug.Log(_currentWeaponPlayerAutoAttackCourutine);
		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			//Debug.Log("SART!!!");
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		IsWeaponPlayerAutoAttacking = false;
		if (_currentWeaponPlayerAutoAttackCourutine != null)
		{
			StartCoroutine(StopEugenucAudioWithDelay());
			//TurnEugenicVFXOff();

			StopCoroutine(_currentWeaponPlayerAutoAttackCourutine);
			_currentWeaponPlayerAutoAttackCourutine = null;
		}
	}

	private IEnumerator StopEugenucAudioWithDelay()
	{
		yield return new WaitForSeconds(0.3f);
		_weaponAudioSource.Stop();
		yield return null;
	}

	public abstract void TurnEugenicVFXOff();

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (true)
		{

			//Debug.Log("sdverbesfrbegh");
			if (!IsWeaponPlayerAutoAttacking)
			{
				break;
			}

			StartCoroutine(SingleEugenicAttack());

			yield return new WaitForSeconds(WeaponAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				IsWeaponPlayerAutoAttacking = false;
				break;
			}
		}
		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator SingleEugenicAttack()
	{
		yield return null;
	}

	protected abstract void InitializeWeaponEugenic();

	private void OnDestroy()
	{
		//TurnEugenicVFXOff();
	}

	public virtual void HideEugenicBottleCap()
	{
		_eugenicBottleCap.SetActive(false);
	
	}

	public virtual void ShowEugenicEffectBothHands()
	{
		_eugenicEffectRightHand.SetActive(true);
		_eugenicEffectLeftHand.SetActive(true);

		_eugenicBottle.SetActive(false);
	}

	public void ShowOnlyEugenicWeaponBottle()
	{
		_eugenicEffectRightHand = transform.Find("Eugenic.R").gameObject;
		_eugenicEffectLeftHand = transform.Find("Eugenic.L").gameObject;
		_eugenicBottle = transform.Find("Armature.R/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R/EugenicBottle").gameObject;
		_eugenicBottleCap = transform.Find("Armature.R/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R/EugenicBottle/EugenicBottleCap").gameObject;

		_eugenicEffectRightHand.SetActive(false);
		_eugenicEffectLeftHand.SetActive(false);

		_eugenicBottle.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, false);
		_eugenicBottle.gameObject.SetActive(true);
	}

	public override IEnumerator InspectWeaponAnimation()
	{
		//Destroy(gameObject);
		//Destroy(gameObject);
		_playerWeaponAnimationController.AnimationInspectWeapon(this);

		//Debug.Log("BEFORE WAIT: " + _eugenicEffectRightHand.name + " Instance ID: " + _eugenicEffectRightHand.GetInstanceID());

		yield return new WaitForSecondsRealtime(3.33f);

		HideEugenicBottleCap();

		yield return new WaitForSecondsRealtime(4.58f);

		ShowEugenicEffectBothHands();

		ShowWeaponInspectVFXs();

		ChangeInspectVFXStransform();

		yield return new WaitForSecondsRealtime(3.2f);

		HideWeaponInspectVFXs();

		yield return new WaitForSecondsRealtime(0.8f);

		// Смотрим, тот ли это вообще объект по ID
		//Debug.Log("BEFORE DESTROY: " + _eugenicEffectRightHand.name + " Instance ID: " + _eugenicEffectRightHand.GetInstanceID());




		Destroy(_eugenicEffectRightHand);
		Destroy(_eugenicEffectLeftHand);
		Destroy(_eugenicBottle);
		Destroy(_eugenicBottleCap);
		Destroy(gameObject);


		//Destroy(FirstPersonWeaponModelInstance);

		yield return null;
	}

	private void ShowWeaponInspectVFXs()
	{
		_vfxInstanceInspectRight = Instantiate(_VFXeffect);
		_vfxInstanceInspectRight.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, false);
		_vfxInstanceInspectRight.layer = LayerMask.NameToLayer("FirstPerson");

		_vfxInstanceInspectLeft = Instantiate(_VFXeffect);
		_vfxInstanceInspectLeft.transform.SetParent(_firstPersonLeftHandWeaponSlotTransform, false);
		_vfxInstanceInspectLeft.layer = LayerMask.NameToLayer("FirstPerson");
	}

	private void HideWeaponInspectVFXs()
	{
		Destroy(_vfxInstanceInspectRight);
		Destroy(_vfxInstanceInspectLeft);
	}

	protected virtual void ChangeInspectVFXStransform()
	{

	}
}