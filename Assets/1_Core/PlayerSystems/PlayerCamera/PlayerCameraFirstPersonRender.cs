using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private PlayerCameraStateMachineController _playerStateMachineCamera;

	private GameObject _playerHeadParent;

	public void Initialize(
		Bootstrap bootstrap,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject playerHeadParent)
	{
		_bootstrap = bootstrap;
		_playerStateMachineCamera = playerCameraStateMachineController;

		_playerHeadParent = playerHeadParent;

		Debug.Log("PlayerCameraFirstPersonRender Initialized");
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerStateMachineCamera.CurrentPlayerCameraStateType == "FirstPerson") 
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