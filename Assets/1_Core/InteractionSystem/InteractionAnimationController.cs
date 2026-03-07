using System.Collections;
using UnityEngine;

public class InteractionAnimationController : MonoBehaviour
{


	private InteractionController interactionController;




	private Animator playerAnimator;


	// Конструктор принимает зависимость
	public void Initialize(GameObject player, InteractionController interactionController)
	{

		playerAnimator = player.GetComponent<Animator>();
	

		this.interactionController = interactionController;


		this.interactionController.OnPickUpNonThrowable += PickUpBothHands;
		this.interactionController.OnPickUpThrowable += PickUpRightHand;
		this.interactionController.OnGetRidOfPickable += () =>
		{
			DropBothHands();
			DropRightHand();
		};



	
		Debug.Log("InteractionAnimationController Initialized");
	}





	private string currentPlayerWeaponRightAnimation = "";
	private string currentPlayerWeaponLeftAnimation = "";
	private string currentPlayerLegKickAttackAnimation = "";


	private void PickUpBothHands()
	{
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 1);
		ChangePlayerWeaponRightAnimation("EquipRightWeapon");

		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponLeft"), 1);
		ChangePlayerWeaponLeftAnimation("EquipLeftWeapon");
	}
	private void DropBothHands()
	{
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 0);
		ChangePlayerWeaponRightAnimation("UnequipRightWeapon");

		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponLeft"), 0);
		ChangePlayerWeaponLeftAnimation("UnequipLeftWeapon");
	}
	private void PickUpRightHand()
	{
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 1);
		ChangePlayerWeaponRightAnimation("EquipRightWeapon");
	}
	private void DropRightHand()
	{
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("WeaponRight"), 0);
		ChangePlayerWeaponRightAnimation("UnequipRightWeapon");
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



