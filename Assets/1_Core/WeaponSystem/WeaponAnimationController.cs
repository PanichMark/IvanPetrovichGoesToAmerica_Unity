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

	private int _layer3rdWeaponRightEquip;
	private int _layer3rdWeaponRightUse;
	private int _layer3rdWeaponLeftEquip;
	private int _layer3rdWeaponLeftUse;
	private int _layer3rdLegKick;

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

		_layer1stWeaponRightEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer1stWeaponRightUse = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightUse.ToString());
		_layer1stWeaponLeftEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer1stWeaponLeftUse = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftUse.ToString());

		_layer3rdWeaponRightEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer3rdWeaponRightUse = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightUse.ToString());
		_layer3rdWeaponLeftEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer3rdWeaponLeftUse =_playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftUse.ToString());
		_layer3rdLegKick = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString());

		_playerAnimator1stPerson.SetLayerWeight(0, 0);

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		HandleCameraRotation();

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
	}

	private void ShowWeaponRight()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 1);
		ChangePlayerWeaponRightAnimation(_playerAnimator1stPerson, AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString(), _layer1stWeaponRightEquip);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layer1stWeaponRightUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 1);
		ChangePlayerWeaponRightAnimation(_playerAnimator3rdPerson, AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString(), _layer3rdWeaponRightEquip);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Right.ToString(), _layer3rdWeaponRightUse);
	}

	private void HideWeaponRight()
	{
		ChangePlayerWeaponRightAnimation(_playerAnimator1stPerson,AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString(), _layer1stWeaponRightEquip);
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator1stPerson, _layer1stWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 0);

		ChangePlayerWeaponRightAnimation(_playerAnimator3rdPerson, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString(), _layer3rdWeaponRightEquip);
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator3rdPerson, _layer3rdWeaponRightEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 0);
	}

	private void ShowWeaponLeft()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 1);
		ChangePlayerWeaponLeftAnimation(_playerAnimator1stPerson, AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString(), _layer1stWeaponLeftEquip);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 1);
		_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layer1stWeaponLeftUse);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 1);
		ChangePlayerWeaponLeftAnimation(_playerAnimator3rdPerson,AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString(), _layer3rdWeaponLeftEquip);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 1);
		_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Hold_Left.ToString(), _layer3rdWeaponLeftUse);
	}

	private void HideWeaponLeft()
	{
		ChangePlayerWeaponLeftAnimation(_playerAnimator1stPerson, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString(), _layer1stWeaponLeftEquip);
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator1stPerson, _layer1stWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 0);

		ChangePlayerWeaponLeftAnimation(_playerAnimator3rdPerson, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString(), _layer3rdWeaponLeftEquip);
		StartCoroutine(WaitForAnimationAndDisable(_playerAnimator3rdPerson, _layer3rdWeaponLeftEquip, AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 0);
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

	private void ChangePlayerWeaponRightAnimation(Animator animator, string animation, int layer, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponRightAnimation != animation)
		{
			_currentPlayerWeaponRightAnimation = animation;
			animator.CrossFade(animation, crossfade, layer);
		}
	}

	private void ChangePlayerWeaponLeftAnimation(Animator animator, string animation, int layer, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponLeftAnimation != animation)
		{
			_currentPlayerWeaponLeftAnimation = animation;
			animator.CrossFade(animation, crossfade, layer);
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
}