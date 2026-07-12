using UnityEngine;

public class WeaponRangedHarmonicaRevolver : WeaponRangedAbstract
{
	public override string WeaponName => "HarmonicaRevolver";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	public override float WeaponDamage => 34f;
	public override bool IsWeaponAuto => false;

	private GameObject _cartridge1stPerson;
	private GameObject _cartridge3rdPerson;

	private GameObject[] _bullets1stPerson = new GameObject[5];
	private GameObject[] _bullets3rdPerson = new GameObject[5];

	protected override void InitializeWeaponRanged()
	{
		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");

		_cartridge1stPerson = FirstPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;
		_cartridge3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;

		for (int i = 0; i < PlayerMagazineAmmoMax; i++)
		{
			_bullets3rdPerson[i] = _cartridge3rdPerson.transform.Find($"Bullet{i + 1}").gameObject;
			_bullets1stPerson[i] = _cartridge1stPerson.transform.Find($"Bullet{i + 1}").gameObject;
		}
	}

	protected override void ApplyWeaponRangedRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(2, 0.02f, 0.08f);
	}

	protected override void HideUsedHarmonicaBullet()
	{
		int bulletIndex = PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent;

		Debug.Log(bulletIndex);
		_bullets3rdPerson[bulletIndex].SetActive(false);
		_bullets1stPerson[bulletIndex].SetActive(false);

		_cartridge1stPerson.transform.localPosition += new Vector3(0.025f, 0, 0);
		_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f, 0, 0);
	}

	protected override void RefillHarmonicaCartridge(int ammoToAdd)
	{
		int count = Mathf.Min(ammoToAdd, PlayerMagazineAmmoMax);

		for (int i = 0; i < count; i++)
		{
			Debug.Log(i);
			_bullets3rdPerson[i].SetActive(true);
			_bullets1stPerson[i].SetActive(true);
		}

		_cartridge1stPerson.transform.localPosition -= new Vector3(0.025f * ammoToAdd, 0, 0);
		_cartridge3rdPerson.transform.localPosition -= new Vector3(0.025f * ammoToAdd, 0, 0);
	}
}