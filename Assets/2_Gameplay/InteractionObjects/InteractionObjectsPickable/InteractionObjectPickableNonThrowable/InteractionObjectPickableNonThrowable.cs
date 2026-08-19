using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableNonThrowable : InteractionObjectPickableAbstract
{
	public static InteractionObjectPickableNonThrowable CreateWithName(GameObject obj, string interactionItemNameSystem)
	{
		var component = obj.GetComponent<InteractionObjectPickableNonThrowable>();
		if (component == null)
		{
			component = obj.AddComponent<InteractionObjectPickableNonThrowable>();
		}
		component._interactionObjectNameSystem = interactionItemNameSystem;

		return component;
	}

	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;

	public override void PickUpObject()
	{
		base.PickUpObject();

		/*
		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdleCrouhcing ||
			_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingCrouching)
		{

		}

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdleStanding ||
		_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingStanding)
		{

		}
		*/

		HalfTheMovementSpeed();
	}

	public override void DropOffObject()
	{
		base.DropOffObject();

		RestoreTheMovementSpeed();
	}

	protected override void InitializePickable()
	{
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>("PlayerMovementStateMachineController");

		_playerMovementController.OnMovementSpeedChangedByStateMachine += HalfTheMovementSpeed;
	}

	private void HalfTheMovementSpeed()
	{
		if (IsObjectPickedUp)
		{
			_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed / 2, false);
		}
	}

	private void RestoreTheMovementSpeed()
	{
		_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed * 2, false);
	}

	protected virtual void OnDestroy()
	{
		_playerMovementController.OnMovementSpeedChangedByStateMachine -= HalfTheMovementSpeed;
	}

	public override void SaveData(ref GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		// Инициализируем словарь или список для текущей сцены, если их нет
		if (data.PickableObjectsData == null)
		{
			data.PickableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<PickableObjectData>>();
		}
		if (!data.PickableObjectsData.ContainsKey(currentScene))
		{
			data.PickableObjectsData[currentScene] = new List<PickableObjectData>();
		}

		var targetList = data.PickableObjectsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.PickableObjectIndex == PickableObjectIndex);

		var updatedItem = new PickableObjectData
		{
			PickableObjectIndex = PickableObjectIndex,
			PickableObjectNameSystem = InteractionObjectNameSystem,
			IsPickableObjectPickedUp = IsObjectPickedUp
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}
	}

	public override void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.PickableObjectsData == null || !data.PickableObjectsData.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == PickableObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) return;

		IsObjectPickedUp = savedState.IsPickableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			gameObject.SetActive(false);
		}
	}
}