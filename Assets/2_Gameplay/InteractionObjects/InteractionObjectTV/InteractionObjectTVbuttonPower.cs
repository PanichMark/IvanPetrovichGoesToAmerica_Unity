using UnityEngine;

public class InteractionObjectTVbuttonPower : MonoBehaviour, IInteractable
{
	private string _interactionHintMessageAction;
	private LocalizationManager _localizationManager;
	private InteractionObjectTVcontroller _tvController;

	public event IInteractable.InteractableObjectHandler OnInteract;

	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => $"{_interactionHintMessageAction}?";
	public string InteractionHintMessageAction => _interactionHintMessageAction;
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		// Находим контроллер (предполагается, что он на том же родителе)
		_tvController = transform.parent.GetComponent<InteractionObjectTVcontroller>();

		// Подписываемся на событие изменения состояния ТВ в контроллере
		_tvController.OnTVstateChanged += UpdateHintAndState;

		// Устанавливаем начальную подсказку (ТВ по умолчанию выключен)
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON", gameObject.name);

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
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TV_PowerOf", gameObject.name);
		}
		else
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TV_PowerOn", gameObject.name);
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_tvController.IsTVturnedOn) // Предполагается, что вы добавите публичное свойство в контроллер
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TV_Off", gameObject.name);
		}
		else
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TV_PowerONn", gameObject.name);
		}
	}
}