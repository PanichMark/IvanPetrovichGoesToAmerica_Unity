using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	private PlayerCameraController playerCamera;

	public void Initialize(PlayerCameraController playerCameraController, GameObject playerHeadParent)
	{
		this.playerCamera = playerCameraController;

		this.PlayerHeadParent = playerHeadParent;

		_isInitialized = true;
		Debug.Log("FirstPersonRender Initialized!");
	}

	private PlayerCameraStateTypes playerCameraStateType;

	private bool _isInitialized = false;

	private GameObject PlayerHeadParent;

	void Update()
	{
		if (!_isInitialized)
			return;

		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson") 
		{
			HideBodyPart(PlayerHeadParent);
	
		}		
		else 
		{
			ShowBodyPart(PlayerHeadParent);
		}
	}
	
	public void ShowBodyPart(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}
		}
	}

	public void HideBodyPart(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			}
		}
	}
}