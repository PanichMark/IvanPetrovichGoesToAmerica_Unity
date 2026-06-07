using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
	//Important
	public string SafeFileDateAndTime;

	//Scene
	public string SceneNameSystem;

	//Player 
	public Vector3 PlayerPosition;
	public Quaternion PlayerRotation;
	public string PlayerMovementStateType;

	//Camera
	public float PlayerCameraDistanceY;
	public float PlayerCameraDistanceZ;
	public Quaternion CameraRotation;
	public string CurrentPlayerCameraStateType;
	public bool IsCameraShoulderRight;

	//Behaviour
	public bool IsPlayerArmed;
	public bool WasPlayerArmed;

	//Resources
	public float PlayerHealth;
	public int HealingItems;
	public int PlayerMana;
	public int ManaReplenishItems;
	public int PlayerMoney;
	public List<AmmoTypeData> AmmoDictionary;

	//Weapons
	public List<string> UnlockedWeapons;
	public List<WeaponRangedData> UnlockedRangedWeapons;
	public string WeaponRightHand;
	public string WeaponLeftHand;

	//NPCs
	public List<NPCdata> NPCdata_Scene_0_Test;
	public List<NPCdata> NPCdata_Scene_1_Church;
	public List<NPCdata> NPCdata_Scene_1_Street;
	public List<NPCdata> NPCdata_Scene_1_RevenueHouse;
	public List<NPCdata> NPCdata_Scene_1_InnerYard;

	//LootObjects
	public List<LootObjectData> LootObjects_Scene_0_Test;
	public List<LootObjectData> LootObjects_Scene_1_Church;
	public List<LootObjectData> LootObjects_Scene_1_Street;
	public List<LootObjectData> LootObjects_Scene_1_RevenueHouse;
	public List<LootObjectData> LootObjects_Scene_1_InnerYard;

	// ConsumableObjects
	public List<ConsumableObjectData> ConsumableObjectData_Scene_0_Test;
	public List<ConsumableObjectData> ConsumableObjectData_Scene_1_Church;
	public List<ConsumableObjectData> ConsumableObjectData_Scene_1_Street;
	public List<ConsumableObjectData> ConsumableObjectData_Scene_1_RevenueHouse;
	public List<ConsumableObjectData> ConsumableObjectData_Scene_1_InnerYard;

	//PickableObjects
	public List<PickableObjectData> PickableObjects_Scene_0_Test;
	public List<PickableObjectData> PickableObjects_Scene_1_Church;
	public List<PickableObjectData> PickableObjects_Scene_1_Street;
	public List<PickableObjectData> PickableObjects_Scene_1_RevenueHouse;
	public List<PickableObjectData> PickableObjects_Scene_1_InnerYard;

	//Keys
	public List<KeyData> CollectedKeys;

	//Openable Objects
	public List<OpenableObjectData> OpenableObjects_Scene_0_Test;
	public List<OpenableObjectData> OpenableObjects_Scene_1_Church;
	public List<OpenableObjectData> OpenableObjects_Scene_1_Street;
	public List<OpenableObjectData> OpenableObjects_Scene_1_RevenueHouse;
	public List<OpenableObjectData> OpenableObjects_Scene_1_InnerYard;

	//Safes
	public List<SafeData> SafesData_Scene_0_Test;
	public List<SafeData> SafesData_Scene_1_Church;
	public List<SafeData> SafesData_Scene_1_Street;
	public List<SafeData> SafesData_Scene_1_RevenueHouse;
	public List<SafeData> SafesData_Scene_1_InnerYard;

	//VendingMachine
	public List<VendingMachineData> VendingMachinesData_Scene_0_Test;
	public List<VendingMachineData> VendingMachinesData_Scene_1_Church;
	public List<VendingMachineData> VendingMachinesData_Scene_1_Street;
	public List<VendingMachineData> VendingMachinesData_Scene_1_RevenueHouse;
	public List<VendingMachineData> VendingMachinesData_Scene_1_InnerYard;

	//Elevators
	public List<ElevatorData> ElevatorsData_Scene_0_Test;
	public List<ElevatorData> ElevatorsData_Scene_1_Church;
	public List<ElevatorData> ElevatorsData_Scene_1_Street;
	public List<ElevatorData> ElevatorsData_Scene_1_RevenueHouse;
	public List<ElevatorData> ElevatorsData_Scene_1_InnerYard;

	//Lights
	public List<LightData> LightsData_Scene_0_Test;
	public List<LightData> LightsData_Scene_1_Church;
	public List<LightData> LightsData_Scene_1_Street;
	public List<LightData> LightsData_Scene_1_RevenueHouse;
	public List<LightData> LightsData_Scene_1_InnerYard;

	//TVs
	public List<TVdata> TVsData_Scene_0_Test;
	public List<TVdata> TVsData_Scene_1_Church;
	public List<TVdata> TVsData_Scene_1_Street;
	public List<TVdata> TVsData_Scene_1_RevenueHouse;
	public List<TVdata> TVsData_Scene_1_InnerYard;

	//Phonographs
	public List<PhonographData> PhonographsData_Scene_0_Test;
	public List<PhonographData> PhonographsData_Scene_1_Church;
	public List<PhonographData> PhonographsData_Scene_1_Street;
	public List<PhonographData> PhonographsData_Scene_1_RevenueHouse;
	public List<PhonographData> PhonographsData_Scene_1_InnerYard;

	public GameData()
	{
		//Important
		SafeFileDateAndTime = DateTime.Now.ToString();

		//Scene
		SceneNameSystem = GameScenesEnum.Scene_0_Test.ToString();

		//Player 
		PlayerPosition = new Vector3(0, 0, -5);
		PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdle.ToString();

		//Camera
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		CameraRotation = new Quaternion(0, 0, 0, 0);
		CurrentPlayerCameraStateType = PlayerCameraStateTypes.FirstPerson.ToString();
		IsCameraShoulderRight = true;

		//Behaviour
		IsPlayerArmed = false;
		WasPlayerArmed = false;

		//Resources
		PlayerHealth = 50;
		HealingItems = 9;
		PlayerMana = 50;
		ManaReplenishItems = 9;
		PlayerMoney = 100;
		AmmoDictionary = new List<AmmoTypeData>
		{
			new AmmoTypeData
			{
				AmmoTypeSystem = AmmoTypes.Ammo9mm,
				AmmoTypeJson = AmmoTypes.Ammo9mm.ToString(),
				TotalAmmoMax = 999,
				TotalAmmoCurrent = 25
			},
			new AmmoTypeData
			{
				AmmoTypeSystem = AmmoTypes.Ammo12gauge,
				AmmoTypeJson = AmmoTypes.Ammo12gauge.ToString(),
				TotalAmmoMax = 999,
				TotalAmmoCurrent = 10
			}
		};

		//Weapons
		UnlockedWeapons = new List<string>();
		UnlockedRangedWeapons = new List<WeaponRangedData>();
		WeaponRightHand = null;
		WeaponLeftHand = null;

		//NPCs
		NPCdata_Scene_0_Test = new List<NPCdata>();
		NPCdata_Scene_1_Church = new List<NPCdata>();
		NPCdata_Scene_1_Street = new List<NPCdata>();
		NPCdata_Scene_1_RevenueHouse = new List<NPCdata>();
		NPCdata_Scene_1_InnerYard = new List<NPCdata>();

		//LootObjects
		LootObjects_Scene_0_Test = new List<LootObjectData>();
		LootObjects_Scene_1_Church = new List<LootObjectData>();
		LootObjects_Scene_1_Street = new List<LootObjectData>();
		LootObjects_Scene_1_RevenueHouse = new List<LootObjectData>();
		LootObjects_Scene_1_InnerYard = new List<LootObjectData>();

		//ConsumableObjects
		ConsumableObjectData_Scene_0_Test = new List<ConsumableObjectData>();
		ConsumableObjectData_Scene_1_Church = new List<ConsumableObjectData>();
		ConsumableObjectData_Scene_1_Street = new List<ConsumableObjectData>();
		ConsumableObjectData_Scene_1_RevenueHouse = new List<ConsumableObjectData>();
		ConsumableObjectData_Scene_1_InnerYard = new List<ConsumableObjectData>();

		//PickableObjects
		PickableObjects_Scene_0_Test = new List<PickableObjectData>();
		PickableObjects_Scene_1_Church = new List<PickableObjectData>();
		PickableObjects_Scene_1_Street = new List<PickableObjectData>();
		PickableObjects_Scene_1_RevenueHouse = new List<PickableObjectData>();
		PickableObjects_Scene_1_InnerYard = new List<PickableObjectData>();

		//Keys
		CollectedKeys = new List<KeyData>();

		//Openable Objects
		OpenableObjects_Scene_0_Test = new List<OpenableObjectData>();
		OpenableObjects_Scene_1_Church = new List<OpenableObjectData>();
		OpenableObjects_Scene_1_Street = new List<OpenableObjectData>();
		OpenableObjects_Scene_1_RevenueHouse = new List<OpenableObjectData>();
		OpenableObjects_Scene_1_InnerYard = new List<OpenableObjectData>();

		//Safes
		SafesData_Scene_0_Test = new List<SafeData>();
		SafesData_Scene_1_Church = new List<SafeData>();
		SafesData_Scene_1_Street = new List<SafeData>();
		SafesData_Scene_1_RevenueHouse = new List<SafeData>();
		SafesData_Scene_1_InnerYard = new List<SafeData>();

		//VendingMachines
		VendingMachinesData_Scene_0_Test = new List<VendingMachineData>();
		VendingMachinesData_Scene_1_Church = new List<VendingMachineData>();
		VendingMachinesData_Scene_1_Street = new List<VendingMachineData>();
		VendingMachinesData_Scene_1_RevenueHouse = new List<VendingMachineData>();
		VendingMachinesData_Scene_1_InnerYard = new List<VendingMachineData>();

		//Elevators
		ElevatorsData_Scene_0_Test = new List<ElevatorData>();
		ElevatorsData_Scene_1_Church = new List<ElevatorData>();
		ElevatorsData_Scene_1_Street = new List<ElevatorData>();
		ElevatorsData_Scene_1_RevenueHouse = new List<ElevatorData>();
		ElevatorsData_Scene_1_InnerYard = new List<ElevatorData>();

		//Lights
		LightsData_Scene_0_Test = new List<LightData>();
		LightsData_Scene_1_Church = new List<LightData>();
		LightsData_Scene_1_Street = new List<LightData>();
		LightsData_Scene_1_RevenueHouse = new List<LightData>();
		LightsData_Scene_1_InnerYard = new List<LightData>();

		//TVs
		TVsData_Scene_0_Test = new List<TVdata>();
		TVsData_Scene_1_Church = new List<TVdata>();
		TVsData_Scene_1_Street = new List<TVdata>();
		TVsData_Scene_1_RevenueHouse = new List<TVdata>();
		TVsData_Scene_1_InnerYard = new List<TVdata>();

		//Phonographs
		PhonographsData_Scene_0_Test = new List<PhonographData>();
		PhonographsData_Scene_1_Church = new List<PhonographData>();
		PhonographsData_Scene_1_Street = new List<PhonographData>();
		PhonographsData_Scene_1_RevenueHouse = new List<PhonographData>();
		PhonographsData_Scene_1_InnerYard = new List<PhonographData>();
	}
}

