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

	public delegate void WeaponVisibilityHandler(GameObject weaponRoot, bool castShadows);
	public event WeaponVisibilityHandler OnShowWeapon;
	public event WeaponVisibilityHandler OnHideWeapon;

	private Coroutine _currentPlayerReloadingCoroutine;

	private LegKickAttackController _legKickAttack;

	public bool IsRightFullArmAttacking { get; private set; }
	public bool IsLeftFullArmAttacking { get; private set; }
	public bool IsReloading { get; private set; }
	public WeaponHandsEnum CurrentPlayerReloadingHelpingHand {  get; private set; }

	private TransferSkinnedMeshRendererArmatureBones _transferBonesFirstPerson;
	private TransferSkinnedMeshRendererArmatureBones _transferBonesThirdPerson;

	private int _layer1stWeaponRightEquip;
	private int _layer1stWeaponRightArm;
	private int _layer1stWeaponRightPalm;
	private int _layer1stWeaponLeftEquip;
	private int _layer1stWeaponLeftArm;
	private int _layer1stWeaponLeftPalm;
	private int _layer1stWeaponReload;

	private int _layer3rdWeaponRightEquip;
	private int _layer3rdWeaponRightArm;
	private int _layer3rdWeaponRightPalm;
	private int _layer3rdWeaponLeftEquip;
	private int _layer3rdWeaponLeftArm;
	private int _layer3rdWeaponLeftPalm;
	private int _layer3rdWeaponReload;
	private int _layer3rdLegKick;

	private float _adjustedCameraAngle;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		LegKickAttackController legKickAttack,
		TransferSkinnedMeshRendererArmatureBones transferBonesFirstPerson,
		TransferSkinnedMeshRendererArmatureBones transferBonesThirdPerson,
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

		_transferBonesFirstPerson = transferBonesFirstPerson;
		_transferBonesThirdPerson = transferBonesThirdPerson;

		_layer1stWeaponRightEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer1stWeaponRightArm = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightArm.ToString());
		_layer1stWeaponRightPalm = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightPalm.ToString());
		_layer1stWeaponLeftEquip = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer1stWeaponLeftArm = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftArm.ToString());
		_layer1stWeaponLeftPalm = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftPalm.ToString());
		_layer1stWeaponReload = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponReload.ToString());

		_layer3rdWeaponRightEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		_layer3rdWeaponRightArm = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightArm.ToString());
		_layer3rdWeaponRightPalm = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightPalm.ToString());
		_layer3rdWeaponLeftEquip = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		_layer3rdWeaponLeftArm = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftArm.ToString());
		_layer3rdWeaponLeftPalm =_playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftPalm.ToString());
		_layer3rdWeaponReload = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponReload.ToString());
		_layer3rdLegKick = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLegKick.ToString());

		_gameController.OnPlayerEarlyDeath += CancelAllWeaponsAnimation;

		_playerWeaponController.OnShowWeapon += ShowWeapon;
		_playerWeaponController.OnHideWeapon += HideWeapon;

		_playerWeaponController.OnWeaponChanged += CancelWeaponHandAnimation;

		_legKickAttack.OnLegKickStateChanged += HandleLegKickStateChange;
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		HandleLookUpDown();
	}

	private void HandleLookUpDown()
	{
		float cameraRotationX = _playerCameraStateMachineController.transform.rotation.eulerAngles.x;
		_adjustedCameraAngle = (cameraRotationX >= 0 && cameraRotationX < 180) ? cameraRotationX : cameraRotationX - 360;

		float startValue = _playerAnimator3rdPerson.GetFloat("UpDown");
		float endValue = 0f;

		if (_playerBehaviour.IsPlayerArmed)
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
			{
				endValue = _adjustedCameraAngle * 0.0153846f;
			}
			else if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				endValue = 0f;
			}
		}

		float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);
		_playerAnimator3rdPerson.SetFloat("UpDown", newValue);
	}

	private void ShowWeapon(WeaponAbstract weapon)
	{
		if (weapon.WeaponHandType == WeaponHandsEnum.Right)
		{
			ShowWeaponRight(weapon);
		}
		else
		{
			ShowWeaponLeft(weapon);
		}
	}

	private void ShowWeaponRight(WeaponAbstract weapon)
	{
		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, _layer1stWeaponRightEquip, true));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 1);
		_playerAnimator1stPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Hold}_{weapon.WeaponHandType}", _layer1stWeaponRightPalm);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, _layer3rdWeaponRightEquip, true));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 1);
		_playerAnimator3rdPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Hold}_{weapon.WeaponHandType}", _layer3rdWeaponRightPalm);

		if (weapon is WeaponEugenicAbstract)
		{
			TransferWeaponEugenicBones(WeaponHandsEnum.Right);
		}
	}

	private void ShowWeaponLeft(WeaponAbstract weapon)
	{
		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, _layer1stWeaponLeftEquip, true));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 1);
		_playerAnimator1stPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Hold}_{weapon.WeaponHandType}", _layer1stWeaponLeftPalm);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, _layer3rdWeaponLeftEquip, true));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 1);
		_playerAnimator3rdPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Hold}_{weapon.WeaponHandType}", _layer3rdWeaponLeftPalm);

		if (weapon is WeaponEugenicAbstract)
		{
			TransferWeaponEugenicBones(WeaponHandsEnum.Left);
		}
	}

	private void HideWeapon(WeaponAbstract weapon)
	{
		if (weapon.WeaponHandType == WeaponHandsEnum.Right)
		{
			HideWeaponRight(weapon);
		}
		else
		{
			HideWeaponLeft(weapon);
		}
	}

	private void HideWeaponRight(WeaponAbstract weapon)
	{
		CancelWeaponHandAnimation(WeaponHandsEnum.Right);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, _layer1stWeaponRightEquip, false));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 0);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, _layer3rdWeaponRightEquip, false));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 0);
	}

	private void HideWeaponLeft(WeaponAbstract weapon)
	{
		CancelWeaponHandAnimation(WeaponHandsEnum.Left);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator1stPerson, _layer1stWeaponLeftEquip, false));
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 0);

		StartCoroutine(ChangePlayerWeaponEquipAnimation(_playerAnimator3rdPerson, _layer3rdWeaponLeftEquip, false));
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 0);
	}

	private IEnumerator ChangePlayerWeaponEquipAnimation(Animator animator, int layer, bool equip)
	{
		float elapsed = 0f;
		float startWeight = animator.GetLayerWeight(layer);
		float targetWeight;
		float transitionSpeed = 0.5f;

		if (equip == true)
		{
			targetWeight = 1f;
		}
		else
		{
			targetWeight = 0f;
		}

		while (elapsed < transitionSpeed)
		{
			elapsed += Time.deltaTime;
			animator.SetLayerWeight(layer, Mathf.Lerp(startWeight, targetWeight, elapsed / transitionSpeed));
			yield return null;
		}

		animator.SetLayerWeight(layer, targetWeight);

		yield return null;
	}

	private void TransferWeaponEugenicBones(WeaponHandsEnum weaponHand)
	{
		GameObject eugenicArmatureFirstPerson = null;
		SkinnedMeshRenderer eugenicSkinnedMeshFirstPerson = null;
		GameObject eugenicArmatureThirdPerson = null;
		SkinnedMeshRenderer eugenicSkinnedMeshThirdPerson = null;

		GameObject deleteOtherHandArmature = null;
		GameObject deleteOtherHandEugenic = null;

		if (weaponHand == WeaponHandsEnum.Right)
		{
			eugenicArmatureFirstPerson = _playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Armature.R").gameObject;
			eugenicSkinnedMeshFirstPerson = _playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Eugenic.R").GetComponent<SkinnedMeshRenderer>();

			eugenicArmatureThirdPerson = _playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Armature.R").gameObject;
			eugenicSkinnedMeshThirdPerson = _playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Eugenic.R").GetComponent<SkinnedMeshRenderer>();
		}
		else
		{
			eugenicArmatureFirstPerson = _playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Armature.L").gameObject;
			eugenicSkinnedMeshFirstPerson = _playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Eugenic.L").GetComponent<SkinnedMeshRenderer>();

			eugenicArmatureThirdPerson = _playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Armature.L").gameObject;
			eugenicSkinnedMeshThirdPerson = _playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Eugenic.L").GetComponent<SkinnedMeshRenderer>();
		}

		_transferBonesFirstPerson.TransferWeaponEugenicBones(eugenicArmatureFirstPerson, eugenicSkinnedMeshFirstPerson, weaponHand);
		_transferBonesThirdPerson.TransferWeaponEugenicBones(eugenicArmatureThirdPerson, eugenicSkinnedMeshThirdPerson, weaponHand);

		if (weaponHand == WeaponHandsEnum.Right)
		{
			deleteOtherHandEugenic = _playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Armature.L").gameObject;
			deleteOtherHandArmature = _playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Eugenic.L").gameObject;

			Destroy(deleteOtherHandArmature);
			Destroy(deleteOtherHandEugenic);

			deleteOtherHandEugenic = _playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Armature.L").gameObject;
			deleteOtherHandArmature = _playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Eugenic.L").gameObject;

			Destroy(deleteOtherHandArmature);
			Destroy(deleteOtherHandEugenic);
		}
		else
		{
			deleteOtherHandEugenic = _playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Armature.R").gameObject;
			deleteOtherHandArmature = _playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance.transform.Find("Eugenic.R").gameObject;

			Destroy(deleteOtherHandArmature);
			Destroy(deleteOtherHandEugenic);

			deleteOtherHandEugenic = _playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Armature.R").gameObject;
			deleteOtherHandArmature = _playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.transform.Find("Eugenic.R").gameObject;

			Destroy(deleteOtherHandArmature);
			Destroy(deleteOtherHandEugenic);
		}


	}

	public IEnumerator WeaponFullArmAttackAnimation(WeaponAbstract weaponFullArm)
	{
		if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
		{
			IsRightFullArmAttacking = true;
		}
		else
		{
			IsLeftFullArmAttacking = true;
		}

		TurnOnFullArmLayer(weaponFullArm.WeaponHandType);

		if (weaponFullArm is WeaponMeleeAbstract weaponMeleeFullArm)
		{
			if (weaponMeleeFullArm.WeaponMeleeType == WeaponsMeleeTypes.Baton)
			{
				if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
				{
					Debug.Log("MELLE ATTACK RIGHT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_Baton_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponRightArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_Baton_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponRightArm, 0f);
				}
				else
				{
					Debug.Log("MELLE ATTACK LEFT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_Baton_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponLeftArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_Baton_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponLeftArm, 0f);
				}
			}
			if (weaponMeleeFullArm.WeaponMeleeType == WeaponsMeleeTypes.Knife)
			{
				if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
				{
					Debug.Log("MELLE ATTACK RIGHT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_Knife_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponRightArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_Knife_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponRightArm, 0f);
				}
				else
				{
					Debug.Log("MELLE ATTACK LEFT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_Knife_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponLeftArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_Knife_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponLeftArm, 0f);
				}
			}
			if (weaponMeleeFullArm.WeaponMeleeType == WeaponsMeleeTypes.CircularSaw)
			{
				if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
				{
					Debug.Log("MELLE ATTACK RIGHT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_CircularSaw_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponRightArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_CircularSaw_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponRightArm, 0f);
				}
				else
				{
					Debug.Log("MELLE ATTACK LEFT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_CircularSaw_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponLeftArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_CircularSaw_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponLeftArm, 0f);
				}
			}
		}
		if (weaponFullArm is WeaponEugenicAbstract weaponEugenicFullArm)
		{
			if (weaponEugenicFullArm.WeaponName == WeaponNames.GenieBreath)
			{
				if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
				{
					Debug.Log("MELLE ATTACK RIGHT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_GenieBreath_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponRightArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_GenieBreath_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponRightArm, 0f);
				}
				else
				{
					Debug.Log("MELLE ATTACK LEFT ANIMATION");
					_playerAnimator1stPerson.Play($"{weaponFullArm.WeaponType}_GenieBreath_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer1stWeaponLeftArm, 0f);
					_playerAnimator3rdPerson.Play($"{weaponFullArm.WeaponType}_GenieBreath_{AnimationsHumanoidWeaponsEnum.Attack}_{weaponFullArm.WeaponHandType}", _layer3rdWeaponLeftArm, 0f);
				}
			}
		}

		yield return new WaitForSeconds(weaponFullArm.WeaponAttackSpeedRate); // return until animation plays TODO;

		TurnOffFullArmLayer(weaponFullArm.WeaponHandType);
		//Debug.Log(weaponMelee.WeaponHandType);

		if (weaponFullArm.WeaponHandType == WeaponHandsEnum.Right)
		{
			IsRightFullArmAttacking = false;
		}
		else
		{
			IsLeftFullArmAttacking = false;
		}

		Debug.Log("Courutine MeleeAttack ended");

		yield return null;
	}

	private void TurnOnFullArmLayer(WeaponHandsEnum handType)
	{
		Debug.Log($"turn ON melee layer {handType}");

		if (handType == WeaponHandsEnum.Right)
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 0);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightArm, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 0);

			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 0);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightArm, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 0);
		}
		else
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 0);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftArm, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 0);

			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 0);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftArm, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 0);
		}
	}

	private void TurnOffFullArmLayer(WeaponHandsEnum handType)
	{
		Debug.Log($"turn OFF melee layer {handType}");

		if (handType == WeaponHandsEnum.Right)
		{
			if (IsRightFullArmAttacking == true)
			{
				_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 1);
				_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 1);

				_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 1);
				_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 1);
			}

			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightArm, 0);

			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightArm, 0);
		}
		else
		{
			if (IsLeftFullArmAttacking == true)
			{
				_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 1);
				_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 1);

				_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 1);
				_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 1);
			}

			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftArm, 0);

			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftArm, 0);
		}
	}

	public IEnumerator WeaponShootAnimation(WeaponRangedAbstract weaponRanged)
	{
		if (weaponRanged.WeaponHandType == WeaponHandsEnum.Right)
		{
			Debug.Log("SHOOT RIGHT ANIMATION");
			_playerAnimator1stPerson.Play($"{weaponRanged.WeaponType}_{weaponRanged.WeaponName}_{AnimationsHumanoidWeaponsEnum.Shoot}_{weaponRanged.WeaponHandType}", _layer1stWeaponRightPalm, 0f);
			_playerAnimator3rdPerson.Play($"{weaponRanged.WeaponType}_{weaponRanged.WeaponName}_{AnimationsHumanoidWeaponsEnum.Shoot}_{weaponRanged.WeaponHandType}", _layer3rdWeaponRightPalm, 0f);
		}
		else
		{
			Debug.Log("SHOOT LEFT ANIMATION");
			_playerAnimator1stPerson.Play($"{weaponRanged.WeaponType}_{weaponRanged.WeaponName}_{AnimationsHumanoidWeaponsEnum.Shoot}_{weaponRanged.WeaponHandType}", _layer1stWeaponLeftPalm, 0f);
			_playerAnimator3rdPerson.Play($"{weaponRanged.WeaponType}_{weaponRanged.WeaponName}_{AnimationsHumanoidWeaponsEnum.Shoot}_{weaponRanged.WeaponHandType}", _layer3rdWeaponLeftPalm, 0f);
		}

		yield return new WaitForSeconds(weaponRanged.WeaponAttackSpeedRate); // return until animation plays TODO;

		Debug.Log("Courutine shoot ended");

		yield return null;
	}

	public IEnumerator PrepareForReloadingWeapon(WeaponRangedAbstract weaponRanged, bool isSingleAnimation, bool isSecondAnimation)
	{
		HideReloadingHelpingHandWeapon(weaponRanged);

		if (isSingleAnimation == true)
		{
			_currentPlayerReloadingCoroutine = StartCoroutine(ReloadWeaponSingleAnimation(weaponRanged));
		}
		else
		{
			_currentPlayerReloadingCoroutine = StartCoroutine(ReloadWeaponDoubleAnimation(weaponRanged));
		}

		yield return _currentPlayerReloadingCoroutine;

		if (!isSecondAnimation)
		{
			if (_playerWeaponController.RightHandWeaponComponent is WeaponRangedAbstract)
			{
				WeaponRangedAbstract weaponRight = _playerWeaponController.RightHandWeaponComponent as WeaponRangedAbstract;
			
				if (weaponRanged.WeaponHandType == WeaponHandsEnum.Left && (weaponRight.PlayerMagazineAmmoCurrent < weaponRight.PlayerMagazineAmmoMax))
				{
					if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
					{
						OnShowWeapon?.Invoke(weaponRight.FirstPersonWeaponModelInstance, true);
					}
					else
					{
						OnShowWeapon?.Invoke(weaponRight.ThirdPersonWeaponModelInstance, true);
					}

					yield return StartCoroutine(weaponRight.ReloadWeaponPlayer(true));
				}
			}
		}
		else
		{
			yield return null;
		}

		ShowReloadingHelpingHandWeapon(weaponRanged);
	}

	private IEnumerator ReloadWeaponSingleAnimation(WeaponRangedAbstract weapon)
	{
		float startTime = Time.time;

		TurnOnReloadLayers();
		IsReloading = true;

		CurrentPlayerReloadingHelpingHand = weapon.WeaponHandType ^ (WeaponHandsEnum)1; //Helping hand is Alternative to weaponHandType

		OnPlayerReload?.Invoke();

		_playerAnimator1stPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Reload}_{weapon.WeaponHandType}", _layer1stWeaponReload, 0f);
		_playerAnimator3rdPerson.Play($"{weapon.WeaponType}_{weapon.WeaponName}_{AnimationsHumanoidWeaponsEnum.Reload}_{weapon.WeaponHandType}", _layer3rdWeaponReload, 0f);

		yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 

		yield return new WaitForSeconds(_playerAnimator1stPerson.GetCurrentAnimatorStateInfo(_layer1stWeaponReload).length);

		// Вычисляем разницу во времени
		float elapsedTime = Time.time - startTime;

		// Выводим результат в консоль
		Debug.Log($"Корутина завершена за {elapsedTime:F2} секунд");

		TurnOffReloadLayers();

		IsReloading = false;

		yield return null;
	}

	private IEnumerator ReloadWeaponDoubleAnimation(WeaponRangedAbstract weaponRanged)
	{
		float startTime = Time.time;

		//Debug.Log("DOUBEL RELOAD");
		TurnOnReloadLayers();
		IsReloading = true;

		CurrentPlayerReloadingHelpingHand = weaponRanged.WeaponHandType ^ (WeaponHandsEnum)1; //Helping hand is Alternative to weaponHandType

		OnPlayerReload?.Invoke();

		if (weaponRanged.WeaponName == WeaponNames.Revolver)
		{
			if (weaponRanged.WeaponHandType == WeaponHandsEnum.Right)
			{
				if (weaponRanged.PlayerMagazineAmmoCurrent == 0)
				{
					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Right.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Right.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
				}
				else
				{
					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadPush_Right.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadPush_Right.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 

					yield return new WaitForSeconds(_playerAnimator1stPerson.GetCurrentAnimatorStateInfo(_layer1stWeaponReload).length);

					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Right.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Right.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
				}
			}
			else
			{
				if (weaponRanged.PlayerMagazineAmmoCurrent == 0)
				{
					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Left.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Left.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
				}
				else
				{
					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadPush_Left.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadPush_Left.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 

					yield return new WaitForSeconds(_playerAnimator1stPerson.GetCurrentAnimatorStateInfo(_layer1stWeaponReload).length);

					_playerAnimator1stPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Left.ToString(), _layer1stWeaponReload, 0f);
					_playerAnimator3rdPerson.Play(AnimationsHumanoidWeaponsEnum.Ranged_Revolver_ReloadInsert_Left.ToString(), _layer3rdWeaponReload, 0f);

					yield return null; // Make Unity wait to load Anim into RAM, else if not, Animator returns default Anim length = 1f sec 
				}
			}

			yield return new WaitForSeconds(_playerAnimator1stPerson.GetCurrentAnimatorStateInfo(_layer1stWeaponReload).length);
		}

		TurnOffReloadLayers();

		IsReloading = false;

		// Вычисляем разницу во времени
		float elapsedTime = Time.time - startTime;

		// Выводим результат в консоль
		Debug.Log($"Корутина завершена за {elapsedTime:F2} секунд");

		yield return null;
	}

	private void ShowReloadingHelpingHandWeapon(WeaponRangedAbstract weaponRanged)
	{
		if (weaponRanged.WeaponHandType == WeaponHandsEnum.Right && _playerWeaponController.LeftHandWeapon != null)
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				OnShowWeapon?.Invoke(_playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}
			else
			{
				OnShowWeapon?.Invoke(_playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}
		}
		else if (weaponRanged.WeaponHandType == WeaponHandsEnum.Left && _playerWeaponController.RightHandWeapon != null)
		{
			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				OnShowWeapon?.Invoke(_playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}
			else
			{
				OnShowWeapon?.Invoke(_playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}
		}
	}

	private void HideReloadingHelpingHandWeapon(WeaponRangedAbstract weaponRanged)
	{
		if (weaponRanged.WeaponHandType == WeaponHandsEnum.Right && _playerWeaponController.LeftHandWeapon != null)
		{
			OnHideWeapon?.Invoke(_playerWeaponController.LeftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			OnHideWeapon?.Invoke(_playerWeaponController.LeftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
		}
		else if (weaponRanged.WeaponHandType == WeaponHandsEnum.Left && _playerWeaponController.RightHandWeapon != null)
		{
			OnHideWeapon?.Invoke(_playerWeaponController.RightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			OnHideWeapon?.Invoke(_playerWeaponController.RightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
		}
	}

	private void TurnOnReloadLayers()
	{
		Debug.Log($"turn ON reload layers");

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightArm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftArm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 1);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightArm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftArm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 1);
	}

	private void TurnOffReloadLayers()
	{
		Debug.Log($"turn OFF reload layers");

		if (_playerWeaponController.RightHandWeapon != null)
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 1);
		}
		if (_playerWeaponController.LeftHandWeapon != null)
		{
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 1);
			_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 1);
		}

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 0);

		if (_playerWeaponController.RightHandWeapon != null)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 1);
		}
		if (_playerWeaponController.LeftHandWeapon != null)
		{
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 1);
			_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 1);
		}

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 0);
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

	private void CancelWeaponHandAnimation(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.Right)
		{
			if (IsRightFullArmAttacking)
			{
				Debug.Log("Melee Right attack Canceled");

				IsRightFullArmAttacking = false;

				TurnOffFullArmLayer(handType);
			}
		}
		else
		{
			if (IsLeftFullArmAttacking)
			{
				Debug.Log("Melee Left attack Canceled");

				IsLeftFullArmAttacking = false;

				TurnOffFullArmLayer(handType);
			}
		}

		if (IsReloading)
		{
			StopCoroutine(_currentPlayerReloadingCoroutine);

			TurnOffReloadLayers();

			_currentPlayerReloadingCoroutine = null;

			IsReloading = false;
		}
	}

	private void CancelAllWeaponsAnimation()
	{
		CancelWeaponHandAnimation(WeaponHandsEnum.Right);
		CancelWeaponHandAnimation(WeaponHandsEnum.Left);

		TurnOffAllWeaponLayers();
	}

	private void TurnOffAllWeaponLayers()
	{
		Debug.Log($"turn OFF ALL weapon layers");

		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightArm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponRightPalm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftEquip, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftArm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponLeftPalm, 0);
		_playerAnimator1stPerson.SetLayerWeight(_layer1stWeaponReload, 0);

		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightArm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponRightPalm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftEquip, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftArm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponLeftPalm, 0);
		_playerAnimator3rdPerson.SetLayerWeight(_layer3rdWeaponReload, 0);
	}
}