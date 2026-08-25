using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class InteractionObjectPickableNonThrowableAbstract : InteractionObjectPickableAbstract
{
	[Header("NonThrowable Movement")]
	[SerializeField] private bool _isMovementRestricted;
	[SerializeField] private float _movementSpeedPenaltyMultiplier;

	private PlayerMovementController _playerMovementController;
	private bool _isCreatedAsBody;
	public override void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		base.PickUpObject(isPickedUpByLoadSafeFile);

		//Debug.Log(_movementSpeedPenaltyMultiplier);
		//Debug.Log(_playerMovementController);
		//Debug.Log(_isMovementRestricted);

		if (_isMovementRestricted)
		{
			//Debug.Log("RSTRICT!");

			_gameController.RestrictPlayerMovementWhileCarryingNonThrowable();

			DecreaseTheMovementSpeed();
		}
	}

	public override void DropOffObject()
	{
		base.DropOffObject();

		if (_isMovementRestricted)
		{
			_gameController.UnrestrictPlayerMovementWhileCarryingNonThrowable();

			RestoreTheMovementSpeed();
		}
	}

	protected override void InitializePickable()
	{
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");

		//_isCreatedAsBody = true;
		//_isMovementRestricted = true;
		//_movementSpeedPenaltyMultiplier = 0.5f;

		if (_isMovementRestricted)
		{
			_playerMovementController.OnMovementSpeedChangedByStateMachine += DecreaseTheMovementSpeed;
		}
	}

	private void DecreaseTheMovementSpeed()
	{
		if (IsObjectPickedUp)
		{
			//Debug.Log(_movementSpeedPenaltyMultiplier);
			_playerMovementController.ChangePlayerMovementSpeed(_movementSpeedPenaltyMultiplier, false);
		}
	}

	private void RestoreTheMovementSpeed()
	{
		_playerMovementController.ChangePlayerMovementSpeed(1, false);
	}

	protected virtual void OnDestroy()
	{
		if (_isMovementRestricted)
		{
			_playerMovementController.OnMovementSpeedChangedByStateMachine -= DecreaseTheMovementSpeed;
		}
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.PickableObjectsData == null || !data.PickableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == GameplayObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsPickableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;

			PickUpObject(true);
			_playerInteractionController.PickUpObjectOnLoadData(gameObject);

			if (_playerInteractionController.CurrentIThrowable == null && _isMovementRestricted)
			{
				_gameController.RestrictPlayerMovementWhileCarryingNonThrowable();
			}
		}
		else
		{
			gameObject.transform.position = savedState.PickableObjectPosition;
			gameObject.transform.rotation = savedState.PickableObjectRotation;
		}

		yield return null;
	}
}