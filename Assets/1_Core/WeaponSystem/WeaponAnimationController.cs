using System.Collections;
using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _weaponController;

	private Animator _playerAnimator1stPerson;
	private Animator _playerAnimator3rdPerson;

	private LegKickAttackController _legKickAttack;

	private int _layer1stWeaponRightEquip;
	private int _layer1stWeaponRightUse;
	private int _layer1stWeaponLeftEquip;
	private int _layer1stWeaponLeftUse;
	private int _layer1stWeaponReload;

	private int _layer3rdWeaponRightEquip;
	private int _layer3rdWeaponRightUse;
	private int _layer3rdWeaponLeftEquip;
	private int _layer3rdWeaponLeftUse;
	private int _layer3rdWeaponReload;
	private int _layer3rdLegKick;

	private string _currentPlayerWeaponRightAnimation1st;
	private string _currentPlayerWeaponRightAnimation3rd;
	private string _currentPlayerWeaponLeftAnimation1st;
	private string _currentPlayerWeaponLeftAnimation3rd;

	private float _adjustedCameraAngle;

	public void Initialize(
		Bootstrap bootstrap,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		LegKickAttackController legKickAttack,
		GameObject player,
		GameObject playerCamera)
	{
		_bootstrap = bootstrap;
		_playerAnimator1stPerson =playerCamera.GetComponent<Animator>();
		_playerAnimator3rdPerson = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_weaponController = weaponController;
		_legKickAttack = legKickAttack;

		_weaponController.OnShowWeaponRight += ShowWeaponRight;
		_weaponController.OnHideWeaponRight += HideWeaponRight;
		_weaponController.OnShowWeaponLeft += ShowWeaponLeft;
		_weaponController.OnHideWeaponLeft += HideWeaponLeft;

		_weaponController.OnWeaponShoot += OnWeaponShoot;

		_layer1stWeaponRightEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer1stWeaponRightUse = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightUse.ToString());
		_layer1stWeaponLeftEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer1stWeaponLeftUse = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftUse.ToString());
		_layer1stWeaponReload = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponReload.ToString());

		_layer3rdWeaponRightEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer3rdWeaponRightUse = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightUse.ToString());
		_layer3rdWeaponLeftEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer3rdWeaponLeftUse =_playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftUse.ToString());
		_layer3rdWeaponReload = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponReload.ToString());
		_layer3rdLegKick = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString());

		_playerAnimator1stPerson.SetLayerWeight(0, 0);

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		HandleLookUpDown();

		/*
		Debug.Log("##########  FIRST PERSON  ###########");
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(2));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(3));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(4));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(5));
		Debug.Log("######################################");

		Debug.Log("##########  THIRD PERSON  ###########");
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(2));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(3));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(4));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(5));
		Debug.Log("######################################");
		*/
	}

	private void ShowWeaponRight()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 1);
		ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, WeaponHandsEnum.HandRight, AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString(), _layer1stWeaponRightEquip);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layer1stWeaponRightUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 1);
		ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, WeaponHandsEnum.HandRight, AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString(), _layer3rdWeaponRightEquip);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layer3rdWeaponRightUse);
	}

	private void HideWeaponRight()
	{
		ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, WeaponHandsEnum.HandRight, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString(), _layer1stWeaponRightEquip);
		StartCoroutine(WaitForHandToGoDown(_playerAnimator1stPerson, _layer1stWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 0);

		ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, WeaponHandsEnum.HandRight, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString(), _layer3rdWeaponRightEquip);
		StartCoroutine(WaitForHandToGoDown(_playerAnimator3rdPerson, _layer3rdWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 0);
	}

	private void ShowWeaponLeft()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 1);
		ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, WeaponHandsEnum.HandLeft, AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString(), _layer1stWeaponLeftEquip);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layer1stWeaponLeftUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 1);
		ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, WeaponHandsEnum.HandLeft, AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString(), _layer3rdWeaponLeftEquip);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layer3rdWeaponLeftUse);
	}

	private void HideWeaponLeft()
	{
		ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, WeaponHandsEnum.HandLeft, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString(), _layer1stWeaponLeftEquip);
		StartCoroutine(WaitForHandToGoDown(_playerAnimator1stPerson, _layer1stWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 0);

		ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, WeaponHandsEnum.HandLeft, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString(), _layer3rdWeaponLeftEquip);
		StartCoroutine(WaitForHandToGoDown(_playerAnimator3rdPerson, _layer3rdWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 0);
	}

	private IEnumerator WaitForHandToGoDown(Animator targetAnimator, int layerIndex, string stateName)
	{
		var stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
		while (!stateInfo.IsName(stateName) || stateInfo.normalizedTime < 0.99f)
		{
			yield return null;
			stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
		}
		targetAnimator.SetLayerWeight(layerIndex, 0);

		//yield return null;
	}

	private void HandleLookUpDown()
	{
		float cameraRotationX = _playerCameraStateMachineController.transform.rotation.eulerAngles.x;
		_adjustedCameraAngle = (cameraRotationX >= 0 && cameraRotationX < 180) ? cameraRotationX : cameraRotationX - 360;

		float startValue = _playerAnimator3rdPerson.GetFloat("UpDown");
		float endValue = 0f;

		if (_playerBehaviour.IsPlayerArmed)
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
			{
				endValue = _adjustedCameraAngle * 0.0153846f;
			}
			else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
			{
				endValue = 0f;
			}
		}

		float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);
		_playerAnimator3rdPerson.SetFloat("UpDown", newValue);
	}

	private void OnWeaponShoot(WeaponHandsEnum weaponHandType)
	{

	}

	private void ChangePlayerWeaponEquipAnimation(Animator animator, WeaponHandsEnum weaponHand, string animation, int layer)
	{
		if (weaponHand == WeaponHandsEnum.HandRight)
		{
			if (animator == _playerAnimator1stPerson)
			{
				if (_currentPlayerWeaponRightAnimation1st != animation)
				{
					_currentPlayerWeaponRightAnimation1st = animation;
					animator.CrossFade(animation, 0.2f, layer);
				}
			}
			else
			{
				if (_currentPlayerWeaponRightAnimation3rd != animation)
				{
					_currentPlayerWeaponRightAnimation3rd = animation;
					animator.CrossFade(animation, 0.2f, layer);
				}
			}
		}
		else
		{
			if (animator == _playerAnimator1stPerson)
			{
				if (_currentPlayerWeaponLeftAnimation1st != animation)
				{
					_currentPlayerWeaponLeftAnimation1st = animation;
					animator.CrossFade(animation, 0.2f, layer);
				}
			}
			else
			{
				if (_currentPlayerWeaponLeftAnimation3rd != animation)
				{
					_currentPlayerWeaponLeftAnimation3rd = animation;
					animator.CrossFade(animation, 0.2f, layer);
				}
			}
		}
	}

	private void HandleLegKickStateChange(bool isKicking)
	{
		if (isKicking)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdLegKick, 1);
			_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.LegKick.ToString(), _layer3rdLegKick, 0f);
		}
		else
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdLegKick, 0);
		}
	}

	public void PrepareReloadAnimation(WeaponsRangedEnum rangedWeaponType)
	{
		if (rangedWeaponType == WeaponsRangedEnum.HarmonicaRevolver)
		{

		}
		if (rangedWeaponType == WeaponsRangedEnum.BergmannBayard)
		{

		}
		if (rangedWeaponType == WeaponsRangedEnum.SawedOffShotgun)
		{

		}
	}
}