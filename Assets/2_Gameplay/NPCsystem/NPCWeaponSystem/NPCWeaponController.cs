using UnityEngine;

public class NPCweaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	[SerializeField] private NPCweaponSlotTypes _weaponRestingSlotType;
	private GameObject _weaponRestingSlot;
	private NPCdetectionManager _NPCdetectionManager;

	private GameObject _weaponHandSlot;

	public void Initialize(NPCdetectionManager NPCdetectionManager)
	{
		_NPCdetectionManager = NPCdetectionManager;

		GameObject weaponInstance = Instantiate(_NPCweapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();

		if (weaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

		_weaponHandSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R").gameObject;

		if (_weaponRestingSlotType== NPCweaponSlotTypes.Belt)
		{
			_weaponRestingSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/WeaponSlot_Belt").gameObject;
		}
		if (_weaponRestingSlotType == NPCweaponSlotTypes.Chest)
		{
			_weaponRestingSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/WeaponSlot_Chest").gameObject;
		}
		if (_weaponRestingSlotType == NPCweaponSlotTypes.Hand)
		{
			_weaponRestingSlot = _weaponHandSlot;
		}


		weaponComponent.InstantiateWeaponNPC(_weaponRestingSlot.transform);
	}

	private void EqiupWeapon()
	{

	}
}