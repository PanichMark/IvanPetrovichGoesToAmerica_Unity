using UnityEngine;

public class NPCweaponAnimationController : MonoBehaviour
{
	private NPCweaponController _NPCweaponController;
	private Animator _NPCweaponAnimator;

	private int LayerAimUpDown;
	private int LayerWeaponRightEquip;
	private int LayerWeaponLeftEquip;
	private int LayerWeaponRightPalm;
	private int LayerWeaponLeftPalm;
	private int LayerWeaponRightArm;
	private int LayerWeaponLeftArm;
	private int LayerWeaponReload;

	public void Initialize(
		NPCweaponController NPCweaponController,
		Animator NPCweaponAnimator)
	{
		_NPCweaponController = NPCweaponController;
		_NPCweaponAnimator = NPCweaponAnimator;

		LayerAimUpDown = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerLookUpDown.ToString());
		LayerWeaponRightEquip = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightEquip.ToString());
		LayerWeaponRightPalm = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightPalm.ToString());
		LayerWeaponRightArm = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponRightFullArm.ToString());
		LayerWeaponLeftEquip = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftEquip.ToString());
		LayerWeaponLeftPalm = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftPalm.ToString());
		LayerWeaponLeftArm = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponLeftFullArm.ToString());
		LayerWeaponReload = _NPCweaponAnimator.GetLayerIndex(AnimatorControllerHumanoidLayersEnum.LayerWeaponReload.ToString());
	}

	private void ProcessUpDownAiming()
	{

	}

	private void EquipWeaponAnimation(NPCweaponSlotTypes slotType, WeaponAbstract weaponType)
	{

	}

	private void UnequipWeaponAnimation(NPCweaponSlotTypes slotType, WeaponAbstract weaponType)
	{

	}

	private void AttackWeaponAnimation(WeaponAbstract weaponType)
	{

	}

	private void ReloadRangedWeaponAnimation(WeaponRangedAbstract rangedWeaponType)
	{

	}
}