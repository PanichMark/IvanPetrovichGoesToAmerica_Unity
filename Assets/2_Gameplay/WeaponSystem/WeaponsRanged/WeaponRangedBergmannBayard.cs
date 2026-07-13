using UnityEngine;

public class WeaponRangedBergmannBayard : WeaponRangedAbstract
{
	public override string WeaponName => "BergmannBayard";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override WeaponsRangedEnum RangedWeaponType => WeaponsRangedEnum.BergmannBayard;
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	public override float WeaponDamage => 20f;
	public override bool IsWeaponAuto => true;

	protected override void InitializeWeaponRanged()
	{
		_weaponAutoAttackSpeedRate = 0.1f;
		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");
	}

	protected override void ApplyWeaponRangedRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilAuto();
	}
}