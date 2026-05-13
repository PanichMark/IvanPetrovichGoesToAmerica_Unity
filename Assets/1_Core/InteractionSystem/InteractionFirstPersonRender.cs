using UnityEngine;

public class InteractionFirstPersonRender : MonoBehaviour
{
	private PlayerCameraStateMachineController _playerCameraStateMachine;
	private InteractionController _interactionController;
	private GameSceneManager _gameSceneManager;

	private bool _areBothArmsBusy;
	private bool _isRightArmBusy;
	private bool _isInitialized = false;

	private GameObject _PlayerFirstPersonHandRight;
	private GameObject _PlayerFirstPersonHandLeft;
	private GameObject _PlayerThirdPersonRightHand;
	private GameObject _PlayerThirdPersonLeftHand;

	public void Initialize(
		GameSceneManager gameSceneManager,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject playerFirstPersonHandRight,
		GameObject playerFirstPersonHandLeft,
		GameObject playerHandRightParent,
		GameObject playerHandLeftParent,
		InteractionController interactionController)
	{
		_gameSceneManager = gameSceneManager;
		_playerCameraStateMachine = playerCameraStateMachineController;

		_PlayerFirstPersonHandRight = playerFirstPersonHandRight;
		_PlayerFirstPersonHandLeft = playerFirstPersonHandLeft;
		_interactionController = interactionController;
		_PlayerThirdPersonRightHand = playerHandRightParent;
		_PlayerThirdPersonLeftHand = playerHandLeftParent;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(this._PlayerFirstPersonHandRight);
		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(this._PlayerFirstPersonHandLeft);

		_interactionController.OnPickUpNonThrowable += () => _areBothArmsBusy = true;
		_interactionController.OnPickUpThrowable += () => _isRightArmBusy = true;
		_interactionController.OnGetRidOfPickable += () =>
		{
			_areBothArmsBusy = false;
			_isRightArmBusy = false;
		};

		_isInitialized = true;
		Debug.Log("InteractionFirstPersonRender initialized!");
	}

	void Update()
	{
		if (!_isInitialized)
			return;

		if (_playerCameraStateMachine.CurrentPlayerCameraStateType == "FirstPerson")
		{
			if (_areBothArmsBusy)
			{
				ShowBodyPart(_PlayerThirdPersonRightHand);
				HideFirstPersonHand(_PlayerFirstPersonHandRight);
				ShowBodyPart(_PlayerThirdPersonLeftHand);
				HideFirstPersonHand(_PlayerFirstPersonHandLeft);
			}
			else if (_isRightArmBusy)
			{
				HideBodyPart(_PlayerThirdPersonRightHand);
				ShowFirstPersonHand(_PlayerFirstPersonHandRight);
			}
			else
			{
				ShowBodyPart(_PlayerThirdPersonRightHand);
				HideFirstPersonHand(_PlayerFirstPersonHandRight);
				ShowBodyPart(_PlayerThirdPersonLeftHand);
				HideFirstPersonHand(_PlayerFirstPersonHandLeft);
			}
		}

		if (_playerCameraStateMachine.CurrentPlayerCameraStateType == "ThirdPerson")
		{
			if (_areBothArmsBusy)
			{
				ShowBodyPart(_PlayerThirdPersonRightHand);
				HideFirstPersonHand(_PlayerFirstPersonHandRight);
				ShowBodyPart(_PlayerThirdPersonLeftHand);
				HideFirstPersonHand(_PlayerFirstPersonHandLeft);
			}
			else if (_isRightArmBusy)
			{
				ShowBodyPart(_PlayerThirdPersonRightHand);
				HideFirstPersonHand(_PlayerFirstPersonHandRight);
			}
			else
			{
				ShowBodyPart(_PlayerThirdPersonRightHand);
				HideFirstPersonHand(_PlayerFirstPersonHandRight);
				ShowBodyPart(_PlayerThirdPersonLeftHand);
				HideFirstPersonHand(_PlayerFirstPersonHandLeft);
			}
		}
	}

	public void ShowBodyPart(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		}
	}

	public void HideBodyPart(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
	}

	public void ShowFirstPersonHand(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
				renderer.enabled = true;
		}
	}

	public void HideFirstPersonHand(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
				renderer.enabled = false;
		}
	}
}