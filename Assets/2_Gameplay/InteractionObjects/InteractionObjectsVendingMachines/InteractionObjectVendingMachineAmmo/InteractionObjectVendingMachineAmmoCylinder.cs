using UnityEngine;
using System.Collections;

public class InteractionObjectVendingMachineAmmoCylinder : MonoBehaviour, IInteractable
{
	private InteractionObjectVendingMachineAmmo _vendingMachine;
	[SerializeField] private float _rotationDuration = 1f;
	[SerializeField] private float _rotationAngle = 90f;
	private bool _isOutOfService;
	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => $"{_localizationManager.GetLocalizedString("InteractionObject_VendingMachine_Ammo_Cylinder", gameObject.name)}?";
	public string InteractionHintMessageAction => null;
	public event IInteractable.InteractableObjectHandler OnInteract;


	public string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;
	private bool _isBusy = false;
	private LocalizationManager _localizationManager;
	public bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;

	private bool _isInteractionHintMessageFailActive;

	private void Start()
	{
		_vendingMachine = GetComponentInParent<InteractionObjectVendingMachineAmmo>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_localizationManager.OnLanguageChanged += ChangeLangauge;
		_vendingMachine.OnWentOutOfService += WentOutOFService;

		if (!_isOutOfService)
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_Wait", gameObject.name)}!";
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name)}!";
		}
	}

	public void ChangeLangauge(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (!_isOutOfService)
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_Wait", gameObject.name)}!";
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name)}!";
		}
	}

	private void WentOutOFService()
	{
		_isInteractionHintMessageFailActive = true;
		_isOutOfService = true;
		_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_OutOfService", gameObject.name)}!";
	}

	private void OnDestroy()
	{
		_vendingMachine.OnWentOutOfService -= WentOutOFService;
	}

	public void Interact()
	{
		if (!_isOutOfService)
		{
			if (!_isBusy)
			{
				_vendingMachine.SetCurrentAmmoType(_vendingMachine.currentAmmoIndex + 1);
				StartCoroutine(RotateAndChangeAmmo());
				_isInteractionHintMessageFailActive = false;
				_isBusy = true;
			}
			else
			{
				_isInteractionHintMessageFailActive = true;
			}
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private IEnumerator RotateAndChangeAmmo()
	{
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(Vector3.right * _rotationAngle);

		float elapsedTime = 0f;

		while (elapsedTime < _rotationDuration)
		{
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / _rotationDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.rotation = endRotation;

		Debug.Log($"Selected ammo type: {_vendingMachine.ammoTypes[_vendingMachine.currentAmmoIndex].AmmoName}");

		_isInteractionHintMessageFailActive = false;
		_isBusy = false;
	}
}