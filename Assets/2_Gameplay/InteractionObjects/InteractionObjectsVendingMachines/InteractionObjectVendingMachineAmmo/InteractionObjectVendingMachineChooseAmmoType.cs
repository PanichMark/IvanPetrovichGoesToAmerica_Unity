using UnityEngine;
using System.Collections;

public class InteractionObjectVendingMachineChooseAmmoType : MonoBehaviour, IInteractable
{
	public delegate void AmmoTypeChangedHandler();

	private InteractionObjectVendingMachineAmmo _vendingMachine;
	[SerializeField] private float _rotationDuration = 1f;
	[SerializeField] private float _rotationAngle = 90f;

	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => $"{_localizationManager.GetLocalizedString("InteractionObject_VendingMachine_Ammo_Reel")}?";
	public string InteractionHintMessageAction => null;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Wait")}!";

	private bool _isBusy = false;
	private LocalizationManager _localizationManager;
	public bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;

	private bool _isInteractionHintMessageFailActive;

	private void Start()
	{
		_vendingMachine = GetComponentInParent<InteractionObjectVendingMachineAmmo>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
	}


	public void Interact()
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