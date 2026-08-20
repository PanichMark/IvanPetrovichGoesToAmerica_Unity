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

	[Header("NonThrowable Movement")]
	[SerializeField] private bool _isMovementRestricted;
	[SerializeField] private float _movementSpeedPenaltyMultiplier;

	private PlayerMovementController _playerMovementController;


	public override void PickUpObject()
	{
		base.PickUpObject();

		if (_isMovementRestricted)
		{
			_gameController.RestrictPlayerMovementWhileCarryingNonThrowable();

			HalfTheMovementSpeed();
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

		if (_isMovementRestricted)
		{
			_playerMovementController.OnMovementSpeedChangedByStateMachine += HalfTheMovementSpeed;
		}
	}

	private void HalfTheMovementSpeed()
	{
		if (IsObjectPickedUp)
		{
			_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed * _movementSpeedPenaltyMultiplier, false);
		}
	}

	private void RestoreTheMovementSpeed()
	{
		_playerMovementController.ChangePlayerMovementSpeed(_playerMovementController.PlayerMovementSpeed * _movementSpeedPenaltyMultiplier, false);
	}

	protected virtual void OnDestroy()
	{
		if (_isMovementRestricted)
		{
			_playerMovementController.OnMovementSpeedChangedByStateMachine -= HalfTheMovementSpeed;
		}
	}
}