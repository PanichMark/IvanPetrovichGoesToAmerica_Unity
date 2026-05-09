using UnityEngine;

public class NPCWeaponController : MonoBehaviour
{
	[SerializeField] private GameObject NPCWeapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	private Transform palmTransform;

	private void Start()
	{
		palmTransform = transform.Find("Armature_Human_Male_Strong/Root/Spine/Arm.R/Forearm.R/Palm.R");

		if (palmTransform == null)
		{
			Debug.LogError("Palm transform not found!");
			return;
		}

		GameObject weaponInstance = Instantiate(NPCWeapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();

		if (weaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

		weaponComponent.InstantiateWeapon(palmTransform);
	}

	private void Update()
	{
		
	}
}