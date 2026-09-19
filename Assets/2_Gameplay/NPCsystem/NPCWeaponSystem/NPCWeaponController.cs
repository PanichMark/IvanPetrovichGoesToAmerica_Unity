using UnityEngine;

public class NPCweaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweaponGive;
	private GameObject _NPCweaponInstance;
	private WeaponAbstract _NPCweaponConponent;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	[SerializeField] private NPCweaponSlotTypes _weaponRestingSlotType;
	private GameObject _weaponRestingSlot;
	private NPCdetectionManager _NPCdetectionManager;
	private NPCstateMachineController _NPCstateMachineController;
	private GameObject _weaponHandSlot;

	private bool _isWeaponEquipped;
	private bool _wasWeaponDropped;

	public void Initialize(
		NPCstateMachineController NPCstateMachineController,
		NPCdetectionManager NPCdetectionManager)
	{
		_NPCstateMachineController = NPCstateMachineController;
		_NPCdetectionManager = NPCdetectionManager;

		_NPCweaponInstance = Instantiate(_NPCweaponGive);
		_NPCweaponConponent = _NPCweaponInstance.GetComponent<WeaponAbstract>();

		if (_NPCweaponConponent == null)
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
			_isWeaponEquipped = true;

			_weaponRestingSlot = _weaponHandSlot;
		}

		_NPCweaponConponent.InstantiateWeaponNPC(_weaponRestingSlot.transform);

		_NPCstateMachineController.OnNewNPCstate += ChangeWeaponState;
	}

	private void ChangeWeaponState(NPCstateTypes newNPCstateType)
	{
		if (newNPCstateType == NPCstateTypes.Alarmed)
		{
			if (!_isWeaponEquipped || _weaponRestingSlotType != NPCweaponSlotTypes.Hand)
			{
				EqiupWeapon();
			}
		}

		if (newNPCstateType == NPCstateTypes.Dead || newNPCstateType == NPCstateTypes.Unconscious)
		{
			if (_isWeaponEquipped && !_wasWeaponDropped)
			{
				DropWeapon();
			}
		}
	}

	private void EqiupWeapon()
	{
		_isWeaponEquipped = true;

		_NPCweaponInstance.transform.SetParent(_weaponHandSlot.transform, false);

		_NPCweaponInstance.transform.localRotation = Quaternion.identity;

	}

	private void UnequipWeapon()
	{
		_isWeaponEquipped = false;
	}

	private void AttackWeapon()
	{ 

	}

	private void ReloadRangedWeapon()
	{

	}

	private void DropWeapon()
	{
		_wasWeaponDropped = true;

		_NPCweaponInstance.transform.SetParent(null);
		_NPCweaponInstance.AddComponent<Rigidbody>();
	}
}