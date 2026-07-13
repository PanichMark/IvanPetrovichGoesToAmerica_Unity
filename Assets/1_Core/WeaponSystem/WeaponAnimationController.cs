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

	private int _layerWeaponRightEquip;
	private int _layerWeaponRightUse;
	private int _layerWeaponLeftEquip;
	private int _layerWeaponLeftUse;
	private int _layerLegKick;

	private string _currentPlayerWeaponRightAnimation;
	private string _currentPlayerWeaponLeftAnimation;
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

		_layerWeaponRightEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layerWeaponRightUse = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightUse.ToString());
		_layerWeaponLeftEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layerWeaponLeftUse =_playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftUse.ToString());
		_layerLegKick = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString());

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		HandleCameraRotation();

		/*
		Debug.Log("##############################");
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(2));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(3));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(4));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(5));
		Debug.Log("##############################");
		*/
	}

	private void ShowWeaponRight()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponRightEquip, 1);
		ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString());
		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponRightUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layerWeaponRightUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponRightEquip, 1);
		ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString());
		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponRightUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layerWeaponRightUse);
	}

	private void HideWeaponRight()
	{
		ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString());

		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator3rdPerson, _layerWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator1stPerson, _layerWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));

		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponRightUse, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponRightUse, 0);
	}

	private void ShowWeaponLeft()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponLeftEquip, 1);
		ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString());
		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponLeftUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layerWeaponLeftUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponLeftEquip, 1);
		ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString());
		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponLeftUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layerWeaponLeftUse);
	}

	private void HideWeaponLeft()
	{
		ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString());

		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator3rdPerson, _layerWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator1stPerson, _layerWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));

		_playerAnimator1stPerson.SetLayerWeight(_layerWeaponLeftUse, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layerWeaponLeftUse, 0);
	}

	private IEnumerator WaitForAnimationAndDisable(Animator targetAnimator, int layerIndex, string stateName)
	{
		//Debug.Log("COURUTINE");

		var stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
		while (!stateInfo.IsName(stateName) || stateInfo.normalizedTime < 0.99f)
		{
			yield return null;
			stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
		}
		targetAnimator.SetLayerWeight(layerIndex, 0);

		//yield return null;
	}

	private void HandleCameraRotation()
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

	private void ChangePlayerWeaponRightAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponRightAnimation != animation)
		{
			_currentPlayerWeaponRightAnimation = animation;
			_playerAnimator3rdPerson.CrossFade(animation, crossfade, _layerWeaponRightEquip);
		}
	}

	private void ChangePlayerWeaponLeftAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponLeftAnimation != animation)
		{
			_currentPlayerWeaponLeftAnimation = animation;
			_playerAnimator3rdPerson.CrossFade(animation, crossfade, _layerWeaponLeftEquip);
		}
	}

	private void HandleLegKickStateChange(bool isKicking)
	{
		if (isKicking)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layerLegKick, 1);
			_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.LegKick.ToString(), _layerLegKick, 0f);
		}
		else
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layerLegKick, 0);
		}
	}
}