using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectElectricalPanel : GameplayObjectJsonSaveLoad, IInteractable, IElectroShockable
{
	public delegate void ElectricalPanelHandler();
	public event ElectricalPanelHandler OnWentOutOfService;

	public string InteractionObjectNameSystem => "InteractionObject_ElectricalPanel";

	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem, gameObject.name)}";

	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	private LocalizationManager _localizationManager;
	public string InteractionHintMessageAction => $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Interact", gameObject.name)}";

	public bool IsOutOfService {  get; private set; }
	private string _interactionHintMessageFail;
	public string InteractionHintMessageFail => $"{_interactionHintMessageFail}!";
	private float _health;
	public bool IsInteractionHintMessageFailActive => true;

	public event IInteractable.InteractableObjectHandler OnInteract;

	private PlayerHealthController _playerResourcesHealthManager;

	private void Start()
	{
	_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerHealthController>();

		if (IsOutOfService)
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name);
		}
		else
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_ElectroShock", gameObject.name);
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void Interact()
	{
		if (!IsOutOfService)
		{
			_playerResourcesHealthManager.TakeDamage(5);
		}
	}

	public void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}

	public void Electrify(float damage)
	{
		if (!IsOutOfService)
		{
			_health -= damage;

			if (_health <= 0)
			{
				_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name);

				IsOutOfService = true;
				OnWentOutOfService?.Invoke();
			}
		}
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		if (IsOutOfService)
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name);
		}
		else
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_ElectroShock", gameObject.name);
		}
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.ElectricalPanelsData == null)
		{
			data.ElectricalPanelsData = new Dictionary<GameScenesGameplayDataEnum, List<ElectricalPanelData>>();
		}
		if (!data.ElectricalPanelsData.ContainsKey(currentScene))
		{
			data.ElectricalPanelsData[currentScene] = new List<ElectricalPanelData>();
		}

		var targetList = data.ElectricalPanelsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.ElectricalPanelIndex == GameplayObjectIndex);

		var updatedItem = new ElectricalPanelData
		{
			ElectricalPanelIndex = GameplayObjectIndex,
			ElectricalPanelSystem = InteractionObjectNameSystem,
			IsElectricalPanelOutOfService = IsOutOfService
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

		if (data.ElectricalPanelsData == null || !data.ElectricalPanelsData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			ElectricalPanelData savedState = sourceList.Find(item => item.ElectricalPanelIndex == GameplayObjectIndex);

			if (savedState.ElectricalPanelIndex != 0)
			{
				IsOutOfService = savedState.IsElectricalPanelOutOfService;

				if (IsOutOfService)
				{
					_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name);
				}
				else
				{
					_interactionHintMessageFail = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_ElectroShock", gameObject.name);
				}
			}
		}

		yield return null;
	}
}
