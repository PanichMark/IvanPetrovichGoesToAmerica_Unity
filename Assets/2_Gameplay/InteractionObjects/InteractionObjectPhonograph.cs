using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractionObjectPhonograph : MonoBehaviour, IInteractable
{
	[SerializeField] private string _interactionObjectNameSystem;
	private LocalizationManager _localizationManager;
	[SerializeField] private AudioClip _audioClip;
	private AudioSource _audioSource;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;

	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";

	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	private bool _isTurnedOn;
	private string _interactionHintMessageAction;

	public string InteractionHintMessageAction => _interactionHintMessageAction;

	public string InteractionHintMessageFail => null;

	public bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if(_isTurnedOn)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_isTurnedOn)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
	}

	public void Interact()
	{
		if (_isTurnedOn)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
			_isTurnedOn = false;

			if (_audioSource != null)
			{
				_audioSource.Stop();
			}
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
			_isTurnedOn = true;

			if (_audioSource != null && _audioClip != null)
			{
				_audioSource.clip = _audioClip;
				_audioSource.Stop();
				_audioSource.Play();
			}
		}
	}

	public void InteractCutscene()
	{
		throw new System.NotImplementedException();
	}
}
