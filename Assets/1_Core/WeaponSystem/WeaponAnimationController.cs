using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
	private PlayerBehaviour _playerBehaviour;

	private PlayerCameraController _playerCameraController;
	private PlayerWeaponController _weaponController;

	private Animator _playerAnimator;
	private bool _isInitialized = false;
	private LegKickAttackController _legKickAttack;
	private string _currentPlayerWeaponRightAnimation = "";
	private string _currentPlayerWeaponLeftAnimation = "";
	private string _currentPlayerLegKickAttackAnimation = "";

	private bool _wasPreviouslyKicking = false;

	private float _adjustedCameraAngle;
	public void Initialize(
		PlayerBehaviour playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerWeaponController weaponController,
		LegKickAttackController legKickAttack,
		GameObject player)
	{
		_playerAnimator = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
	
		_playerCameraController = playerCameraController;
		_weaponController = weaponController;
		_legKickAttack = legKickAttack;

		_isInitialized = true;

		Debug.Log("WeaponAnimationController Initialized");
	}

	private void Update()
	{
		if (!_isInitialized)
			return;

		float cameraRotationX = _playerCameraController.transform.rotation.eulerAngles.x;
		if (cameraRotationX >= 0 && cameraRotationX < 180)
		{
			_adjustedCameraAngle = cameraRotationX;
		}
		else if (cameraRotationX < 360 && cameraRotationX > -180)
		{
			_adjustedCameraAngle = cameraRotationX - 360;
		}

		if (_playerBehaviour.IsPlayerArmed == true && _playerCameraController.CurrentPlayerCameraStateType == "ThirdPerson")
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

		if (_playerBehaviour.IsPlayerArmed == true && _playerCameraController.CurrentPlayerCameraStateType == "FirstPerson")
		{
			_playerAnimator.SetFloat("UpDown", 0);
		}

		if (_weaponController.RightHandWeapon != null)
		{
			if (_weaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 1);
				ChangePlayerWeaponRightAnimation("EquipRightWeapon");
			}
			else
			{
				ChangePlayerWeaponRightAnimation("UnequipRightWeapon");
				if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponRight")).IsName("UnequipRightWeapon") && _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponRight")).normalizedTime >= 0.99f)
				{
					_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponRightAnimation("UnequipRightWeapon");
			if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponRight")).IsName("UnequipRightWeapon") && _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponRight")).normalizedTime >= 0.99f)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 0);
			}
		}

		if (_weaponController.LeftHandWeapon != null)
		{
			if (_weaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponLeft"), 1);
				ChangePlayerWeaponLeftAnimation("EquipLeftWeapon");
			}
			else
			{
				ChangePlayerWeaponLeftAnimation("UnequipLeftWeapon");

				if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponLeft")).IsName("UnequipLeftWeapon") && _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponLeft")).normalizedTime >= 0.99f)
				{
					_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponLeft"), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponLeftAnimation("UnequipLeftWeapon");

			if (_playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponLeft")).IsName("UnequipLeftWeapon") && _playerAnimator.GetCurrentAnimatorStateInfo(_playerAnimator.GetLayerIndex("WeaponLeft")).normalizedTime >= 0.99f)
			{
				_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponLeft"), 0);
			}
		}

		if (_legKickAttack.IsPlayerLegKicking == true)
		{

			_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("LegKick"), 1);

			if (!_wasPreviouslyKicking)
			{
				_playerAnimator.Play("LegKick", 4, 0f); 
			}

			_playerAnimator.Play("LegKick");
		}
		else if (_legKickAttack.IsPlayerLegKicking == false)
		{
			_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("LegKick"), 0);
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

	private void ChangePlayerLegKickAttackAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerLegKickAttackAnimation != animation)
		{
			_currentPlayerLegKickAttackAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}
}