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

	//Resources
	public float PlayerHealth;
	public int HealingItems;
	public int PlayerMana;
	public int ManaReplenishItems;
	public int PlayerMoney;
	public List<AmmoTypeData> AmmoDictionary;

	//Weapons
	public List<string> UnlockedWeapons;
	public List<WeaponRangedTypeData> UnlockedRangedWeapons;
	public string WeaponRightHand;
	public string WeaponLeftHand;

	//LootObjects
	public List<LootObjectData> LootObjects_Scene_0_Test;
	public List<LootObjectData> LootObjects_Scene_1_Church;
	public List<LootObjectData> LootObjects_Scene_1_Street;
	public List<LootObjectData> LootObjects_Scene_1_RevenueHouse;
	public List<LootObjectData> LootObjects_Scene_1_InnerYard;

	//PickableObjects
	public List<PickableObjectData> PickableObjects_Scene_0_Test;
	public List<PickableObjectData> PickableObjects_Scene_1_Church;
	public List<PickableObjectData> PickableObjects_Scene_1_Street;
	public List<PickableObjectData> PickableObjects_Scene_1_RevenueHouse;
	public List<PickableObjectData> PickableObjects_Scene_1_InnerYard;

	public GameData()
	{
		//Important
		SafeFileDateAndTime = DateTime.Now.ToString();

		//Scene
		SceneNameSystem = GameScenesEnum.Scene_0_Test.ToString();

		//Player 
		PlayerPosition = new Vector3(0, 0, 0);
		PlayerRotation = new Quaternion(0, 0, 0, 0);
		PlayerMovementStateType = PlayerMovementStateTypes.PlayerIdle.ToString();

		//Camera
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		CameraRotation = new Quaternion(0, 0, 0, 0);
		CurrentPlayerCameraStateType = PlayerCameraStateTypes.ThirdPerson.ToString();
		IsCameraShoulderRight = true;

		//Resources
		PlayerHealth = 50;
		HealingItems = 9;
		PlayerMana = 50;
		ManaReplenishItems = 9;
		PlayerMoney = 100;
		AmmoDictionary = new List<AmmoTypeData>();

		//Weapons
		UnlockedWeapons = new List<string>();
		UnlockedRangedWeapons = new List<WeaponRangedTypeData>();
		WeaponRightHand = null;
		WeaponLeftHand = null;

		//LootObjects
		LootObjects_Scene_0_Test = new List<LootObjectData>();
		LootObjects_Scene_1_Church = new List<LootObjectData>();
		LootObjects_Scene_1_Street = new List<LootObjectData>();
		LootObjects_Scene_1_RevenueHouse = new List<LootObjectData>();
		LootObjects_Scene_1_InnerYard = new List<LootObjectData>();

		//PickableObjects
		PickableObjects_Scene_0_Test = new List<PickableObjectData>();
		PickableObjects_Scene_1_Church = new List<PickableObjectData>();
		PickableObjects_Scene_1_Street = new List<PickableObjectData>();
		PickableObjects_Scene_1_RevenueHouse = new List<PickableObjectData>();
		PickableObjects_Scene_1_InnerYard = new List<PickableObjectData>();
	}
}

[System.Serializable]
public struct LootObjectData
{
	public int LootObjectIndex;
	public string LootObjectNameSystem;
	public bool WasLootObjectCollected;     
}

[System.Serializable]
public struct PickableObjectData
{
	public int PickableObjectIndex;
	public string PickableObjectNameSystem;
	public bool WasPickableObjectPickedUp;
	public bool WasPickableObjectDestroyed;
}