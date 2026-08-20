using UnityEngine;

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
	private GameController _gameController;


	public override void PickUpObject()
	{
		base.PickUpObject();

		_gameController.RestrictPlayerMovementWhileCarryingNonThrowable();

		HalfTheMovementSpeed();
	}

	public override void DropOffObject()
	{
		base.DropOffObject();

		_gameController.UnrestrictPlayerMovementWhileCarryingNonThrowable();

		RestoreTheMovementSpeed();
	}

	protected override void InitializePickable()
	{
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_gameController = ServiceLocator.Resolve<GameController>("GameController");

		_playerMovementController.OnMovementSpeedChangedByStateMachine += HalfTheMovementSpeed;
	}

	private void HalfTheMovementSpeed()
	{
		if (IsObjectPickedUp)
		{
			_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed / 1.75f, false);
		}
	}

	private void RestoreTheMovementSpeed()
	{
		_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed * 1.75f, false);
	}

	protected virtual void OnDestroy()
	{
		_playerMovementController.OnMovementSpeedChangedByStateMachine -= HalfTheMovementSpeed;
	}
}