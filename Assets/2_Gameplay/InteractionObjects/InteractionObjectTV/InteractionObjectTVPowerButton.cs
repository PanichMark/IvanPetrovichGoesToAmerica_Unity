using UnityEngine;

public class InteractionObjectTVPowerButton : MonoBehaviour, IInteractable
{
	private string _interactionHintMessageAction;
	private LocalizationManager _localizationManager;
	private InteractionObjectTVController _tvController;

	public event IInteractable.InteractableObjectHandler OnInteract;

	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => $"{_interactionHintMessageAction}?";
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		// Находим контроллер (предполагается, что он на том же родителе)
		_tvController = transform.parent.GetComponent<InteractionObjectTVController>();

		// Подписываемся на событие изменения состояния ТВ в контроллере
		_tvController.OnTVStateChanged += UpdateHintAndState;

		// Устанавливаем начальную подсказку (ТВ по умолчанию выключен)
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void Interact()
	{
		// При нажатии просто просим контроллер переключить состояние
		_tvController.TogglePower();
	}

	public void InteractCutscene()
	{
		Interact();
	}

	// Этот метод вызывается контроллером при изменении состояния ТВ
	private void UpdateHintAndState(bool isOn)
	{
		if (isOn)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerOFF");
		}
		else
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_tvController.IsTVturnedOn) // Предполагается, что вы добавите публичное свойство в контроллер
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerOFF");
		}
		else
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");
		}
	}
}