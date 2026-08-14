using UnityEngine;

public class InteractionFirstPersonRenderer : MonoBehaviour
{
	private GameObject _currentThrowableObject;
	private GameObject _playerFirstPersonHandRight;
	private GameObject _playerThirdPersonHandRight;
	private PlayerCameraStateMachineController _playerCameraStateMachine;
	private GameScenesManager _gameSceneManager;
	private InteractionController _interactionController;

	public void Initialize(
		GameScenesManager gameSceneManager,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		InteractionController interactionController,
		GameObject playerFirstPersonHandRight,
		GameObject playerThirdPersonHandRight)
	{
		_gameSceneManager = gameSceneManager;
		_playerCameraStateMachine = playerCameraStateMachineController;
		_interactionController = interactionController;
		_playerFirstPersonHandRight = playerFirstPersonHandRight;
		_playerThirdPersonHandRight = playerThirdPersonHandRight;

		_playerCameraStateMachine.OnFirstPersonCameraState += UpdateVisibilityForFirstPerson;
		_playerCameraStateMachine.OnThirdPersonCameraState += UpdateVisibilityForThirdPerson;
		//_gameSceneManager.OnBeginLoadingMainMenuScene += HideAll;

		_interactionController.OnPickUpThrowable += (InteractionObjectsPickableTypes pickableType) => { OnObjectPickedUp(); };
		_interactionController.OnGetRidOfThrowable += OnObjectDropped;
	}

	public void OnObjectPickedUp()
	{
		_currentThrowableObject = _interactionController.CurrentPickableObject;

		ApplyCurrentCameraState();
	}

	public void OnObjectDropped()
	{
		_currentThrowableObject = null;

		HideFirstPersonRightHand();
		ShowThirdPersonRightHand();
	}

	private void ApplyCurrentCameraState()
	{
		if (_playerCameraStateMachine.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			UpdateVisibilityForFirstPerson();
		}
		else
		{
			UpdateVisibilityForThirdPerson();
		}
	}

	private void UpdateVisibilityForFirstPerson()
	{
		if (_currentThrowableObject != null)
		{
			ShowFirstPersonRightHand();
			HideThirdPersonRightHand();
		}
	}

	private void UpdateVisibilityForThirdPerson()
	{
		if (_currentThrowableObject != null)
		{
			HideFirstPersonRightHand();
			ShowThirdPersonRightHand();
		}
	}

	private void ShowFirstPersonRightHand()
	{
		foreach (var renderer in _playerFirstPersonHandRight.GetComponentsInChildren<Renderer>(true))
		{
			renderer.enabled = true;
		}
	}

	private void HideFirstPersonRightHand()
	{
		foreach (var renderer in _playerFirstPersonHandRight.GetComponentsInChildren<Renderer>(true))
		{
			renderer.enabled = false;
		}
	}

	private void ShowThirdPersonRightHand()
	{
		foreach (var renderer in _playerThirdPersonHandRight.GetComponentsInChildren<Renderer>(true))
		{
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		}
	}

	private void HideThirdPersonRightHand()
	{
		foreach (var renderer in _playerThirdPersonHandRight.GetComponentsInChildren<Renderer>(true))
		{
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
	}

	private void HideAll()
	{
		//ShowRightHand(_playerFirstPersonHandRight, false);
		//ShowThirdPersonHand(_playerThirdPersonHandRight, false);
	}
}