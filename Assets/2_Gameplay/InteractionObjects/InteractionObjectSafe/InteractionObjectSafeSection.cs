using UnityEngine;
using System.Collections;

public class InteractionObjectSafeSection : MonoBehaviour, IInteractable
{
	[SerializeField] private string _interactionObjectNameUI;
	[SerializeField] private int _safeSectionSlotNumber;
	[SerializeField][Range(0, 9)] private int _correctSectionPosition;

	
	public int CorrectSectionPosition => _correctSectionPosition;
	public bool IsSectionPositionCorrect { get; private set; }
	public int currentSectionPosition { get; private set; }
	private LocalizationManager _localizationManager;
	private float _sectionRotationSpeed = 0.15f;
	private Coroutine _sectionCoroutine;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionHintMessageAction => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Rotate")}";
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameUI)} #{_safeSectionSlotNumber.ToString()}";
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI} ?";
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionObjectNameSystem => null;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
	}

	public void Interact()
	{
		if (_sectionCoroutine == null)
		{
			_sectionCoroutine = StartCoroutine(RotateSmoothly(_sectionRotationSpeed));
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	IEnumerator RotateSmoothly(float duration)
	{
		currentSectionPosition = currentSectionPosition < 9 ? currentSectionPosition + 1 : 0;

		Quaternion rotateFrom = transform.localRotation;
		Quaternion rotateTo = transform.localRotation * Quaternion.Euler(0, 36, 0);

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			transform.localRotation = Quaternion.Slerp(rotateFrom, rotateTo, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Debug.Log($"Section #{InteractionObjectNameUI} new position is {currentSectionPosition}");

		IsSectionPositionCorrect = currentSectionPosition == _correctSectionPosition;

		if (IsSectionPositionCorrect)
			Debug.Log($"Section #{InteractionObjectNameUI} CORRECT");

		transform.localRotation = rotateTo;
		_sectionCoroutine = null;
	}

	public void SetSectionPositionToCorrect()
	{
		IsSectionPositionCorrect = true;
	}
}