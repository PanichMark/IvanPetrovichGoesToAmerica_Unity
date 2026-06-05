using System;
using UnityEngine;

public class InteractionAnimationController : MonoBehaviour
{
	private InteractionController _interactionController;
	private Animator _playerAnimator;

	private string _currentPlayerRightHandWeaponAnimation = "";
	private string _currentPlayerLeftHandWeaponAnimation = "";
	private string _currentPlayerLegKickAttackAnimation = "";

	public void Initialize(GameObject player, InteractionController interactionController)
	{
		_playerAnimator = player.GetComponent<Animator>();
		_interactionController = interactionController;

		_interactionController.OnPickUpNonThrowable += PickUpWithBothHands;
		_interactionController.OnPickUpThrowable += PickUpWithRightHand;
		_interactionController.OnGetRidOfNonThrowable += () =>
		{
			DropBothWithHands();
			DropWithRightHand();
		};

		Debug.Log("InteractionAnimationController Initialized");
	}

	private void PickUpWithBothHands()
	{
		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 1);
		ChangePlayerRightHandWeaponAnimation("EquipRightWeapon");

		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponLeft"), 1);
		ChangePlayerLeftHandWeaponAnimation("EquipLeftWeapon");
	}

	private void DropBothWithHands()
	{
		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 0);
		ChangePlayerRightHandWeaponAnimation("UnequipRightWeapon");

		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponLeft"), 0);
		ChangePlayerLeftHandWeaponAnimation("UnequipLeftWeapon");
	}

	private void PickUpWithRightHand()
	{
		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 1);
		ChangePlayerRightHandWeaponAnimation("EquipRightWeapon");
	}

	private void DropWithRightHand()
	{
		_playerAnimator.SetLayerWeight(_playerAnimator.GetLayerIndex("WeaponRight"), 0);
		ChangePlayerRightHandWeaponAnimation("UnequipRightWeapon");
	}

	private void ChangePlayerRightHandWeaponAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerRightHandWeaponAnimation != animation)
		{
			_currentPlayerRightHandWeaponAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}

	private void ChangePlayerLeftHandWeaponAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerLeftHandWeaponAnimation != animation)
		{
			_currentPlayerLeftHandWeaponAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}

	////??????? idk
	private void ChangePlayerLegKickAttackAnimation(string animation, float crossfade = 0.2f)
	{
		if (_currentPlayerLegKickAttackAnimation != animation)
		{
			_currentPlayerLegKickAttackAnimation = animation;
			_playerAnimator.CrossFade(animation, crossfade);
		}
	}
}