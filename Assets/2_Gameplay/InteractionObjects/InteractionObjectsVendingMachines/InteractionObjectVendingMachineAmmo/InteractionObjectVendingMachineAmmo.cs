using UnityEngine;

public class InteractionObjectVendingMachineAmmo : InteractionObjectVendingMachine
{
	[SerializeField] private VendingMachineAmmoTypeData[] _ammoTypes;
	private int _currentAmmoIndex = 0;

	public VendingMachineAmmoTypeData[] ammoTypes => _ammoTypes;

	public int currentAmmoIndex
	{
		get => _currentAmmoIndex;
		private set => _currentAmmoIndex = value;
	}

	public void SetCurrentAmmoType(int index)
	{
		currentAmmoIndex = index % _ammoTypes.Length;
		UpdateGoods();
	}

	private void UpdateGoods()
	{
		var data = _ammoTypes[currentAmmoIndex];
		_goodsForSaleModel = data.AmmoPrefab;
		_goodsPrice = data.Price;
		_goodsName = data.AmmoName;
	}

	private void Start()
	{
		UpdateGoods();
		base.Start();
	}
}