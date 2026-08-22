using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableNonThrowable : InteractionObjectPickableAbstract
{
	public static InteractionObjectPickableNonThrowable CreateWithName(GameObject obj, string interactionItemNameSystem, InteractionObjectPickableData pickableBodyData)
	{
		var component = obj.GetComponent<InteractionObjectPickableNonThrowable>();
		if (component == null)
		{
			component = obj.AddComponent<InteractionObjectPickableNonThrowable>();
		}
		Debug.Log(component);
	
		component.SetUpPickableBody(interactionItemNameSystem, pickableBodyData);

		return component;
	}

	protected void SetUpPickableBody(string interactionObjectNameSystem, InteractionObjectPickableData pickableBodyData)
	{
		_interactionObjectPickableType = pickableBodyData;
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(interactionObjectNameSystem);


		Collider = gameObject.AddComponent<BoxCollider>();

		BoxCollider box = (BoxCollider)Collider;
		box.center = new Vector3(0f, 0.5f, 0f);
		box.size = new Vector3(0.7f, 1f, 0.7f);

		var rigidbody = GetComponent<Rigidbody>();
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		InitializePickable();
	}

	[Header("NonThrowable Movement")]
	[SerializeField] private bool _isMovementRestricted;
	[SerializeField] private float _movementSpeedPenaltyMultiplier;

	private PlayerMovementController _playerMovementController;

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

		_isCreatedAsBody = true;
		_canBeBroken = false;
		_isMovementRestricted = true;
		_movementSpeedPenaltyMultiplier = 0.5f;

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

	public override void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.PickableObjectsData == null || !data.PickableObjectsData.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == PickableObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) return;

		IsObjectPickedUp = savedState.IsPickableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;

			PickUpObject(true);
			//Debug.Log(_playerInteractionController);
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

	}
}