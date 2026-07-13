using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _weaponController;
	private Animator _playerAnimator;
	private LegKickAttackController _legKickAttack;

	private int _layerWeaponRightEquip;
	private int _layerWeaponLeftEquip;
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
		GameObject player)
	{
		_bootstrap = bootstrap;
		_playerAnimator = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_weaponController = weaponController;
		_legKickAttack = legKickAttack;

		_weaponController.OnWeaponShoot += OnWeaponShoot;

		_weaponController.OnShowWeaponRight += ShowWeaponRight;
		_weaponController.OnHideWeaponRight += HideWeaponRight;
		_weaponController.OnShowWeaponLeft += ShowWeaponLeft;
		_weaponController.OnHideWeaponLeft += HideWeaponLeft;



		_layerWeaponRightEquip = _playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layerWeaponLeftEquip = _playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layerLegKick = _playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString());

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;

		Debug.Log("WeaponAnimationController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized) return;

		HandleCameraRotation();
	}

	private void ShowWeaponRight()
	{
		_playerAnimator.SetLayerWeight(_layerWeaponRightEquip, 1);
		ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Right.ToString());
	}

	private void HideWeaponRight()
	{
		ChangePlayerWeaponRightAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString());

		var stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(_layerWeaponRightEquip);
		if (stateInfo.IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Right.ToString()) && stateInfo.normalizedTime >= 0.99f)
		{
			_playerAnimator.SetLayerWeight(_layerWeaponRightEquip, 0);
		}
	}
	private void ShowWeaponLeft()
	{
		_playerAnimator.SetLayerWeight(_layerWeaponLeftEquip, 1);
		ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.EquipWeapon_Left.ToString());
	}
	private void HideWeaponLeft()
	{
		ChangePlayerWeaponLeftAnimation(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString());

		var stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(_layerWeaponLeftEquip);
		// Проверяем имя состояния через ToString() от enum, как это сделано в правом обработчике
		if (stateInfo.IsName(AnimationsHumanoidWeaponsEnum.UnequipWeapon_Left.ToString()) && stateInfo.normalizedTime >= 0.99f)
		{
			_playerAnimator.SetLayerWeight(_layerWeaponLeftEquip, 0);
		}
	}

	private void HandleCameraRotation()
	{
		float cameraRotationX = _playerCameraStateMachineController.transform.rotation.eulerAngles.x;
		_adjustedCameraAngle = (cameraRotationX >= 0 && cameraRotationX < 180) ? cameraRotationX : cameraRotationX - 360;

		float startValue = _playerAnimator.GetFloat("UpDown");
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
		_playerAnimator.SetFloat("UpDown", newValue);
	}


	private void OnWeaponShoot(WeaponHandsEnum weaponHandType)
	{
		// Логика обработки выстрела остается без изменений
	}

	private void ChangePlayerWeaponRightAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponRightAnimation != animation)
		{
			_currentPlayerWeaponRightAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade, _layerWeaponRightEquip);
		}
	}

	private void ChangePlayerWeaponLeftAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerWeaponLeftAnimation != animation)
		{
			_currentPlayerWeaponLeftAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade, _layerWeaponLeftEquip);
		}
	}

	private void HandleLegKickStateChange(bool isKicking)
	{
		if (isKicking)
		{
			_playerAnimator.SetLayerWeight(_layerLegKick, 1);
			_playerAnimator.Play(AnimationsHumanoidWeaponsEnum.LegKick.ToString(), _layerLegKick, 0f);
		}
		else
		{
			_playerAnimator.SetLayerWeight(_layerLegKick, 0);
		}
	}
}