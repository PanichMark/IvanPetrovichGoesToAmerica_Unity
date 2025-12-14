using System;
using UnityEngine;

[System.Serializable]
public class GameData
{
	//CurrentScene
	public string CurrentSceneNameSystem;
	public string CurrentSceneNameUI;

	//Date&Time
	public string CurrentDateAndTime;

	//Player Movement
	public string CurrentPlayerMovementStateType;
	public Vector3 PlayerPosition;
	public Quaternion PlayerRotation;


	//Camera
	public string CurrentPlayerCameraStateType;
	public float PlayerCameraDistanceY;
	public float PlayerCameraDistanceZ;
	public Quaternion CameraRotation;
	public bool IsCameraShoulderRight;

	//PlayerMoney
	public int PlayerMoney;

	//PlayerHealth
	public int PlayerHealth;
	public int HealingItems;

	//PlayerMana
	public int PlayerMana;
	public int ManaReplenishItems;

	//Collectables
	public LootItemsData[] LootItemsSceneTEST;
	public LootItemsData[] LootItemsScene1;

	public GameData()
	{
		CurrentSceneNameSystem = "SceneTEST";
		CurrentSceneNameUI = "Тестовая сцена";

		CurrentDateAndTime = DateTime.Now.ToString();

		CurrentPlayerMovementStateType = "PlayerIdle";
		PlayerPosition = new Vector3(2, 0, 4);
		PlayerRotation = new Quaternion(0, 0, 0, 0);

		CurrentPlayerCameraStateType = "ThirdPerson";
		PlayerCameraDistanceY = -1.75f;
		PlayerCameraDistanceZ = 3.25f;
		CameraRotation = new Quaternion(0, 0, 0, 0);
		IsCameraShoulderRight = true;

		PlayerMoney = 200;

		PlayerHealth = 40;
		HealingItems = 3;

		PlayerMana = 15;
		ManaReplenishItems = 6;

		LootItemsSceneTEST = new LootItemsData[20];
		LootItemsScene1 = new LootItemsData[20];

	}
}

[System.Serializable]
public struct LootItemsData
{
	public int LootItemIndex;
	public string LootItemName;
	public bool WasLootItemCollected;     
		
}

