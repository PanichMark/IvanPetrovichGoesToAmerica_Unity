using UnityEngine;
using UnityEngine.SceneManagement;

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
	private GameObject _ejectedCartridge;

	private GameObject[] _bullets1stPerson = new GameObject[5];
	private GameObject[] _bullets3rdPerson = new GameObject[5];

	private Vector3 _cartridgeOriginalPosition;

	protected override void InitializeWeaponRanged()
	{
		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");

		_cartridge1stPerson = FirstPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;
		_cartridge3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Cartridge").gameObject;

		_cartridgeOriginalPosition = _cartridge1stPerson.transform.localPosition;

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
		int bulletIndex = PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent - 1;

		//Debug.Log(bulletIndex);
		_bullets3rdPerson[bulletIndex].SetActive(false);
		_bullets1stPerson[bulletIndex].SetActive(false);

		_cartridge1stPerson.transform.localPosition += new Vector3(0.025f, 0, 0);
		_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f, 0, 0);

		if ((bulletIndex == 4 && PlayerAmmoTotalCurrent > 0))
		{
			EjectCartridge();
		}
	}

	private void EjectCartridge()
	{
		Debug.Log("EJECT");
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == "FirstPerson")
		{
			_ejectedCartridge = Instantiate(_cartridge1stPerson);
		}
		else
		{
			_ejectedCartridge = Instantiate(_cartridge3rdPerson);
		}

		_ejectedCartridge.layer = LayerMask.NameToLayer("Default");

		_ejectedCartridge.transform.SetParent(null);

		SceneManager.MoveGameObjectToScene(_ejectedCartridge, SceneManager.GetSceneByBuildIndex(1));

		_ejectedCartridge.transform.position = transform.position;

		_ejectedCartridge.AddComponent<BoxCollider>();

		Rigidbody rb1 = _ejectedCartridge.AddComponent<Rigidbody>();

		Destroy(_ejectedCartridge, 30);

		//rb1.interpolation = RigidbodyInterpolation.Interpolate;

		_cartridge1stPerson.SetActive(false);
		_cartridge3rdPerson.SetActive(false);
	}

	protected override void RefillHarmonicaCartridge(int ammoToAdd)
	{
		int count = Mathf.Min(ammoToAdd, PlayerMagazineAmmoMax);

		if (PlayerAmmoTotalCurrent >= 5)
		{
			for (int i = 0; i < count; i++)
			{
				_bullets3rdPerson[i].SetActive(true);
				_bullets1stPerson[i].SetActive(true);
			}
		}
		else
		{
			for (int PlayerAmmoTotalCurrent = 0; PlayerAmmoTotalCurrent < count; PlayerAmmoTotalCurrent++)
			{
				_bullets3rdPerson[PlayerAmmoTotalCurrent].SetActive(true);
				_bullets1stPerson[PlayerAmmoTotalCurrent].SetActive(true);
			}
		}

		_cartridge1stPerson.SetActive(true);
		_cartridge3rdPerson.SetActive(true);

		Debug.Log(PlayerAmmoTotalCurrent);



		if (PlayerAmmoTotalCurrent == 0)
		{
			_cartridge1stPerson.transform.localPosition = _cartridgeOriginalPosition;
			_cartridge3rdPerson.transform.localPosition = _cartridgeOriginalPosition;
		}
		else
		{
			_cartridge1stPerson.transform.localPosition -= new Vector3(0.025f * count, 0, 0);
			_cartridge3rdPerson.transform.localPosition -= new Vector3(0.025f * count, 0, 0);
		}
	}
}