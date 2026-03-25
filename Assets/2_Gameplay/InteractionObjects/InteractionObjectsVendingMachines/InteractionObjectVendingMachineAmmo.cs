using UnityEngine;

public class InteractionObjectVendingMachineAmmo : InteractionObjectVendingMachine
{
	// Приватное поле для хранения данных. Видно в Инспекторе.
	[SerializeField] private VendingMachineAmmoTypeData[] _ammoTypes;

	// Приватное поле для текущего индекса.
	private int _currentAmmoIndex = 0;

	// Публичное свойство только для чтения (get).
	// Позволяет другим классам видеть массив, но не изменять ссылку на него.
	public VendingMachineAmmoTypeData[] ammoTypes => _ammoTypes;

	// Публичное свойство для доступа к текущему индексу.
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
		goodsForSaleModel = data.ammoPrefab;
		goodsPrice = data.price;
		goodsName = data.ammoName;
	}

	private void Start()
	{
		UpdateGoods();
		base.Start();
	}
}