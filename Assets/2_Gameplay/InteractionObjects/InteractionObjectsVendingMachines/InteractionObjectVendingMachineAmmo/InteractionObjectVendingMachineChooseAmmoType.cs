using UnityEngine;
using System.Collections;

public class InteractionObjectVendingMachineChooseAmmoType : MonoBehaviour, IInteractable
{
	public delegate void AmmoTypeChangedHandler();

	private InteractionObjectVendingMachineAmmo _vendingMachine;
	[SerializeField] private string _name = "Выбор патронов";
	[SerializeField] private float _rotationDuration = 1f;
	[SerializeField] private float _rotationAngle = 90f;

	public string InteractionObjectNameSystem => _name;
	public string InteractionObjectNameUI => _name;
	public string InteractionHintMessageMain => "Сменить тип патронов";
	public string InteractionHintMessageAction => "Сменить";
	public string InteractionHintMessageFail => "Подождите!";

	private bool _isBusy = false;

	public bool IsInteractionHintMessageFailActive => _isBusy;

	private void Start()
	{
		_vendingMachine = GetComponentInParent<InteractionObjectVendingMachineAmmo>();
	}

	public void Interact()
	{
		if (_isBusy)
			return;

		_vendingMachine.SetCurrentAmmoType(_vendingMachine.currentAmmoIndex + 1);
		StartCoroutine(RotateAndChangeAmmo());
		_isBusy = true;
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

		_isBusy = false;
	}
}