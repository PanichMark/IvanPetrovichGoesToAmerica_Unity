using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonGameData
{
	//DateAndTime
	public string SafeFileDateAndTime;

	//Scene
	public GameScenesGameplayEnum Scene;

	//CoreObjects
	[JsonProperty("MissionData")]
	public MissionData MissionData = new MissionData();

	[JsonProperty("PlayerBehaviour")]
	public PlayerBehaviourData PlayerBehaviour = new PlayerBehaviourData();

	[JsonProperty("PlayerMisdeedsInScenes")]
	public Dictionary<GameScenesGameplayEnum, List<PlayerMisdeedsInScene>> PlayerMisdeedsInSceneData;

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
	public Dictionary<GameScenesGameplayEnum, List<NPCdata>> NPCsData;

	[JsonProperty("LootObjects")]
	public Dictionary<GameScenesGameplayEnum, List<LootObjectData>> LootObjectsData;

	[JsonProperty("NonThrowableUndestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<NonThrowableUndestructableObjectData>> NonThrowableUndestructableObjectsData;

	[JsonProperty("NonThrowableDestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<NonThrowableDestructableObjectData>> NonThrowableDestructableObjectsData;

	[JsonProperty("ThrowableUndestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<ThrowableUndestructableObjectData>> ThrowableUndestructableObjectsData;

	[JsonProperty("ThrowableDestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<ThrowableDestructableObjectData>> ThrowableDestructableObjectsData;

	[JsonProperty("OpenableUndestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<OpenableUndestructableObjectData>> OpenableUndestructableObjectsData;

	[JsonProperty("OpenableDestructableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<OpenableDestructableObjectData>> OpenableDestructableObjectsData;

	[JsonProperty("SafesUndestructable")]
	public Dictionary<GameScenesGameplayEnum, List<SafeUndestructableData>> SafesUndestructableData;

	[JsonProperty("SafesDestructable")]
	public Dictionary<GameScenesGameplayEnum, List<SafeDestructableData>> SafesDestructableData;

	[JsonProperty("VendingMachines")]
	public Dictionary<GameScenesGameplayEnum, List<VendingMachineData>> VendingMachinesData;

	[JsonProperty("Elevators")]
	public Dictionary<GameScenesGameplayEnum, List<ElevatorData>> ElevatorsData;

	[JsonProperty("Lights")]
	public Dictionary<GameScenesGameplayEnum, List<LightData>> LightsData;

	[JsonProperty("TVs")]
	public Dictionary<GameScenesGameplayEnum, List<TVdata>> TVsData;

	[JsonProperty("Phonographs")]
	public Dictionary<GameScenesGameplayEnum, List<PhonographData>> PhonographsData;

	[JsonProperty("BreakableObjects")]
	public Dictionary<GameScenesGameplayEnum, List<BreakableObjectData>> BreakableObjectsData;

	[JsonProperty("ElectricalPanels")]
	public Dictionary<GameScenesGameplayEnum, List<ElectricalPanelData>> ElectricalPanelsData;

	[JsonProperty("HintMessages")]
	public Dictionary<GameScenesGameplayEnum, List<HintMessageData>> HintMessagesData;

	public JsonGameData()
	{
		//DateAndTime
		SafeFileDateAndTime = DateTime.Now.ToString();

		//Scene
		Scene = GameScenesGameplayEnum.Scene_0_Test;

		//Mission
		MissionData.Mission = 0.1f;
		MissionData.MissionStep = 0;

		//PlayerBehaviour
		PlayerBehaviour.IsPlayerArmed = false;
		PlayerBehaviour.WasPlayerArmed = false;

		// PlayerMovement
		PlayerMovement.PlayerPosition = new Vector3(0, 0, -5);
		PlayerMovement.PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovement.PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdleStanding.ToString();

		//PlayerCamera
		PlayerCamera.PLayerCameraDistanceY = -1.75f;
		PlayerCamera.PlayerCameraDistanceZ = 3.25f;
		PlayerCamera.PlayerCameraRotation = new Quaternion(0, 0, 0, 0);
		PlayerCamera.PlayerCameraStateType = PlayerCameraStateTypes.MainMenu.ToString();
		PlayerCamera.IsPlayerCameraShoulderRight = true;

		//PlayerResources
		PlayerResources.PlayerHealth = 80;
		PlayerResources.PlayerHealingItemsNumber = 0;
		PlayerResources.PlayerMana = 80;
		PlayerResources.PlayerManaReplenishItemsNumber = 0;
		PlayerResources.PlayerMoney = 0;

		//PlayerWeapons
		PlayerWeapons.UnlockedPlayerWeapons = new List<string>();
		PlayerWeapons.UnlockedPlayerRangedWeapons = new List<WeaponRangedData>();
		PlayerWeapons.PlayerAmmo = new List<AmmoTypeData>
		{
			new AmmoTypeData
			{
				AmmoType = AmmoTypes.Ammo9mm,
				AmmoReserve = 25
			},
			new AmmoTypeData
			{
				AmmoType = AmmoTypes.Ammo12gauge,
				AmmoReserve = 10
			},
			new AmmoTypeData
			{
				AmmoType = AmmoTypes.AmmoTranquilizerDart,
				AmmoReserve = 5
			}
		};
		PlayerWeapons.PlayerWeaponRightHand = null;
		PlayerWeapons.PlayerWeaponLeftHand = null;

		InitializeGameplayObjectsData();
	}

	private void InitializeGameplayObjectsData()
	{
		NPCsData = CreateEmptyDictionary<NPCdata>();

		LootObjectsData = CreateEmptyDictionary<LootObjectData>();

		NonThrowableUndestructableObjectsData = CreateEmptyDictionary<NonThrowableUndestructableObjectData>();
		NonThrowableDestructableObjectsData = CreateEmptyDictionary<NonThrowableDestructableObjectData>();
		ThrowableUndestructableObjectsData = CreateEmptyDictionary<ThrowableUndestructableObjectData>();
		ThrowableDestructableObjectsData = CreateEmptyDictionary<ThrowableDestructableObjectData>();

		OpenableUndestructableObjectsData = CreateEmptyDictionary<OpenableUndestructableObjectData>();
		OpenableDestructableObjectsData = CreateEmptyDictionary<OpenableDestructableObjectData>();

		SafesUndestructableData = CreateEmptyDictionary<SafeUndestructableData>();
		SafesDestructableData = CreateEmptyDictionary<SafeDestructableData>();

		VendingMachinesData = CreateEmptyDictionary<VendingMachineData>();
		ElevatorsData = CreateEmptyDictionary<ElevatorData>();
		LightsData = CreateEmptyDictionary<LightData>();
		TVsData = CreateEmptyDictionary<TVdata>();
		PhonographsData = CreateEmptyDictionary<PhonographData>();
		BreakableObjectsData = CreateEmptyDictionary<BreakableObjectData>();
		ElectricalPanelsData = CreateEmptyDictionary<ElectricalPanelData>();
		HintMessagesData = CreateEmptyDictionary<HintMessageData>();
	}

	private Dictionary<GameScenesGameplayEnum, List<T>> CreateEmptyDictionary<T>()
	{
		var dict = new Dictionary<GameScenesGameplayEnum, List<T>>();
		foreach (GameScenesGameplayEnum scene in Enum.GetValues(typeof(GameScenesGameplayEnum)))
		{
			dict[scene] = new List<T>();
		}
		return dict;
	}
}

[System.Serializable]
public class MissionData
{
	public float Mission;
	public int MissionStep;
}

[System.Serializable]
public class PlayerBehaviourData
{
	public bool IsPlayerArmed;
	public bool WasPlayerArmed;

	public int TimesPlayerSpottedGameTotal;
	public int PeopleKilledGameTotal;
	public GameCityState CityState;
}

[System.Serializable]
public struct PlayerMisdeedsInScene
{
	public bool WasPlayerSpottedInThisScene;
	public bool WerePeopleKilledInThisScene;
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
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion PlayerCameraRotation;
	public string PlayerCameraStateType;
	public bool IsPlayerCameraShoulderRight;
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
	public List<string> UnlockedPlayerWeapons;
	public List<WeaponRangedData> UnlockedPlayerRangedWeapons;
	public List<AmmoTypeData> PlayerAmmo;
	public string PlayerWeaponRightHand;
	public string PlayerWeaponLeftHand;
}

[System.Serializable]
public struct NPCdata
{
	public int NPCindex;
	public string NPCnameSystem;

	[JsonConverter(typeof(JsonFileDataHandler.Vector3Converter))]
	public Vector3 NPCposition;
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion NPCrotation;
	public int NPCnextAnchorPoint;
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
public struct NonThrowableUndestructableObjectData
{
	public int NonThrowableUndestructableObjectIndex;
	public string NonThrowableUndestructableObjectNameSystem;

	[JsonConverter(typeof(JsonFileDataHandler.Vector3Converter))]
	public Vector3 NonThrowableUndestructableObjectPosition;
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion NonThrowableUndestructableObjectRotation;
	public bool IsNonThrowableUndestructableObjectPickedUp;
}

[System.Serializable]
public struct NonThrowableDestructableObjectData
{
	public int NonThrowableDestructableObjectIndex;
	public string NonThrowableDestructableObjectNameSystem;

	[JsonConverter(typeof(JsonFileDataHandler.Vector3Converter))]
	public Vector3 NonThrowableDestructableObjectPosition;
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion NonThrowableDestructableObjectRotation;
	public bool IsNonThrowableDestructableObjectPickedUp;
	public float NonThrowableDestructableObjectHealth;
}

[System.Serializable]
public struct ThrowableUndestructableObjectData
{
	public int ThrowableUndestructableObjectIndex;
	public string ThrowableUndestructableObjectNameSystem;

	[JsonConverter(typeof(JsonFileDataHandler.Vector3Converter))]
	public Vector3 ThrowableUndestructableObjectPosition;
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion ThrowableUndestructableObjectRotation;
	public bool IsThrowableUndestructableObjectPickedUp;
	public bool WasThrowableUndestructableObjectThrown;
}

[System.Serializable]
public struct ThrowableDestructableObjectData
{
	public int ThrowableDestructableObjectIndex;
	public string ThrowableDestructableObjectNameSystem;

	[JsonConverter(typeof(JsonFileDataHandler.Vector3Converter))]
	public Vector3 ThrowableDestructableObjectPosition;
	[JsonConverter(typeof(JsonFileDataHandler.QuaternionConverter))]
	public Quaternion ThrowableDestructableObjectRotation;
	public bool IsThrowableDestructableObjectPickedUp;
	public bool WasThrowableDestructableObjectThrown;
	public float ThrowableDestructableObjectHealth;
}

[System.Serializable]
public struct OpenableUndestructableObjectData
{
	public int OpenableUndestructableObjectIndex;
	public string OpenableUndestructableObjectNameSystem;

	public bool IsOpenableUndestructableObjectOpened;
	public bool IsOpenableUndestructableObjectUnlocked;
}

[System.Serializable]
public struct OpenableDestructableObjectData
{
	public int OpenableDestructableObjectIndex;
	public string OpenableDestructableObjectNameSystem;

	public bool IsOpenableDestructableObjectOpened;
	public bool IsOpenableDestructableObjectUnlocked;
	public float OpenableDestructableObjectHealth;
}

[System.Serializable]
public struct SafeUndestructableData
{
	public int SafeUndestructableIndex;
	public string SafeUndestructableNameSystem;

	public bool IsSafeUndestructableOpened;
	public int SafeUndestructableRotationSection_1_Position;
	public int SafeUndestructableRotationSection_2_Position;
	public int SafeUndestructableRotationSection_3_Position;
}

[System.Serializable]
public struct SafeDestructableData
{
	public int SafeDestructableIndex;
	public string SafeDestructableNameSystem;

	public bool IsSafeDestructableOpened;
	public bool IsSafeDestructableDestroyed;
	public int SafeDestructableRotationSection_1_Position;
	public int SafeDestructableRotationSection_2_Position;
	public int SafeDestructableRotationSection_3_Position;
}

[System.Serializable]
public struct VendingMachineData
{
	public int VendingMachineIndex;
	public string VendingMachineNameSystem;

	public float VendingMachineHealth;
	public int VendingMachineSpawnedGoods;
}

[System.Serializable]
public struct ElevatorData
{
	public int ElevatorIndex;
	public string ElevatorNameSystem;

	public bool IsElevatorUp;
	public bool IsElevatorPoweredOn;
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