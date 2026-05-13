using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	private PlayerCameraStateMachineController _playerStateMachineCamera;

	private bool _isInitialized = false;

	private GameObject _playerHeadParent;

	public void Initialize(PlayerCameraStateMachineController playerCameraStateMachineController, GameObject playerHeadParent)
	{
		_playerStateMachineCamera = playerCameraStateMachineController;

		_playerHeadParent = playerHeadParent;

		_isInitialized = true;

		Debug.Log("FirstPersonRender Initialized!");
	}

	void Update()
	{
		if (!_isInitialized)
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