using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectOpenableSliding : InteractionObjectOpenableAbstract
{
	[SerializeField] protected float _openingSpeed;
	protected Coroutine _currentAnimation;

	[SerializeField] private Vector3 _intermediatePositionOffset;
	[SerializeField] private Vector3 _openedPositionOffset;
	
	private Vector3 _closedPosition;
	private Vector3 _intermediatePosition;
	private Vector3 _openedPosition;

	public override bool IsInteractionHintMessageFailActive => false;
	public override string InteractionHintMessageFail => null;

	protected string _interactionHintMessageMain;
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	public void Start()
	{
		_isObjectOpened = false;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open", gameObject.name);
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

		UpdatePositions();

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void UpdatePositions()
	{
		_closedPosition = transform.localPosition;
		_intermediatePosition = _closedPosition + _intermediatePositionOffset;
		_openedPosition = _intermediatePosition + _openedPositionOffset;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open", gameObject.name);
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close", gameObject.name);
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	public override void Interact()
	{
		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}


		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close", gameObject.name);
			_currentAnimation = StartCoroutine(OpenSequence());
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open", gameObject.name);
			_currentAnimation = StartCoroutine(CloseSequence());
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

	}

	public override void InteractCutscene()
	{
		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close", gameObject.name);
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open", gameObject.name);
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	private IEnumerator OpenSequence()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		_isObjectOpened = true;

		if (_intermediatePositionOffset != Vector3.zero)
		{
			while (Vector3.Distance(transform.localPosition, _intermediatePosition) > 0.001f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePosition, Time.deltaTime * _openingSpeed);
				yield return null;
			}
		}

		while (Vector3.Distance(transform.localPosition, _openedPosition) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}
		
		_currentAnimation = null;
	}

	private IEnumerator CloseSequence()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		_isObjectOpened = false;

		if (_intermediatePositionOffset != Vector3.zero)
		{
			while (Vector3.Distance(transform.localPosition, _intermediatePosition) > 0.001f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePosition, Time.deltaTime * _openingSpeed);
				yield return null;
			}
		}
		
		while (Vector3.Distance(transform.localPosition, _closedPosition) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null)
		{
			data.OpenableUndestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<OpenableUndestructableObjectData>>();
		}
		if (!data.OpenableUndestructableObjectsData.ContainsKey(currentScene))
		{
			data.OpenableUndestructableObjectsData[currentScene] = new List<OpenableUndestructableObjectData>();
		}

		var targetList = data.OpenableUndestructableObjectsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.OpenableUndestructableObjectIndex == GameplayObjectIndex);

		if (indexInList != -1)
		{
			var existingItem = targetList[indexInList];

			existingItem.IsOpenableUndestructableObjectUnlocked = IsOpenableUnlocked;
			existingItem.IsOpenableUndestructableObjectOpened = _isObjectOpened;
			existingItem.OpenableUndestructableObjectNameSystem = InteractionObjectNameSystem;

			targetList[indexInList] = existingItem;
		}
		else
		{
			targetList.Add(new OpenableUndestructableObjectData
			{
				OpenableUndestructableObjectIndex = GameplayObjectIndex,
				OpenableUndestructableObjectNameSystem = InteractionObjectNameSystem,
				IsOpenableUndestructableObjectUnlocked = IsOpenableUnlocked,
				IsOpenableUndestructableObjectOpened = _isObjectOpened
			});
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null || !data.OpenableUndestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.OpenableUndestructableObjectIndex == GameplayObjectIndex);

		if (savedState.Equals(default(OpenableUndestructableObjectData))) yield break;

		IsOpenableUnlocked = savedState.IsOpenableUndestructableObjectUnlocked;
		_isObjectOpened = savedState.IsOpenableUndestructableObjectOpened;

		yield return null;
	}
}