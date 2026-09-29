using UnityEngine;

public class NPCweaponAnimationController : MonoBehaviour
{
	private NPCweaponController _NPCweaponController;
	private Animator _NPCweaponAnimator;
	private NPCdetectionVisualController _NPCdetectionVisualController;

	private int LayerAimUpDown;
	private int LayerWeaponRightEquip;
	private int LayerWeaponLeftEquip;
	private int LayerWeaponRightPalm;
	private int LayerWeaponLeftPalm;
	private int LayerWeaponRightArm;
	private int LayerWeaponLeftArm;
	private int LayerWeaponReload;

	private bool _isInitialized;

	private float _aimUpDownParameter;

	public void Initialize(
		NPCdetectionVisualController NPCdetectionVisualController,
		NPCweaponController NPCweaponController,
		Animator NPCweaponAnimator)
	{
		_NPCdetectionVisualController = NPCdetectionVisualController;
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

		_aimUpDownParameter = _NPCweaponAnimator.GetFloat("UpDown");

		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized) return;

		ProcessAimUpDown();
	}

	private void ProcessAimUpDown()
	{
		float endValue;

		// Проверяем, видит ли NPC игрока
		if (_NPCdetectionVisualController.IsPlayerVisible)
		{
			// Игрок ВИДЕН: вычисляем целевой угол
			float angle = _NPCdetectionVisualController.RaycastAngleFromXAxis;
			float clampedAngle = Mathf.Clamp(angle, -70f, 70f);
			endValue = clampedAngle / 70f;
		}
		else
		{
			// Игрок НЕ ВИДЕН: целевое значение — 0 (нейтральное положение)
			endValue = 0f;
		}

		// Плавно интерполируем текущее значение к целевому (endValue)
		float rawLerp = Mathf.Lerp(_aimUpDownParameter, endValue, Time.deltaTime * 6f);
		_aimUpDownParameter = rawLerp;

		// Фиксируем значение, если мы почти достигли цели (чтобы не было микро-вибраций)
		if (Mathf.Abs(rawLerp - endValue) < 0.001f)
		{
			_aimUpDownParameter = endValue;
		}

		_NPCweaponAnimator.SetFloat("UpDown", _aimUpDownParameter);
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