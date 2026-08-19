using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class GameData
{
	//DateAndTime
	public string SafeFileDateAndTime;

	//Mission
	public string MissionNameSystem;

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

	[JsonProperty("NPCs")]
	public Dictionary<GameScenesGameplayDataEnum, List<NPCdata>> NPCdataByScene;

	[JsonProperty("LootObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<LootObjectData>> LootObjectsByScene;

	[JsonProperty("ConsumableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<ConsumableObjectData>> ConsumableObjectsByScene;

	[JsonProperty("PickableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<PickableObjectData>> PickableObjectsByScene;

	[JsonProperty("Keys")]
	public Dictionary<GameScenesGameplayDataEnum, List<KeyData>> KeysByScene;

	[JsonProperty("OpenableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<OpenableObjectData>> OpenableObjectsByScene;

	[JsonProperty("Safes")]
	public Dictionary<GameScenesGameplayDataEnum, List<SafeData>> SafesByScene;

	[JsonProperty("VendingMachines")]
	public Dictionary<GameScenesGameplayDataEnum, List<VendingMachineData>> VendingMachinesByScene;

	[JsonProperty("Elevators")]
	public Dictionary<GameScenesGameplayDataEnum, List<ElevatorData>> ElevatorsByScene;

	[JsonProperty("Lights")]
	public Dictionary<GameScenesGameplayDataEnum, List<LightData>> LightsByScene;

	[JsonProperty("TVs")]
	public Dictionary<GameScenesGameplayDataEnum, List<TVdata>> TVsByScene;

	[JsonProperty("Phonographs")]
	public Dictionary<GameScenesGameplayDataEnum, List<PhonographData>> PhonographsByScene;

	public GameData()
	{
		//DateAndTime
		SafeFileDateAndTime = DateTime.Now.ToString();

		//Mission
		MissionNameSystem = GameMissionsNamesEnum.Mission_0_NothingEverHappensInThisCountry.ToString();

		//Scene
		SceneNameSystem = GameScenesSystemEnum.Scene_0_Test.ToString();

		//Player 
		PlayerPosition = new Vector3(0, 0, -5);
		PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdleStanding.ToString();

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
		HealingItems = 1;
		PlayerMana = 50;
		ManaReplenishItems = 1;
		PlayerMoney = 200;
		AmmoDictionary = new List<AmmoTypeData>
		{
			new AmmoTypeData
			{
				AmmoTypeSystem = AmmoTypes.Ammo9mm,
				AmmoTypeJson = AmmoTypes.Ammo9mm.ToString(),
				AmmoMax = 999,
				AmmoReserve = 25
			},
			new AmmoTypeData
			{
				AmmoTypeSystem = AmmoTypes.Ammo12gauge,
				AmmoTypeJson = AmmoTypes.Ammo12gauge.ToString(),
				AmmoMax = 999,
				AmmoReserve = 10
			}
		};

		//Weapons
		UnlockedWeapons = new List<string>();
		UnlockedRangedWeapons = new List<WeaponRangedData>();
		WeaponRightHand = null;
		WeaponLeftHand = null;

		InitializeGameplayObjectsData();
	}

	private void InitializeGameplayObjectsData()
	{
		NPCdataByScene = CreateEmptyDictionary<NPCdata>();
		LootObjectsByScene = CreateEmptyDictionary<LootObjectData>();
		ConsumableObjectsByScene = CreateEmptyDictionary<ConsumableObjectData>();
		PickableObjectsByScene = CreateEmptyDictionary<PickableObjectData>();
		KeysByScene = CreateEmptyDictionary<KeyData>();
		OpenableObjectsByScene = CreateEmptyDictionary<OpenableObjectData>();
		SafesByScene = CreateEmptyDictionary<SafeData>();
		VendingMachinesByScene = CreateEmptyDictionary<VendingMachineData>();
		ElevatorsByScene = CreateEmptyDictionary<ElevatorData>();
		LightsByScene = CreateEmptyDictionary<LightData>();
		TVsByScene = CreateEmptyDictionary<TVdata>();
		PhonographsByScene = CreateEmptyDictionary<PhonographData>();
	}

	private Dictionary<GameScenesGameplayDataEnum, List<T>> CreateEmptyDictionary<T>()
	{
		var dict = new Dictionary<GameScenesGameplayDataEnum, List<T>>();
		foreach (GameScenesGameplayDataEnum scene in Enum.GetValues(typeof(GameScenesGameplayDataEnum)))
		{
			dict[scene] = new List<T>();
		}
		return dict;
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