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

	[JsonProperty("PlayerBehaviour")]
	public PlayerBehaviourData PlayerBehaviour = new PlayerBehaviourData();

	[JsonProperty("PlayerMovement")]
	public PlayerMovementData PlayerMovement = new PlayerMovementData();

	[JsonProperty("PlayerCamera")]
	public PlayerCameraData PlayerCamera = new PlayerCameraData();

	[JsonProperty("PlayerResources")]
	public PlayerResourcesData PlayerResources = new PlayerResourcesData();

	[JsonProperty("PlayerWeapons")]
	public PlayerWeaponsData PlayerWeapons = new PlayerWeaponsData();

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

		//PlayerBehaviour
		PlayerBehaviour.IsArmed = false;
		PlayerBehaviour.WasArmed = false;

		// PlayerMovement
		PlayerMovement.PlayerPosition = new Vector3(0, 0, -5);
		PlayerMovement.PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovement.PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdleStanding.ToString();

		//PlayerCamera
		PlayerCamera.PLayerCameraDistanceY = -1.75f;
		PlayerCamera.PlayerCameraDistanceZ = 3.25f;
		PlayerCamera.PlayerCameraRotation = new Quaternion(0, 0, 0, 0);
		PlayerCamera.PlayerCameraStateType = PlayerCameraStateTypes.FirstPerson.ToString();
		PlayerCamera.PlayerCameraIsShoulderRight = true;

		//PlayerResources
		PlayerResources.PlayerHealth = 50;
		PlayerResources.PlayerHealingItemsNumber = 1;
		PlayerResources.PlayerMana = 50;
		PlayerResources.PlayerManaReplenishItemsNumber = 1;
		PlayerResources.PlayerMoney = 200;

		//PlayerWeapons
		PlayerWeapons.UnlockedWeapons = new List<string>();
		PlayerWeapons.UnlockedRangedWeapons = new List<WeaponRangedData>();
		PlayerWeapons.Ammo = new List<AmmoTypeData>
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
			},
			new AmmoTypeData
			{
				AmmoTypeSystem = AmmoTypes.AmmoTranquilizerDart,
				AmmoTypeJson = AmmoTypes.AmmoTranquilizerDart.ToString(),
				AmmoMax = 999,
				AmmoReserve = 5
			}
		};
		PlayerWeapons.WeaponRightHand = null;
		PlayerWeapons.WeaponLeftHand = null;

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
public class PlayerBehaviourData
{
	public bool IsArmed;
	public bool WasArmed;
}

[System.Serializable]
public class PlayerMovementData
{
	public Vector3 PlayerPosition;
	public Quaternion PlayerRotation;
	public string PlayerMovementStateType;
}

[System.Serializable]
public class PlayerCameraData
{
	public float PLayerCameraDistanceY;
	public float PlayerCameraDistanceZ;
	[JsonConverter(typeof(FileDataHandler.QuaternionConverter))]
	public Quaternion PlayerCameraRotation;
	public string PlayerCameraStateType;
	public bool PlayerCameraIsShoulderRight;
}

[System.Serializable]
public class PlayerResourcesData
{
	public float PlayerHealth;
	public int PlayerHealingItemsNumber;
	public float PlayerMana;
	public int PlayerManaReplenishItemsNumber;
	public int PlayerMoney;
}

[System.Serializable]
public class PlayerWeaponsData
{
	public List<string> UnlockedWeapons;
	public List<WeaponRangedData> UnlockedRangedWeapons;
	public List<AmmoTypeData> Ammo;
	public string WeaponRightHand;
	public string WeaponLeftHand;
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