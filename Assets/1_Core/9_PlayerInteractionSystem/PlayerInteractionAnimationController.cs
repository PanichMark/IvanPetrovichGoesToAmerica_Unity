using UnityEngine;
using System.Collections;

public class PlayerInteractionAnimationController : MonoBehaviour
{
	private PlayerInteractionController _interactionController;
	private Animator _playerAnimator3rdPerson;
	private Animator _playerAnimator1stPerson;

	private int _layerPickableBothArms3rd;
	private int _layerPickableRightArm3rd;
	private int _layerPickableRightArm1st;

	public void Initialize(
		PlayerInteractionController interactionController,
		GameObject player,
		GameObject playerCamera)
	{
		_playerAnimator3rdPerson = player.GetComponent<Animator>();
		_playerAnimator1stPerson = playerCamera.GetComponent<Animator>();
		_interactionController = interactionController;

		_layerPickableBothArms3rd = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerPickableBothArms.ToString());
		_layerPickableRightArm3rd = _playerAnimator3rdPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerPickableRightArm.ToString());
		_layerPickableRightArm1st = _playerAnimator1stPerson.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerPickableRightArm.ToString());

		_interactionController.OnPickUpThrowable += PickUpWithRightHand;
		_interactionController.OnPickUpNonThrowable += PickUpWithBothHands;
		_interactionController.OnGetRidOfThrowable += DropWithRightHand;
		_interactionController.OnGetRidOfNonThrowable += DropWithBothHands;

		_interactionController.OnThrowTrowable += ThrowWithRightHand;


		Debug.Log("InteractionAnimationController Initialized");
	}

	private void PickUpWithBothHands(InteractionObjectsPickableTypes pickableType)
	{
		_playerAnimator3rdPerson.SetLayerWeight(_layerPickableBothArms3rd, 1f);
		_playerAnimator3rdPerson.Play($"{pickableType.ToString()}_Hold", _layerPickableBothArms3rd, 0f);
	}

	private void PickUpWithRightHand(InteractionObjectsPickableTypes pickableType)
	{
		_playerAnimator3rdPerson.SetLayerWeight(_layerPickableRightArm3rd, 1f);
		_playerAnimator3rdPerson.Play($"{pickableType.ToString()}_Hold", _layerPickableRightArm3rd, 0f);

		_playerAnimator1stPerson.SetLayerWeight(_layerPickableRightArm1st, 1f);
		_playerAnimator1stPerson.Play($"{pickableType.ToString()}_Hold", _layerPickableRightArm1st, 0f);
	}

	private void DropWithBothHands()
	{
		Debug.Log("DROP BOTH");
		_playerAnimator3rdPerson.SetLayerWeight(_layerPickableBothArms3rd, 0f);
	}

	private void DropWithRightHand()
	{
		Debug.Log("DROP RIGHT");
		_playerAnimator3rdPerson.SetLayerWeight(_layerPickableRightArm3rd, 0f);

		_playerAnimator1stPerson.SetLayerWeight(_layerPickableRightArm1st, 0f);
	}

	private void ThrowWithRightHand(InteractionObjectsPickableTypes throwableType)
	{
		StartCoroutine(ThrowWithRightHandCoroutine());

		_playerAnimator3rdPerson.Play($"{throwableType.ToString()}_Throw", _layerPickableRightArm3rd, 0f);

		_playerAnimator1stPerson.Play($"{throwableType.ToString()}_Throw", _layerPickableRightArm1st, 0f);
	}

	private IEnumerator ThrowWithRightHandCoroutine()
	{
		yield return new WaitForSeconds(0.4f);

		_interactionController.EarlyThrowThrowable();

		yield return new WaitForSeconds(0.35f);

		_interactionController.LateThrowThrowable();
	}
}