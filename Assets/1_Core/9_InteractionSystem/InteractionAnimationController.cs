using UnityEngine;

public class InteractionAnimationController : MonoBehaviour
{
	private InteractionController _interactionController;
	private Animator _playerAnimator;

	private string _currentPlayerRightHandWeaponAnimation = "";
	private string _currentPlayerLeftHandWeaponAnimation = "";
	private string _currentPlayerLegKickAttackAnimation = "";
	private GameObject _gameObjectSpineSlot;
	public void Initialize(
		InteractionController interactionController,
		GameObject player,
		GameObject gameObjectSpineSlot)
	{
		_playerAnimator = player.GetComponent<Animator>();
		_interactionController = interactionController;
		_gameObjectSpineSlot = gameObjectSpineSlot;
		//_interactionController.OnPickUpNonThrowable += PickUpWithBothHands;
		//_interactionController.OnPickUpThrowable += PickUpWithRightHand;
		_interactionController.OnGetRidOfNonThrowable += () =>
		{
			//DropBothWithHands();
			//DropWithRightHand();
		};

		Debug.Log("InteractionAnimationController Initialized");
	}

}