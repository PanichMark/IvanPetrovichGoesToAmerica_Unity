using UnityEngine;

public class NPCWeaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	private Transform _palmTransform;

	private void Start()
	{
		_palmTransform = transform.Find("Armature_Human_Male_Strong/Root/Spine/Arm.R/Forearm.R/Palm.R");

		if (_palmTransform == null)
		{
			Debug.LogError("Palm transform not found!");
			return;
		}

		GameObject weaponInstance = Instantiate(_NPCweapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();

		if (weaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

		weaponComponent.InstantiateWeapon(_palmTransform);
	}

	private void Update()
	{
		
	}
}