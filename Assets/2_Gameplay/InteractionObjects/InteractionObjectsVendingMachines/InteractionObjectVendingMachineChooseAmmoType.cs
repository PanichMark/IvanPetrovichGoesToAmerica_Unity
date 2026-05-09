using UnityEngine;
using System.Collections;

public class InteractionObjectVendingMachineChooseAmmoType : MonoBehaviour, IInteractable
{
	public delegate void AmmoTypeChangedHandler();

	[SerializeField] private InteractionObjectVendingMachineAmmo vendingMachine;
	[SerializeField] private string Name = "Выбор патронов";
	[SerializeField] private float rotationDuration = 1f;
	[SerializeField] private Vector3 rotationAxis = Vector3.right;
	[SerializeField] private float rotationAngle = 90f;

	public string InteractionObjectNameSystem => Name;
	public string InteractionObjectNameUI => Name;
	public string InteractionHintMessageMain => "Сменить тип патронов";
	public string InteractionHintAction => "Сменить";
	public string InteractionHintMessageAdditional => "Подождите!";

	private bool isBusy = false;

	public bool IsInteractionHintMessageAdditionalActive => isBusy;

	public void Interact()
	{
		if (isBusy)
			return;

		vendingMachine.SetCurrentAmmoType(vendingMachine.currentAmmoIndex + 1);
		StartCoroutine(RotateAndChangeAmmo());
		isBusy = true;
	}

	private IEnumerator RotateAndChangeAmmo()
	{
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(rotationAxis * rotationAngle);

		float elapsedTime = 0f;

		while (elapsedTime < rotationDuration)
		{
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.rotation = endRotation;

		Debug.Log($"Selected ammo type: {vendingMachine.ammoTypes[vendingMachine.currentAmmoIndex].ammoName}");

		isBusy = false;
	}
}