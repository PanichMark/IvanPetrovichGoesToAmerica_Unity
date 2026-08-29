using System.Collections;
using UnityEngine;

public class WeaponSpecialMonocular : WeaponAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Monocular;
	private Coroutine _monocularScopeCoroutine;
	public override WeaponTypes WeaponType => WeaponTypes.Special;

	public override float WeaponDamage => 0;
	private SeethroughSceneObjectsRegistrator _seethroughSceneObjectsRegistrator;
	public override bool IsWeaponAuto => true;
	private GameObject _canvasHUDmonocular;
	private GameplayCanvases _gameCanvasesList;
	public override float WeaponAttackSpeedRate => throw new System.NotImplementedException();

	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		throw new System.NotImplementedException();
	}

	public override void InitializeWeapon()
	{
		_gameCanvasesList = ServiceLocator.Resolve<GameplayCanvases>();
		_canvasHUDmonocular = _gameCanvasesList.CanvasHUDmonocular;

		_seethroughSceneObjectsRegistrator = FindObjectOfType<SeethroughSceneObjectsRegistrator>();
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		//throw new System.NotImplementedException();
	}

	public override void StopAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override void WeaponAttack()
	{
		if (_isAttacking)
		{
			return;
		}

		StartCoroutine(ScopeMonocular());
	}

	
	private IEnumerator ScopeMonocular()
	{
		_canvasHUDmonocular.SetActive(true);

		_isAttacking = true;

		if (_seethroughSceneObjectsRegistrator.SeethroughSceneObject.Count > 0)
		{
			foreach (GameObject obj in _seethroughSceneObjectsRegistrator.SeethroughSceneObject)
			{
				if (obj != null)
				{
					obj.layer = LayerMask.NameToLayer("Seethrough");
				}
			}
		}

		if (!_weaponAudioSource.isPlaying)
		{
			//_weaponAudioSource.PlayOneShot(_weaponSoundAttack);
		}

		_monocularScopeCoroutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this, false));

		yield return _monocularScopeCoroutine;

		if (_seethroughSceneObjectsRegistrator.SeethroughSceneObject.Count > 0)
		{
			foreach (GameObject obj in _seethroughSceneObjectsRegistrator.SeethroughSceneObject)
			{
				if (obj != null)
				{
					obj.layer = LayerMask.NameToLayer("Default");
				}
			}
		}

		_canvasHUDmonocular.SetActive(false);
		_isAttacking = false;

		//_currentWeaponPlayerMeleeAttackRoutine = null;
	}
	
}
