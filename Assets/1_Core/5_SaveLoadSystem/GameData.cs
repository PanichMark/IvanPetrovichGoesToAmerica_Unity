using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
	//DateAndTime
	public string SafeFileDateAndTime;

	//Scene
	public string Scene;

	[JsonProperty("MissionData")]
	public MissionData MissionData = new MissionData();

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

		//Scene
		Scene = GameScenesSystemEnum.Scene_0_Test.ToString();

		//Mission
		MissionData.Mission = GameMissionsNamesEnum.Mission_0_NothingEverHappensInThisCountry.ToString();
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
		PlayerCamera.PlayerCameraStateType = PlayerCameraStateTypes.FirstPerson.ToString();
		PlayerCamera.IsPlayerCameraShoulderRight = true;

		//PlayerResources
		PlayerResources.PlayerHealth = 50;
		PlayerResources.PlayerHealingItemsNumber = 1;
		PlayerResources.PlayerMana = 50;
		PlayerResources.PlayerManaReplenishItemsNumber = 1;
		PlayerResources.PlayerMoney = 200;

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
public class MissionData
{
	public string Mission;
	public int MissionStep;
}

[System.Serializable]
public class PlayerBehaviourData
{
	public bool IsPlayerArmed;
	public bool WasPlayerArmed;
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

	public Vector3 NPCposition;
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
	public Quaternion PickableObjectRotation;
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

	public float VendingMachineElectroHealth;
	public int VendingMachineSpawnedGoods;
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