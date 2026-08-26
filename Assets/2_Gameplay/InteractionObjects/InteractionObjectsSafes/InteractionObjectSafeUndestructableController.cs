using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectSafeUndestructableController : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] private string _interactionObjectNameUI;

	public virtual string InteractionObjectNameSystem => null;
	public string InteractionHintMessageAction => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open")}";
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameUI)}";

	private LocalizationManager _localizationManager;

	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public event IInteractable.InteractableObjectHandler OnInteract;
	
	public virtual string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_WrongCombination")}!";
	public virtual bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	private bool _isAdditionalInteractionHintActive;
	protected bool _isSafeOpened;
	protected Collider _handleCollider;
	protected GameObject _safeDoor;
	protected Transform _safeDoorTransform;

	[SerializeField] private float _safeDoorOpeningSpeed;
	[SerializeField] private float _safeDoorOpenedRotation;
	[SerializeField] protected GameObject _safeRotatorySection1;
	[SerializeField] protected GameObject _safeRotatorySection2;
	[SerializeField] protected GameObject _safeRotatorySection3;
	protected InteractionObjectSafeRotationSection _section1;
	protected InteractionObjectSafeRotationSection _section2;
	protected InteractionObjectSafeRotationSection _section3;
	protected Quaternion _safeDoorOpenedPosition;



	protected virtual void InitializeSafe()
	{

	}

	void Start()
	{
		_handleCollider = GetComponent<Collider>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		
		_safeDoor = transform.parent.gameObject;
		_safeDoorTransform = _safeDoor.GetComponent<Transform>();



	
		_section1 = _safeRotatorySection1.GetComponent<InteractionObjectSafeRotationSection>();
		_section2 = _safeRotatorySection2.GetComponent<InteractionObjectSafeRotationSection>();
		_section3 = _safeRotatorySection3.GetComponent<InteractionObjectSafeRotationSection>();

		Vector3 openedEulerAngles = new Vector3(0, _safeDoorOpenedRotation, 0);
		_safeDoorOpenedPosition = Quaternion.Euler(openedEulerAngles);

		if (_isSafeOpened)
		{
			_safeDoorTransform.localRotation = _safeDoorOpenedPosition;
			_section1.SetSectionPositionToCorrect();
			_section2.SetSectionPositionToCorrect();
			_section3.SetSectionPositionToCorrect();
		}



		CheckRotatorySectionCorrection();

		InitializeSafe();
	}

	

	public void Interact()
	{
		if (!_isSafeOpened)
		{
			CheckRotatorySectionCorrection();
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	IEnumerator OpenSafeDoor()
	{
		gameObject.tag = "Untagged";

		_safeRotatorySection1.tag = "Untagged";
		_safeRotatorySection2.tag = "Untagged";
		_safeRotatorySection3.tag = "Untagged";

		while (Quaternion.Angle(_safeDoorTransform.localRotation, _safeDoorOpenedPosition) > 0.1f)
		{
			_safeDoorTransform.localRotation = Quaternion.RotateTowards(
				_safeDoorTransform.localRotation,
				_safeDoorOpenedPosition,
				Time.deltaTime * _safeDoorOpeningSpeed);
			yield return null;
		}
	}

	private void CheckRotatorySectionCorrection()
	{
		if (_section1.currentSectionPosition == _section1.CorrectSectionPosition)
			_section1.SetSectionPositionToCorrect();
		if (_section2.currentSectionPosition == _section2.CorrectSectionPosition)
			_section2.SetSectionPositionToCorrect();
		if (_section3.currentSectionPosition == _section3.CorrectSectionPosition)
			_section3.SetSectionPositionToCorrect();

		if (_section1.IsSectionPositionCorrect && _section2.IsSectionPositionCorrect && _section3.IsSectionPositionCorrect)
		{
			_isAdditionalInteractionHintActive = false;
			_isSafeOpened = true;

			StartCoroutine(OpenSafeDoor());
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}

		if (_isSafeOpened)
		{
			float yAngle = _section1.CorrectSectionPosition != 0 ? 360f / 10 * _section1.CorrectSectionPosition : 0f;
			Vector3 openedEulerAngles = new Vector3(0, yAngle, 0);
			var sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section1.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = _section2.CorrectSectionPosition != 0 ? 360f / 10 * _section2.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section2.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = _section3.CorrectSectionPosition != 0 ? 360f / 10 * _section3.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section3.transform.localRotation = sectionCorrectPositionRotation;
		}
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.SafesUndestructableData == null)
		{
			data.SafesUndestructableData = new Dictionary<GameScenesGameplayDataEnum, List<SafeUndestructableData>>();
		}
		if (!data.SafesUndestructableData.ContainsKey(currentScene))
		{
			data.SafesUndestructableData[currentScene] = new List<SafeUndestructableData>();
		}

		var targetList = data.SafesUndestructableData[currentScene];

		int indexInList = targetList.FindIndex(item => item.SafeUndestructableIndex == GameplayObjectIndex);

		var updatedItem = new SafeUndestructableData
		{
			SafeUndestructableIndex = GameplayObjectIndex,
			SafeUndestructableNameSystem = InteractionObjectNameSystem,
			IsSafeUndestructableOpened = _isSafeOpened,
			SafeUndestructableRotationSection_1_Position = _section1.currentSectionPosition,
			SafeUndestructableRotationSection_2_Position = _section2.currentSectionPosition,
			SafeUndestructableRotationSection_3_Position = _section3.currentSectionPosition
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.SafesUndestructableData == null || !data.SafesUndestructableData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.SafeUndestructableIndex == GameplayObjectIndex);

		if (savedState.Equals(default(SafeUndestructableData))) yield break;

		_isSafeOpened = savedState.IsSafeUndestructableOpened;

		_section1.SetLoadedPosition(savedState.SafeUndestructableRotationSection_1_Position);
		_section2.SetLoadedPosition(savedState.SafeUndestructableRotationSection_2_Position);
		_section3.SetLoadedPosition(savedState.SafeUndestructableRotationSection_3_Position);

		if (_isSafeOpened)
		{
			gameObject.tag = "Untagged";
			_safeRotatorySection1.tag = "Untagged";
			_safeRotatorySection2.tag = "Untagged";
			_safeRotatorySection3.tag = "Untagged";

			_safeDoorTransform.localRotation = _safeDoorOpenedPosition;
		}

		yield return null;
	}
}