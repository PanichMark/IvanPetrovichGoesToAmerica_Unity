using UnityEngine;

public class NPCweaponController : MonoBehaviour
{
	[SerializeField] private GameObject _NPCweaponGive;
	private GameObject _NPCweaponInstance;
	private WeaponAbstract _NPCweaponComponent;
	public Vector3 NPCWeaponSlotTransform { get; private set; }
	[SerializeField] private NPCweaponSlotTypes _weaponRestingSlotType;
	[SerializeField] private WeaponHandType _weaponHandType;
	[SerializeField] private float _attackCooldown;
	private GameObject _weaponRestingSlot;
	private NPCdetectionManager _NPCdetectionManager;
	private NPCstateMachineController _NPCstateMachineController;
	private GameObject _weaponHandSlot;
	private TransferSkinnedMeshRendererArmatureBones _transferArmatureBones;
	private Transform _NPCweaponAttackPoint;
	private bool _isWeaponEquipped;
	private bool _wasWeaponDropped;

	public void Initialize(
		NPCstateMachineController NPCstateMachineController,
		NPCdetectionManager NPCdetectionManager,
		TransferSkinnedMeshRendererArmatureBones transferArmatureBones)
	{
		_NPCstateMachineController = NPCstateMachineController;
		_NPCdetectionManager = NPCdetectionManager;
		_transferArmatureBones = transferArmatureBones;

		_NPCweaponInstance = Instantiate(_NPCweaponGive);
		
		_NPCweaponComponent = _NPCweaponInstance.GetComponent<WeaponAbstract>();

		if (_NPCweaponComponent == null)
		{
			Debug.LogError("WeaponAbstract component not found on weapon instance!");
			return;
		}

		if (_weaponHandType == WeaponHandType.Right)
		{
			_weaponHandSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R").gameObject;
		}
		else
		{
			_weaponHandSlot = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Arm.L/Forearm.L/Palm.L/WeaponSlot_Hand.L").gameObject;
		}

		if (_weaponRestingSlotType == NPCweaponSlotTypes.Belt)
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

		if (_NPCweaponComponent is WeaponEugenicAbstract)
		{
			TransferWeaponEugenicBones(_weaponHandType);
		}

		_NPCweaponAttackPoint = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine");
		_NPCweaponAttackPoint.rotation = Quaternion.Euler(0f, 90f, 0f);

		_NPCweaponComponent.InstantiateWeaponNPC(_weaponRestingSlot.transform, _NPCweaponAttackPoint);

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
				if (_NPCweaponComponent is not WeaponEugenicAbstract)
				{
					DropWeapon();
				}
				else
				{

				}
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
		_NPCweaponComponent.WeaponNPCattack();
	}

	private float _timer;

	private void Update()
	{
		_timer += Time.deltaTime;

		if (_timer >= 1f)
		{
			_timer = 0f;
			_NPCweaponComponent.WeaponNPCattack();
		}
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

	private void TransferWeaponEugenicBones(WeaponHandType weaponHand)
	{
		GameObject eugenicArmature = null;
		SkinnedMeshRenderer eugenicSkinnedMesh = null;

		GameObject deleteOtherHandEugenicArmature = null;
		GameObject deleteOtherHandEugenicSkinnedMesh = null;

		if (weaponHand == WeaponHandType.Right)
		{
			eugenicArmature = _NPCweaponInstance.transform.Find("Armature.R").gameObject;
			eugenicSkinnedMesh = _NPCweaponInstance.transform.Find("Eugenic.R").GetComponent<SkinnedMeshRenderer>();
		}
		else
		{
			eugenicArmature = _NPCweaponInstance.transform.Find("Armature.L").gameObject;
			eugenicSkinnedMesh = _NPCweaponInstance.transform.Find("Eugenic.L").GetComponent<SkinnedMeshRenderer>();
		}

		_transferArmatureBones.TransferWeaponEugenicBones(eugenicArmature, eugenicSkinnedMesh, weaponHand);
		
		if (weaponHand == WeaponHandType.Right)
		{
			deleteOtherHandEugenicSkinnedMesh = _NPCweaponInstance.transform.Find("Armature.L").gameObject;
			deleteOtherHandEugenicArmature = _NPCweaponInstance.transform.Find("Eugenic.L").gameObject;

			Destroy(deleteOtherHandEugenicArmature);
			Destroy(deleteOtherHandEugenicSkinnedMesh);
		}
		else
		{
			deleteOtherHandEugenicSkinnedMesh = _NPCweaponInstance.transform.Find("Armature.R").gameObject;
			deleteOtherHandEugenicArmature = _NPCweaponInstance.transform.Find("Eugenic.R").gameObject;

			Destroy(deleteOtherHandEugenicArmature);
			Destroy(deleteOtherHandEugenicSkinnedMesh);
		}
	}
}