using UnityEngine;

public class InteractionAnimationController : MonoBehaviour
{
	private InteractionController _interactionController;
	private Animator _playerAnimator;

	private int _layerPickableBothArms;
	private int _layerPickableRightArm;

	public void Initialize(
		InteractionController interactionController,
		GameObject player)
	{
		_playerAnimator = player.GetComponent<Animator>();
		_interactionController = interactionController;

		_layerPickableBothArms = _playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerPickableBothArms.ToString());
		_layerPickableRightArm = _playerAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerPickableRightArm.ToString());

		_interactionController.OnPickUpThrowable += PickUpWithRightHand;
		_interactionController.OnPickUpNonThrowable += PickUpWithBothHands;
		_interactionController.OnGetRidOfThrowable += DropWithRightHand;
		_interactionController.OnGetRidOfNonThrowable += DropWithBothHands;

		Debug.Log("InteractionAnimationController Initialized");
	}

	private void PickUpWithBothHands(InteractionObjectsPickableTypes pickableType)
	{
		_playerAnimator.SetLayerWeight(_layerPickableBothArms, 1f);
		_playerAnimator.Play(pickableType.ToString(), _layerPickableBothArms, 0f);
	}

	private void PickUpWithRightHand(InteractionObjectsPickableTypes pickableType)
	{
		_playerAnimator.SetLayerWeight(_layerPickableRightArm, 1f);
		_playerAnimator.Play(pickableType.ToString(), _layerPickableRightArm, 0f);
	}

	private void DropWithBothHands()
	{
		Debug.Log("DROP BOTH");
		_playerAnimator.SetLayerWeight(_layerPickableBothArms, 0f);
	}

	private void DropWithRightHand()
	{
		Debug.Log("DROP RIGHT");
		_playerAnimator.SetLayerWeight(_layerPickableRightArm, 0f);
	}
}