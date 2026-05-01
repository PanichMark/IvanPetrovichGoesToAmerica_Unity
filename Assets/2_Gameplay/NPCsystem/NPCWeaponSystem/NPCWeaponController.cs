using UnityEngine;
using static UnityEditor.U2D.ScriptablePacker;

public class NPCWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject NPCWeapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	Transform palmTransform;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		palmTransform = transform.Find("Armature_Human_Male_Strong/Root/Spine/Arm.R/Forearm.R/Palm.R");
		

		GameObject weaponInstance = Instantiate(NPCWeapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();
        weaponComponent.InstantiateWeapon(palmTransform);
	}

	private void Update()
	{
		//Debug.Log(palmTransform);
	}

}
