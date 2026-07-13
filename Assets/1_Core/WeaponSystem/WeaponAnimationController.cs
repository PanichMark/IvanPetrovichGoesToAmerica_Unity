using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private PlayerBehaviourController _playerBehaviour;

	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _weaponController;

	private Animator _playerAnimator;

	private LegKickAttackController _legKickAttack;
	private string _currentPlayerWeaponRightAnimation;
	private string _currentPlayerWeaponLeftAnimation;

	private bool _wasPreviouslyKicking = false;

	private float _adjustedCameraAngle;
	public void Initialize(
		Bootstrap bootstrap,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		LegKickAttackController legKickAttack,
		GameObject player)
	{
		_bootstrap = bootstrap;
		_playerAnimator = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
	
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_weaponController = weaponController;
		_legKickAttack = legKickAttack;

		Debug.Log("WeaponAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		float cameraRotationX = _playerCameraStateMachineController.transform.rotation.eulerAngles.x;
		if (cameraRotationX >= 0 && cameraRotationX < 180)
		{
			_adjustedCameraAngle = cameraRotationX;
		}
		else if (cameraRotationX < 360 && cameraRotationX > -180)
		{
			_adjustedCameraAngle = cameraRotationX - 360;
		}

		if (_playerBehaviour.IsPlayerArmed == true && _playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
		{
			float startValue = _playerAnimator.GetFloat("UpDown");

			float endValue = _adjustedCameraAngle * 0.0153846f;
	
			float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);

			_playerAnimator.SetFloat("UpDown", newValue);
		}
		else
		{
			float startValue = _playerAnimator.GetFloat("UpDown");
			
			float endValue = 0f;

			float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);

			_playerAnimator.SetFloat("UpDown", newValue);
		}

		if (_playerBehaviour.IsPlayerArmed == true && _playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_playerAnimator.SetFloat("UpDown", 0);
		}

		if (_weaponController.RightHandWeapon != null)
		{
			if (_weaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString()), 1);
				ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString());
			}
			else
			{
				ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString());

				if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString())).IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString())
					&& _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString())).normalizedTime >= 0.99f)
				{
					_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString()), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString());

			if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString())).IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString())
				&& _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString())).normalizedTime >= 0.99f)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString()), 0);
			}
		}

		if (_weaponController.LeftHandWeapon != null)
		{
			if (_weaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString()), 1);
				ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString());
			}
			else
			{
				ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString());

				if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString())).IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString())
					&& _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString())).normalizedTime >= 0.99f)
				{
					_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString()), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString());

			if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString())).IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString())
				&& _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString())).normalizedTime >= 0.99f)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString()), 0);
			}
		}

		if (_legKickAttack.IsPlayerLegKicking == true)
		{
			_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString()), 1);

			if (!_wasPreviouslyKicking)
			{
				_playerAnimator.Play(AnimationsHumanoidWeaponsEnum.LegKick.ToString(), (int)AnimatorControllerHumanoidLayersEnum.LayerLegKick, 0f);
			}

			_playerAnimator.Play(AnimationsHumanoidWeaponsEnum.LegKick.ToString(), (int)AnimatorControllerHumanoidLayersEnum.LayerLegKick);
		}
		else if (_legKickAttack.IsPlayerLegKicking == false)
		{
			_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString()), 0);
		}
		_wasPreviouslyKicking = _legKickAttack.IsPlayerLegKicking;
	}

	private void ChangePlayerWeaponRightAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponRightAnimation != animation)
		{
			_currentPlayerWeaponRightAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}

	private void ChangePlayerWeaponLeftAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponLeftAnimation != animation)
		{
			_currentPlayerWeaponLeftAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}
}