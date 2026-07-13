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

	private int _cartgridgeSlidingStep;

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

		ReapplyCartridgePosition();
	}

	protected override void ApplyWeaponRangedRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(2, 0.02f, 0.08f);
	}

	private void ReapplyCartridgePosition()
	{
		if (PlayerMagazineAmmoCurrent == 0)
		{
			_cartridge1stPerson.SetActive(false);
			_cartridge3rdPerson.SetActive(false);
			return;
		}

		_cartgridgeSlidingStep = PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent;
		//Debug.Log(_cartgridgeSlidingStep);

		if (_cartgridgeSlidingStep > 0)
		{
			_cartridge1stPerson.transform.localPosition += new Vector3(0.025f * (_cartgridgeSlidingStep), 0, 0);
			_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f * (_cartgridgeSlidingStep), 0, 0);

			for (int i = 0; i <= _cartgridgeSlidingStep; i++)
			{
				_bullets3rdPerson[i].SetActive(false);
				_bullets1stPerson[i].SetActive(false);
			}
		}
	}

	protected override void HideUsedHarmonicaBullet()
	{
		_bullets3rdPerson[_cartgridgeSlidingStep].SetActive(false);
		_bullets1stPerson[_cartgridgeSlidingStep].SetActive(false);

		_cartridge1stPerson.transform.localPosition += new Vector3(0.025f, 0, 0);
		_cartridge3rdPerson.transform.localPosition += new Vector3(0.025f, 0, 0);

		_cartgridgeSlidingStep++;

		if (_cartgridgeSlidingStep == 5)
		{
			EjectCartridge();
		}
	}

	private void EjectCartridge()
	{
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
		_ejectedCartridge.transform.rotation = _cartridge1stPerson.transform.rotation;

		_ejectedCartridge.AddComponent<BoxCollider>();
		_ejectedCartridge.AddComponent<Rigidbody>();

		_cartridge1stPerson.SetActive(false);
		_cartridge3rdPerson.SetActive(false);
	}

	protected override void RefillHarmonicaCartridge(int ammoToAdd)
	{
		int count = Mathf.Min(ammoToAdd, PlayerMagazineAmmoMax);

		_cartridge1stPerson.SetActive(true);
		_cartridge3rdPerson.SetActive(true);

		for (int i = 0; i < PlayerMagazineAmmoMax; i++)
		{
			_bullets3rdPerson[i].SetActive(false);
			_bullets1stPerson[i].SetActive(false);
		}

		if (count == 5)
		{
			for (int i = 0; i < PlayerMagazineAmmoMax; i++)
			{
				_bullets3rdPerson[i].SetActive(true);
				_bullets1stPerson[i].SetActive(true);
			}
		}
		else
		{
			for (int i = 0; i < PlayerMagazineAmmoCurrent; i++)
			{
				_bullets3rdPerson[i].SetActive(true);
				_bullets1stPerson[i].SetActive(true);
			}
		}

		if (PlayerAmmoReserve == 0)
		{
			_cartridge1stPerson.transform.localPosition = _cartridgeOriginalPosition;
			_cartridge3rdPerson.transform.localPosition = _cartridgeOriginalPosition;
		}
		else
		{
			_cartridge1stPerson.transform.localPosition -= new Vector3(0.025f * _cartgridgeSlidingStep, 0, 0);
			_cartridge3rdPerson.transform.localPosition -= new Vector3(0.025f * _cartgridgeSlidingStep, 0, 0);
		}

		_cartgridgeSlidingStep = 0;
	}
}