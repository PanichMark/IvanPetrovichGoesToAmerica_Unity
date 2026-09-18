using UnityEngine;

public class NPCweaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweapon;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	[SerializeField] private NPCweaponSlotTypes _weaponRestingSlotType;
	private GameObject _weaponRestingSlot;

	public void Initialize()
	{
		GameObject weaponInstance = Instantiate(_NPCweapon);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();

		if (weaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

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
			_weaponRestingSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R").gameObject;
		}

		weaponComponent.InstantiateWeaponNPC(_weaponRestingSlot.transform);
	}
}