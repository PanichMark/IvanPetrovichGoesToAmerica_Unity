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

	//Player Movement
	public Vector3 PlayerPosition;
	public Quaternion PlayerRotation;
	public string PlayerMovementStateType;

	//PlayerCamera
	public float PlayerCameraDistanceY;
	public float PlayerCameraDistanceZ;
	public Quaternion PlayerCameraRotation;
	public string PlayerCameraCurrentStateType;
	public bool IsPlayerCameraShoulderRight;

	//PlayerBehaviour
	public bool IsPlayerArmed;
	public bool WasPlayerArmed;

	//PlayerResources
	public float PlayerHealth;
	public int PlayerHealingItemsNumber;
	public float PlayerMana;
	public int PlayerManaReplenishItemsNumber;
	public int PlayerMoney;

	//PlayerWeapons
	public List<string> UnlockedWeapons;
	public List<WeaponRangedData> UnlockedRangedWeapons;
	public List<AmmoTypeData> PlayerAmmo;
	public string WeaponRightHand;
	public string WeaponLeftHand;

	//GameplayObjects
	[JsonProperty("NPCs")]
	public Dictionary<GameScenesGameplayDataEnum, List<NPCdata>> NPCsData;

	[JsonProperty("LootObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<LootObjectData>> LootObjectsData;

	[JsonProperty("ConsumableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<ConsumableObjectData>> ConsumableObjectsData;

	[JsonProperty("PickableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<PickableObjectData>> PickableObjectsData;

	[JsonProperty("Keys")]
	public Dictionary<GameScenesGameplayDataEnum, List<KeyData>> KeysData;

	[JsonProperty("OpenableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<OpenableObjectData>> OpenableObjectsData;

	[JsonProperty("Safes")]
	public Dictionary<GameScenesGameplayDataEnum, List<SafeData>> SafesData;

	[JsonProperty("VendingMachines")]
	public Dictionary<GameScenesGameplayDataEnum, List<VendingMachineData>> VendingMachinesData;

	[JsonProperty("Elevators")]
	public Dictionary<GameScenesGameplayDataEnum, List<ElevatorData>> ElevatorsData;

	[JsonProperty("Lights")]
	public Dictionary<GameScenesGameplayDataEnum, List<LightData>> LightsData;

	[JsonProperty("TVs")]
	public Dictionary<GameScenesGameplayDataEnum, List<TVdata>> TVsData;

	[JsonProperty("Phonographs")]
	public Dictionary<GameScenesGameplayDataEnum, List<PhonographData>> PhonographsData;

	[JsonProperty("BreakableObjects")]
	public Dictionary<GameScenesGameplayDataEnum, List<BreakableObjectData>> BreakableObjectsData;

	[JsonProperty("ElectricalPanels")]
	public Dictionary<GameScenesGameplayDataEnum, List<ElectricalPanelData>> ElectricalPanelsData;

	[JsonProperty("HintMessages")]
	public Dictionary<GameScenesGameplayDataEnum, List<HintMessageData>> HintMessagesData;

	public GameData()
	{
		//DateAndTime
		SafeFileDateAndTime = DateTime.Now.ToString();

		//Mission
		MissionNameSystem = GameMissionsNamesEnum.Mission_0_NothingEverHappensInThisCountry.ToString();

		//Scene
		SceneNameSystem = GameScenesSystemEnum.Scene_0_Test.ToString();

		//Player Movement
		PlayerPosition = new Vector3(0, 0, -5);
		PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdleStanding.ToString();

		//PlayerCamera
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		PlayerCameraRotation = new Quaternion(0, 0, 0, 0);
		PlayerCameraCurrentStateType = PlayerCameraStateTypes.FirstPerson.ToString();
		IsPlayerCameraShoulderRight = true;

		//PlayerBehaviour
		IsPlayerArmed = false;
		WasPlayerArmed = false;

		//PlayerResources
		PlayerHealth = 50;
		PlayerHealingItemsNumber = 1;
		PlayerMana = 50;
		PlayerManaReplenishItemsNumber = 1;
		PlayerMoney = 200;

		//PlayerWeapons
		UnlockedWeapons = new List<string>();
		UnlockedRangedWeapons = new List<WeaponRangedData>();
		PlayerAmmo = new List<AmmoTypeData>
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
		WeaponRightHand = null;
		WeaponLeftHand = null;

		InitializeGameplayObjectsData();
	}

	private void InitializeGameplayObjectsData()
	{
		NPCsData = CreateEmptyDictionary<NPCdata>();
		LootObjectsData = CreateEmptyDictionary<LootObjectData>();
		ConsumableObjectsData = CreateEmptyDictionary<ConsumableObjectData>();
		PickableObjectsData = CreateEmptyDictionary<PickableObjectData>();
		KeysData = CreateEmptyDictionary<KeyData>();
		OpenableObjectsData = CreateEmptyDictionary<OpenableObjectData>();
		SafesData = CreateEmptyDictionary<SafeData>();
		VendingMachinesData = CreateEmptyDictionary<VendingMachineData>();
		ElevatorsData = CreateEmptyDictionary<ElevatorData>();
		LightsData = CreateEmptyDictionary<LightData>();
		TVsData = CreateEmptyDictionary<TVdata>();
		PhonographsData = CreateEmptyDictionary<PhonographData>();
		BreakableObjectsData = CreateEmptyDictionary<BreakableObjectData>();
		ElectricalPanelsData = CreateEmptyDictionary<ElectricalPanelData>();
		HintMessagesData = CreateEmptyDictionary<HintMessageData>();
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
	public int NPCindex;
	public string NPCnameSystem;

	public Vector3 NPCposition;
	public Quaternion NPCrotation;
	public string NPCstate;
	public float NPChealth;
}

[System.Serializable]
public struct LootObjectData
{
	public int LootObjectIndex;
	public string LootObjectNameSystem;

	public bool IsLootObjectCollected;
}

[System.Serializable]
public struct ConsumableObjectData
{
	public int ConsumableObjectIndex;
	public string ConsumableObjectNameSystem;

	public bool IsConsumableObjectConsumed;
}

[System.Serializable]
public struct PickableObjectData
{
	public int PickableObjectIndex;
	public string PickableObjectNameSystem;

	public Vector3 PickableObjectPosition;
	public Quaternion PickableObjecRotation;
	public bool IsPickableObjectPickedUp;
	public bool IsPickableObjectDestroyed;
}

[System.Serializable]
public struct KeyData
{
	public string KeyNameSystem;

	public bool IsKeyCollected;
}

[System.Serializable]
public struct OpenableObjectData
{
	public int OpenableObjectIndex;
	public string OpenableObjectNameSystem;

	public bool IsOpenableObjectOpened;
	public bool IsOpenableObjectUnlocked;
	public bool IsOpenableObjectDestroyed;
}

[System.Serializable]
public struct SafeData
{
	public int SafeIndex;
	public string SafeNameSystem;

	public bool IsSafeOpened;
	public int SafeRotationSection_1_Position;
	public int SafeRotationSection_2_Position;
	public int SafeRotationSection_3_Position;
}

[System.Serializable]
public struct VendingMachineData
{
	public int VendingMachineIndex;
	public string VendingMachineNameSystem;
}

[System.Serializable]
public struct ElevatorData
{
	public int ElevatorIndex;
	public string ElevatorNameSystem;
}

[System.Serializable]
public struct LightData
{
	public int LightIndex;
	public string LightNameSystem;

	public bool IsLightTurnedOn;
}

[System.Serializable]
public struct TVdata
{
	public int TVindex;
	public string TVnameSystem;

	public bool IsTVturnedOn;
	public int TVchannel;
}

[System.Serializable]
public struct PhonographData
{
	public int PhonographIndex;
	public string PhonographNameSystem;

	public bool IsPhonographTurnedOn;
}

[System.Serializable]
public struct BreakableObjectData
{
	public int BreakableObjectIndex;
	public string BreakableObjectSystem;

	public bool IsBreakableObjectDestroyed;
	public float BreakableObjectHealth;
}

[System.Serializable]
public struct ElectricalPanelData
{
	public int ElectricalPanelIndex;
	public string ElectricalPanelSystem;

	public bool IsElectricalPanelOutOfService;
}

[System.Serializable]
public struct HintMessageData
{
	public int HintMessageIndex;
	public string HintMessageSystem;

	public bool WasHintMessageShown;
}