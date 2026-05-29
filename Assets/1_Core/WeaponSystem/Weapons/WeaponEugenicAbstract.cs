using UnityEngine;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public int ManaCost { get; protected set; }
	protected GameObject _player;
	protected GameObject _camera;
	protected PlayerResourcesManaManager playerResourcesManaManager;

	private void Start()
	{
		if (_isThisPlayerWeapon == true)
		{
			_player = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
			_camera = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");

			playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		}

		InitializeWeaponEugenic();
	}

	protected abstract void InitializeWeaponEugenic();
}