[System.Serializable]
public struct NPCdata
{

}

[System.Serializable]
public struct LootObjectData
{
	public int LootObjectIndex;
	public string LootObjectNameSystem;
	public bool WasLootObjectCollected;
}

[System.Serializable]
public struct ConsumableObjectData
{

}

[System.Serializable]
public struct PickableObjectData
{
	public int PickableObjectIndex;
	public string PickableObjectNameSystem;
	public Vector3 PickableObjecPosition;
	public Quaternion PickableObjecRotation;
	public bool WasPickableObjectPickedUp;
	public bool WasPickableObjectDestroyed;
}

[System.Serializable]
public struct KeyData
{
	public string KeyName;
	public bool WasKeyCollected;
}

[System.Serializable]
public struct OpenableObjectData
{
	public int OpenableObjectIndex;
	public string OpenableObjectNameSystem;
	public bool WasOpenableObjectUnlocked;
	public bool WasOpenableObjectOpened;
}

[System.Serializable]
public struct SafeData
{

}

[System.Serializable]
public struct VendingMachineData
{

}

[System.Serializable]
public struct ElevatorData
{

}

[System.Serializable]
public struct LightData
{

}

[System.Serializable]
public struct TVdata
{

}

[System.Serializable]
public struct PhonographData
{

}