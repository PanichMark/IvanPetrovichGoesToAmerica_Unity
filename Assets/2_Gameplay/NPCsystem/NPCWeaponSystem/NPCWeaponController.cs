using UnityEngine;

public class NPCWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject NPCWeapon;
	public Vector3 NPCWeaponSlotTransform {  get; private set; }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        NPCWeaponSlotTransform = transform.forward * 0.5f;

		GameObject weaponInstance = Instantiate(NPCWeapon);
		//WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();
        //weaponComponent.InstantiateWeapon(this);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
