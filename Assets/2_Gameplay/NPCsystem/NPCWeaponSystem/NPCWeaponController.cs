using UnityEngine;

public class NPCWeaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	[SerializeField] private Transform _weaponHand;
	[SerializeField] private Transform _weaponRestingSlot;

	public void Initialize()
	{
		GameObject weaponInstance = Instantiate(_NPCweapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();

		if (weaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

		weaponComponent.InstantiateWeaponNPC(_weaponHand);
	}

	private void Update()
	{
		
	}
}