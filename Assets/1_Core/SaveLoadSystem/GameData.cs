using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
	//Important
	public string CurrentSceneNameSystem;
	public string CurrentDateAndTime;

	//Player 
	public string CurrentPlayerMovementStateType;
	public Vector3 PlayerPosition;
	public Quaternion PlayerRotation;

	//Camera
	public string CurrentPlayerCameraStateType;
	public float PlayerCameraDistanceY;
	public float PlayerCameraDistanceZ;
	public Quaternion CameraRotation;
	public bool IsCameraShoulderRight;

	//Resources
	public float PlayerHealth;
	public int HealingItems;
	public int PlayerMana;
	public int ManaReplenishItems;
	public int PlayerMoney;


	public List<LootObjectData> LootObjects_Scene_0_Test;
	public List<LootObjectData> LootObjects_Scene_1_StreetMain;

	public GameData()
	{
		//Important
		CurrentSceneNameSystem = "NEW_SceneTest";
		CurrentDateAndTime = DateTime.Now.ToString();

		//Player 
		CurrentPlayerMovementStateType = "PlayerIdle";
		PlayerPosition = new Vector3(2, 0, 4);
		PlayerRotation = new Quaternion(0, 0, 0, 0);

		//Camera
		CurrentPlayerCameraStateType = "ThirdPerson";
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		CameraRotation = new Quaternion(0, 0, 0, 0);
		IsCameraShoulderRight = true;

		//Resources
		PlayerHealth = 40;
		HealingItems = 3;
		PlayerMana = 15;
		ManaReplenishItems = 6;
		PlayerMoney = 200;


		LootObjects_Scene_0_Test = new List<LootObjectData>();
		LootObjects_Scene_1_StreetMain = new List<LootObjectData>();

	}
}

[System.Serializable]
public struct LootObjectData
{
	public int LootObjectIndex;
	public string LootObjectNameSystem;
	public bool WasLootObjectCollected;     
}