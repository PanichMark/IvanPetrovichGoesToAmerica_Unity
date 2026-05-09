using UnityEngine;

public class InteractionFirstPersonRender : MonoBehaviour
{
	private PlayerCameraController playerCamera;
	private InteractionController interactionController;
	private GameSceneManager gameSceneManager;

	private bool AreBothArmsBusy;
	private bool IsRightArmBusy;
	private bool _isInitialized = false;

	private GameObject PlayerFirstPersonHandRight;
	private GameObject PlayerFirstPersonHandLeft;
	private GameObject PlayerHandRightParent;
	private GameObject PlayerHandLeftParent;

	public void Initialize(
		GameSceneManager gameSceneManager,
		PlayerCameraController playerCameraController,
		GameObject playerFirstPersonHandRight,
		GameObject playerFirstPersonHandLeft,
		GameObject playerHandRightParent,
		GameObject playerHandLeftParent,
		InteractionController interactionController)
	{
		this.gameSceneManager = gameSceneManager;
		this.playerCamera = playerCameraController;

		this.PlayerFirstPersonHandRight = playerFirstPersonHandRight;
		this.PlayerFirstPersonHandLeft = playerFirstPersonHandLeft;
		this.interactionController = interactionController;
		this.PlayerHandRightParent = playerHandRightParent;
		this.PlayerHandLeftParent = playerHandLeftParent;

		this.gameSceneManager.OnBeginLoadMainMenuScene += () => HideFirstPersonHand(this.PlayerFirstPersonHandRight);
		this.gameSceneManager.OnBeginLoadMainMenuScene += () => HideFirstPersonHand(this.PlayerFirstPersonHandLeft);

		this.interactionController.OnPickUpNonThrowable += () => AreBothArmsBusy = true;
		this.interactionController.OnPickUpThrowable += () => IsRightArmBusy = true;
		this.interactionController.OnGetRidOfPickable += () =>
		{
			AreBothArmsBusy = false;
			IsRightArmBusy = false;
		};

		_isInitialized = true;
		Debug.Log("InteractionFirstPersonRender initialized!");
	}

	void Update()
	{
		if (!_isInitialized)
			return;

		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
		{
			if (AreBothArmsBusy)
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
				ShowBodyPart(PlayerHandLeftParent);
				HideFirstPersonHand(PlayerFirstPersonHandLeft);
			}
			else if (IsRightArmBusy)
			{
				HideBodyPart(PlayerHandRightParent);
				ShowFirstPersonHand(PlayerFirstPersonHandRight);
			}
			else
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
				ShowBodyPart(PlayerHandLeftParent);
				HideFirstPersonHand(PlayerFirstPersonHandLeft);
			}
		}

		if (playerCamera.CurrentPlayerCameraStateType == "ThirdPerson")
		{
			if (AreBothArmsBusy)
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
				ShowBodyPart(PlayerHandLeftParent);
				HideFirstPersonHand(PlayerFirstPersonHandLeft);
			}
			else if (IsRightArmBusy)
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
			}
			else
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
				ShowBodyPart(PlayerHandLeftParent);
				HideFirstPersonHand(PlayerFirstPersonHandLeft);
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