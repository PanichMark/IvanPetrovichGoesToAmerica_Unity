using System.Collections;
using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
	private PlayerBehaviour playerBehaviour;

	private PlayerCameraController playerCameraController;
	private PlayerWeaponController weaponController;

	private Animator playerAnimator;
	private bool _isInitialized = false;
	private LegKickAttackController legKickAttack;
	public void Initialize(GameObject player, PlayerBehaviour playerBehaviour,
		PlayerCameraController playerCameraController, PlayerWeaponController weaponController, LegKickAttackController legKickAttack)
	{
		playerAnimator = player.GetComponent<Animator>();
		this.playerBehaviour = playerBehaviour;
	
		this.playerCameraController = playerCameraController;
		this.weaponController = weaponController;
		this.legKickAttack = legKickAttack;

		_isInitialized = true;
		Debug.Log("WeaponAnimationController Initialized");
	}

	private string currentPlayerWeaponRightAnimation = "";
	private string currentPlayerWeaponLeftAnimation = "";
	private string currentPlayerLegKickAttackAnimation = "";

	private bool wasPreviouslyKicking = false;

	private float adjustedCameraAngle;

	private void Update()
	{
		if (!_isInitialized)
			return;

		

		float cameraRotationX = playerCameraController.transform.rotation.eulerAngles.x;
		if (cameraRotationX >= 0 && cameraRotationX < 180)
		{
			adjustedCameraAngle = cameraRotationX;
		}
		else if (cameraRotationX < 360 && cameraRotationX > -180)
		{
			adjustedCameraAngle = cameraRotationX - 360;
		}

		if (playerBehaviour.IsPlayerArmed == true && playerCameraController.CurrentPlayerCameraStateType == "ThirdPerson")
		{
			float startValue = playerAnimator.GetFloat("UpDown");

			float endValue = adjustedCameraAngle * 0.0153846f;
	
			float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);

			playerAnimator.SetFloat("UpDown", newValue);
		}
		else
		{
			float startValue = playerAnimator.GetFloat("UpDown");
			
			float endValue = 0f;

			float newValue = Mathf.Lerp(startValue, endValue, Time.deltaTime * 6);

			playerAnimator.SetFloat("UpDown", newValue);
		}

		if (playerBehaviour.IsPlayerArmed == true && playerCameraController.CurrentPlayerCameraStateType == "FirstPerson")
		{
			playerAnimator.SetFloat("UpDown", 0);
		}

		if (weaponController.RightHandWeapon != null)
		{
			if (weaponController.rightHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 1);
				ChangePlayerWeaponRightAnimation("EquipRightWeapon");
			}
			else
			{
				ChangePlayerWeaponRightAnimation("UnequipRightWeapon");
				if (playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponRight")).IsName("UnequipRightWeapon") && playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponRight")).normalizedTime >= 0.99f)
				{
					playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponRightAnimation("UnequipRightWeapon");
			if (playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponRight")).IsName("UnequipRightWeapon") && playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponRight")).normalizedTime >= 0.99f)
			{
				playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 0);
			}
		}

		if (weaponController.LeftHandWeapon != null)
		{
			if (weaponController.leftHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
			{
				playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponLeft"), 1);
				ChangePlayerWeaponLeftAnimation("EquipLeftWeapon");
			}
			else
			{
				ChangePlayerWeaponLeftAnimation("UnequipLeftWeapon");

				if (playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponLeft")).IsName("UnequipLeftWeapon") && playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponLeft")).normalizedTime >= 0.99f)
				{
					playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponLeft"), 0);
				}
			}
		}
		else
		{
			ChangePlayerWeaponLeftAnimation("UnequipLeftWeapon");

			if (playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponLeft")).IsName("UnequipLeftWeapon") && playerAnimator.GetCurrentAnimatorStateInfo(playerAnimator.GetLayerIndex("WeaponLeft")).normalizedTime >= 0.99f)
			{
				playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponLeft"), 0);
			}
		}

		if (legKickAttack.IsPlayerLegKicking == true)
		{

			playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("LegKick"), 1);

			if (!wasPreviouslyKicking)
			{
				playerAnimator.Play("LegKick", 4, 0f); 
			}

			playerAnimator.Play("LegKick");
		}
		else if (legKickAttack.IsPlayerLegKicking == false)
		{
			playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("LegKick"), 0);
		}
		wasPreviouslyKicking = legKickAttack.IsPlayerLegKicking;
	}

	private void ChangePlayerWeaponRightAnimation(string animation, float crossfade = 0.2f)
	{
		if (currentPlayerWeaponRightAnimation != animation)
		{
			currentPlayerWeaponRightAnimation = animation;
			playerAnimator.CrossFade(animation, crossfade);
		}
	}

	private void ChangePlayerWeaponLeftAnimation(string animation, float crossfade = 0.2f)
	{
		if (currentPlayerWeaponLeftAnimation != animation)
		{
			currentPlayerWeaponLeftAnimation = animation;
			playerAnimator.CrossFade(animation, crossfade);
		}
	}

	private void ChangePlayerLegKickAttackAnimation(string animation, float crossfade = 0.2f)
	{
		if (currentPlayerLegKickAttackAnimation != animation)
		{
			currentPlayerLegKickAttackAnimation = animation;
			playerAnimator.CrossFade(animation, crossfade);
		}
	}
}