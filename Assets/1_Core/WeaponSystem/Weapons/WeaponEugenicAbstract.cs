using UnityEngine;

public abstract class EugenicWeaponAbstract : WeaponAbstract
{
	public int ManaCost { get; protected set; }
	protected GameObject player;
	protected GameObject camera;
	protected PlayerResourcesManaManager playerResourcesManaManager;

	private void Start()
	{
		if (IsThisPlayerWeapon == true)
		{
			player = ServiceLocator.Resolve<GameObject>("Player");
			camera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");

			playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		}

		InitializeWeaponEugenic();
	}

	protected abstract void InitializeWeaponEugenic();
}

