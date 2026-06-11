using UnityEngine;
using System.Collections.Generic;
using UnityEditor.XR;

public class InteractionObjectLightSwitchButton : MonoBehaviour, IInteractable, IElectroShockable
{
	private InteractionObjectLightSwitchController _lightSwitchController;

	[SerializeField] private string _interactionObjectNameSystem;
	[SerializeField] private bool _isThisTurnOnButton = true;
	private LocalizationManager _localizationManager;
	private bool _isLightTurnedOn;
	private List<Material> _lightMaterialsList = new List<Material>();
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;

	private string _interactionHintMessageAction;

	void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_lightSwitchController = GetComponentInParent<InteractionObjectLightSwitchController>();

		_lightMaterialsList.Clear();

		foreach (var obj in _lightSwitchController.LightObjectsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				_lightMaterialsList.Add(renderer.material);
			}
		}
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		if (_isThisTurnOnButton)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
	}

	// ... (другие части класса остаются без изменений)

	// Этот метод теперь только решает, что вызвать: TurnOn или TurnOff
	public void Interact()
	{
		if (_isThisTurnOnButton)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	// Метод для включения света
	private void TurnOn()
	{
		_isLightTurnedOn = true;

		for (int i = 0; i < _lightMaterialsList.Count; i++)
		{
			// Проверка на null, чтобы избежать ошибок, если материал не назначен
			if (_lightMaterialsList[i] == null) continue;

			// Устанавливаем цвет свечения из контроллера
			_lightMaterialsList[i].SetColor("_EmissionColor", _lightSwitchController.LightEmissionColor);

			// Принудительно обновляем состояние эмиссии (включаем её)
			_lightMaterialsList[i].DisableKeyword("_EMISSION");
			_lightMaterialsList[i].EnableKeyword("_EMISSION");

			// Устанавливаем флаг для работы с глобальным освещением в реальном времени
			_lightMaterialsList[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		}
	}

	// Метод для выключения света
	private void TurnOff()
	{
		_isLightTurnedOn = false;

		for (int i = 0; i < _lightMaterialsList.Count; i++)
		{
			// Проверка на null
			if (_lightMaterialsList[i] == null) continue;

			// Выключаем свечение, устанавливая черный цвет
			_lightMaterialsList[i].SetColor("_EmissionColor", Color.black);

			// Принудительно обновляем состояние эмиссии (выключаем её)
			_lightMaterialsList[i].DisableKeyword("_EMISSION");
			_lightMaterialsList[i].EnableKeyword("_EMISSION");

			// Флаг оставляем, чтобы при включении не было артефактов
			_lightMaterialsList[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		}
	}
	// ...

	public void InteractCutscene()
	{
		Interact();
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_isThisTurnOnButton)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
	}

	public void Electrify(float damage)
	{
		if (!_isLightTurnedOn)
		{
			TurnOn();
		}
	}
}