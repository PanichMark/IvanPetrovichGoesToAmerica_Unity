using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class InteractionObjectPhonograph : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] private string _interactionObjectNameSystem;
	private LocalizationManager _localizationManager;
	[SerializeField] private AudioClip _audioClip;
	private AudioSource _audioSource;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;

	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem, gameObject.name)}";

	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	private bool _isTurnedOn;
	private string _interactionHintMessageAction;

	public string InteractionHintMessageAction => _interactionHintMessageAction;

	public string InteractionHintMessageFail => null;

	public bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
	_audioSource = GetComponent<AudioSource>();
	_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if(_isTurnedOn)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOff", gameObject.name)}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOn", gameObject.name)}";
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_isTurnedOn)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOff", gameObject.name)}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOn", gameObject.name)}";
		}
	}

	public void Interact()
	{
		if (_isTurnedOn)
		{
			TurnOff();


		}
		else
		{

			TurnOn();
		}
	}

	private void TurnOn()
	{
		_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOff", gameObject.name)}";
		_isTurnedOn = true;


		_audioSource.clip = _audioClip;
		_audioSource.Stop();
		_audioSource.Play();
	}

	private void TurnOff()
	{
		_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOn", gameObject.name)}";
		_isTurnedOn = false;


		_audioSource.Stop();
	}

	public void InteractCutscene()
	{
		throw new System.NotImplementedException();
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.PhonographsData == null)
		{
			data.PhonographsData = new Dictionary<GameScenesGameplayDataEnum, List<PhonographData>>();
		}
		if (!data.PhonographsData.ContainsKey(currentScene))
		{
			data.PhonographsData[currentScene] = new List<PhonographData>();
		}

		var targetList = data.PhonographsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.PhonographIndex == GameplayObjectIndex);

		var updatedItem = new PhonographData
		{
			PhonographIndex = GameplayObjectIndex,
			PhonographNameSystem = _interactionObjectNameSystem,
			IsPhonographTurnedOn = _isTurnedOn
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

		if (data.PhonographsData == null || !data.PhonographsData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			PhonographData savedState = sourceList.Find(item => item.PhonographIndex == GameplayObjectIndex);

			if (savedState.PhonographIndex != 0)
			{
				_isTurnedOn = savedState.IsPhonographTurnedOn;

	
				if (_isTurnedOn)
				{
					TurnOn();
				}

				
			}
		}

		yield return null;
	}
}