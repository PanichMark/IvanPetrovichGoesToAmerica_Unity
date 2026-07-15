using System.Collections;
using UnityEngine;

public class PlayerWeaponAnimationController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private PlayerWeaponController _playerWeaponController;

	private Animator _playerAnimator1stPerson;
	private Animator _playerAnimator3rdPerson;

	public delegate void ReloadHandler();
	public event ReloadHandler OnPlayerReload;

	private Coroutine _currentPlayerReloadingCoroutine;

	private LegKickAttackController _legKickAttack;

	public bool IsPlayerReloading { get; private set; }
	public WeaponHandsEnum CurrentPlayerReloadingHelpingHand {  get; private set; }

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
		GameController gameController,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		LegKickAttackController legKickAttack,
		GameObject player,
		GameObject playerCamera)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_playerAnimator1stPerson =playerCamera.GetComponent<Animator>();
		_playerAnimator3rdPerson = player.GetComponent<Animator>();
		_playerBehaviour = playerBehaviour;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_playerWeaponController = weaponController;
		_legKickAttack = legKickAttack;

		_playerWeaponController.OnShowWeaponRight += ShowWeaponRight;
		_playerWeaponController.OnHideWeaponRight += HideWeaponRight;
		_playerWeaponController.OnShowWeaponLeft += ShowWeaponLeft;
		_playerWeaponController.OnHideWeaponLeft += HideWeaponLeft;

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

		_gameController.OnPlayerEarlyDeath += CancelReloadingAnimation;
		_playerWeaponController.OnWeaponHidden += CancelReloadingAnimation;

		//_playerAnimator1stPerson.SetLayerWeight(0, 0);

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		HandleLookUpDown();

		/*
		Debug.Log("##########  FIRST PERSON  ###########");
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(0));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(1));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(2));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(3));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(4));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(5));
		Debug.Log(_playerAnimator1stPerson.GetLayerWeight(6));
		Debug.Log("######################################");

		Debug.Log("##########  THIRD PERSON  ###########");
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(0));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(1));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(2));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(3));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(4));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(5));
		Debug.Log(_playerAnimator3rdPerson.GetLayerWeight(6));
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

	public IEnumerator WeaponShootAnimation(WeaponsRangedEnum rangedWeaponType, WeaponHandsEnum weaponHandType, float weaponAttackSpeedRate)
	{
		if (weaponHandType == WeaponHandsEnum.HandRight)
		{
			Debug.Log("SHOOT RIGHT ANIMATION");
			_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Shoot_Right.ToString(), _layer1stWeaponRightUse, 0f);
			_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Shoot_Right.ToString(), _layer3rdWeaponRightUse, 0f);
		}
		else
		{
			Debug.Log("SHOOT LEFT ANIMATION");
			_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Shoot_Left.ToString(), _layer1stWeaponLeftUse, 0f);
			_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_Shoot_Left.ToString(), _layer3rdWeaponLeftUse, 0f);
		}

		yield return new WaitForSeconds(weaponAttackSpeedRate); // return until animation plays TODO;

		Debug.Log("Courutine shoot edned");

		yield return null;
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

	private void CancelReloadingAnimation()
	{ 
		if (_currentPlayerReloadingCoroutine != null)
		{
			Debug.Log("Reloading Canceled");

			_currentPlayerReloadingCoroutine = null;

			TurnOffAllWeaponLayers();

			IsPlayerReloading = false;
		}
	}

	public IEnumerator PrepareForReloadingWeapon(WeaponsRangedEnum rangedWeaponType, WeaponHandsEnum weaponHandType, bool isSingleAnimation)
	{
		//Debug.Log("REALODDIIIIIIIIING");

		if (isSingleAnimation == true)
		{
			_currentPlayerReloadingCoroutine = StartCoroutine(ReloadWeaponSingleAnimation(rangedWeaponType, weaponHandType));
		}
		else
		{
			_currentPlayerReloadingCoroutine = StartCoroutine(ReloadWeaponDoubleAnimation(rangedWeaponType, weaponHandType));
		}

		yield return _currentPlayerReloadingCoroutine;
	}

	private IEnumerator ReloadWeaponDoubleAnimation(WeaponsRangedEnum rangedWeaponType, WeaponHandsEnum weaponHandType)
	{
		float startTime = Time.time;

		//Debug.Log("DOUBEL RELOAD");
		TurnOffWeaponAttackLayers();
		IsPlayerReloading = true;

		CurrentPlayerReloadingHelpingHand = weaponHandType ^ (WeaponHandsEnum)1; //Helping hand is Alternative to weaponHandType

		OnPlayerReload?.Invoke();

		if (rangedWeaponType == WeaponsRangedEnum.HarmonicaRevolver)
		{


			if (weaponHandType == WeaponHandsEnum.HandRight)
			{
				_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_ReloadInsertCartridge_Right.ToString(), _layer1stWeaponReload, 0f);
				_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_ReloadInsertCartridge_Right.ToString(), _layer3rdWeaponReload, 0f);

				yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
			}
			else
			{
				_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_ReloadInsertCartridge_Left.ToString(), _layer1stWeaponReload, 0f);
				_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_HarmonicaRevolver_ReloadInsertCartridge_Left.ToString(), _layer3rdWeaponReload, 0f);

				yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
			}

			yield return new WaitForSeconds(_playerAnimator1stPerson.GetCurrentAnimatorStateInfo(_layer1stWeaponReload).length);


		}


		TurnOnWeaponAttackLayers();

		IsPlayerReloading = false;


		// Вычисляем разницу во времени
		float elapsedTime = Time.time - startTime;

		// Выводим результат в консоль
		Debug.Log($"Корутина завершена за {elapsedTime:F2} секунд. аа");

		yield return null;
	}

	private IEnumerator ReloadWeaponSingleAnimation(WeaponsRangedEnum rangedWeaponType, WeaponHandsEnum weaponHandType)
	{
		float startTime = Time.time;

		TurnOffWeaponAttackLayers();
		IsPlayerReloading = true;

		CurrentPlayerReloadingHelpingHand = weaponHandType ^ (WeaponHandsEnum)1; //Helping hand is Alternative to weaponHandType

		OnPlayerReload?.Invoke();




		if (rangedWeaponType == WeaponsRangedEnum.BergmannBayard)
		{

		}
		if (rangedWeaponType == WeaponsRangedEnum.SawedOffShotgun)
		{

		}

		// Вычисляем разницу во времени
		float elapsedTime = Time.time - startTime;

		// Выводим результат в консоль
		Debug.Log($"Корутина завершена за {elapsedTime:F2} секунд. аа");

		TurnOnWeaponAttackLayers();

		IsPlayerReloading = false;

		yield return null;
	}

	private void TurnOnWeaponAttackLayers()
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 1);
		}
		if (_playerWeaponController.LeftHandWeapon != null)
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 1);
		}

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 0);

		if (_playerWeaponController.RightHandWeapon != null)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 1);
		}
		if (_playerWeaponController.LeftHandWeapon != null)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 1);
		}

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 0);
	}

	private void TurnOffWeaponAttackLayers()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 0);

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 1);
		
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 0);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 1);
	}

	private void TurnOffAllWeaponLayers()
	{
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightUse, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftUse, 0);

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 0);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightUse, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftUse, 0);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 0);
	}
}