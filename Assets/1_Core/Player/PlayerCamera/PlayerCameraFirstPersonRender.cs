using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	private PlayerCameraController _playerCamera;

	private bool _isInitialized = false;

	private GameObject _playerHeadParent;

	public void Initialize(PlayerCameraController playerCameraController, GameObject playerHeadParent)
	{
		_playerCamera = playerCameraController;

		_playerHeadParent = playerHeadParent;

		_isInitialized = true;

		Debug.Log("FirstPersonRender Initialized!");
	}

	void Update()
	{
		if (!_isInitialized)
			return;

		if (_playerCamera.CurrentPlayerCameraStateType == "FirstPerson") 
		{
			HideBodyPart(_playerHeadParent);
		}		
		else 
		{
			ShowBodyPart(_playerHeadParent);
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