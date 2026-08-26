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
	private InteractionObjectSafeFallSensor _interactionObjectSafeFallSensor;
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public event IInteractable.InteractableObjectHandler OnInteract;
	private bool _isSafeBroken;
	public virtual string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_WrongCombination")}!";
	public virtual bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	private bool _isAdditionalInteractionHintActive;
	private bool _isSafeOpened;
	private Collider _handleCollider;
	private GameObject _safeDoor;
	private Transform _safeDoorTransform;

	[SerializeField] private float _safeDoorOpeningSpeed;
	[SerializeField] private float _safeDoorOpenedRotation;
	[SerializeField] private GameObject _safeRotatorySection1;
	[SerializeField] private GameObject _safeRotatorySection2;
	[SerializeField] private GameObject _safeRotatorySection3;
	private InteractionObjectSafeRotationSection _section1;
	private InteractionObjectSafeRotationSection _section2;
	private InteractionObjectSafeRotationSection _section3;
	private Quaternion _safeDoorOpenedPosition;

	private GameObject _safeBody;
	private Rigidbody _safeBodyRb;

	public bool IsFalling {  get; private set; }
	private float _fallStartTime;
	[SerializeField] private float _fallSpeedThreshold;
	[SerializeField] private float _fallDurationLimit;
	[SerializeField] private float _impactForceMultiplier;

	void Start()
	{
		_handleCollider = GetComponent<Collider>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		
		_safeDoor = transform.parent.gameObject;
		_safeDoorTransform = _safeDoor.GetComponent<Transform>();

		_safeBody = _safeDoor.transform.parent.gameObject;
		_safeBodyRb = _safeBody.GetComponent<Rigidbody>();
		_interactionObjectSafeFallSensor = _safeBody.GetComponent<InteractionObjectSafeFallSensor>();

		_interactionObjectSafeFallSensor.Initialize(this, _safeDoor);
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
	}

	void Update()
	{
		if (!_isSafeBroken)
		{
			CheckForFall();
		}
	}

	private void CheckForFall()
	{
		if (_safeBodyRb == null) return;

		float verticalVelocity = _safeBodyRb.linearVelocity.y;

		if (IsFalling)
		{
			// Сбрасываем флаг, если сейф остановился или подпрыгнул (ударился), но не сломался сразу
			if (verticalVelocity >= -0.1f || verticalVelocity > 0f)
			{
				IsFalling = false;
			}
		}
		// Начинаем отсчет времени ТОЛЬКО если скорость превысила порог вниз
		else if (verticalVelocity < -_fallSpeedThreshold)
		{
			IsFalling = true;
			_fallStartTime = Time.time;
		}
	}

	// Добавьте этот НОВЫЙ МЕТОД в класс InteractionObjectSafeController
	public void OnSafeBodyCollided(Collision collision)
	{
		//Debug.Log("SAFE BODY COLLIDED! " + collision.collider.name);

		// Ваша логика проверки времени падения остается здесь
		if (IsFalling && (Time.time - _fallStartTime) >= _fallDurationLimit)
		{
			BreakSafeFromImpact();
		}

		// Сбрасываем флаг в любом случае
		IsFalling = false;
	}

	private void BreakSafeFromImpact()
	{
		_isSafeBroken = true;
		_handleCollider.enabled = false;
		_safeDoor.transform.SetParent(null);
		_safeDoor.tag = "Interactable";
		Rigidbody doorRigidbody = _safeDoor.AddComponent<Rigidbody>();
		doorRigidbody.AddForce(transform.forward * _impactForceMultiplier, ForceMode.Impulse);
		_safeRotatorySection1.tag = "Untagged";
		_safeRotatorySection2.tag = "Untagged";
		_safeRotatorySection3.tag = "Untagged";
		gameObject.tag = "Untagged";
		Debug.Log("SAFE BROKEN!!!!");

		enabled = false;
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

		if (data.SafesDestructableData == null)
		{
			data.SafesDestructableData = new Dictionary<GameScenesGameplayDataEnum, List<SafeDestructableData>>();
		}
		if (!data.SafesDestructableData.ContainsKey(currentScene))
		{
			data.SafesDestructableData[currentScene] = new List<SafeDestructableData>();
		}

		var targetList = data.SafesDestructableData[currentScene];

		int indexInList = targetList.FindIndex(item => item.SafeDestructableIndex == GameplayObjectIndex);

		var updatedItem = new SafeDestructableData
		{
			SafeDestructableIndex = GameplayObjectIndex,
			SafeDestructableNameSystem = InteractionObjectNameSystem,
			IsSafeDestructableOpened = _isSafeOpened,
			IsSafeDestructableDestroyed = _isSafeBroken,
			SafeDestructableRotationSection_1_Position = _section1.currentSectionPosition,
			SafeDestructableRotationSection_2_Position = _section2.currentSectionPosition,
			SafeDestructableRotationSection_3_Position = _section3.currentSectionPosition
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

		if (data.SafesDestructableData == null || !data.SafesDestructableData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.SafeDestructableIndex == GameplayObjectIndex);

		if (savedState.Equals(default(SafeDestructableData))) yield break;

		_isSafeOpened = savedState.IsSafeDestructableOpened;
		_isSafeBroken = savedState.IsSafeDestructableDestroyed;

		_section1.SetLoadedPosition(savedState.SafeDestructableRotationSection_1_Position);
		_section2.SetLoadedPosition(savedState.SafeDestructableRotationSection_2_Position);
		_section3.SetLoadedPosition(savedState.SafeDestructableRotationSection_3_Position);

		if (_isSafeOpened)
		{
			gameObject.tag = "Untagged";
			_safeRotatorySection1.tag = "Untagged";
			_safeRotatorySection2.tag = "Untagged";
			_safeRotatorySection3.tag = "Untagged";

			if (!_isSafeBroken)
			{
				_safeDoorTransform.localRotation = _safeDoorOpenedPosition;
			}
		}

		if (_isSafeBroken)
		{
			_handleCollider.enabled = false;
			_safeDoor.transform.SetParent(null);
			_safeDoor.tag = "Interactable";
			Rigidbody doorRigidbody = _safeDoor.AddComponent<Rigidbody>();
			gameObject.tag = "Untagged";
			_safeRotatorySection1.tag = "Untagged";
			_safeRotatorySection2.tag = "Untagged";
			_safeRotatorySection3.tag = "Untagged";
		}

		yield return null;
	}
}