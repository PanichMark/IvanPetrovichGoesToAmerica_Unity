using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
		_goodsForSale = data.AmmoPrefab;
		_goodsPrice = data.Price;
		_goodsName = _localizationManager.GetLocalizedString(data.AmmoName, gameObject.name);
	}

	public override void SetpUpVendingMachine()
	{
		UpdateGoods();
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.VendingMachinesData == null || !data.VendingMachinesData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			VendingMachineData savedState = sourceList.Find(item => item.VendingMachineIndex == GameplayObjectIndex);

			if (savedState.VendingMachineIndex != 0)
			{
				_vendingMachineElectroHealth = savedState.VendingMachineHealth;

				if (savedState.VendingMachineSpawnedGoods > 0)
				{
					for (int i = 0; i < savedState.VendingMachineSpawnedGoods; i++)
					{
						SpawnGoods();
					}
				}

				if (_vendingMachineElectroHealth <= 0)
				{
					IsOutOfService = true;

					InvokeOnWentOutOfService();
				}
			}
		}

		yield return null;
	}
}