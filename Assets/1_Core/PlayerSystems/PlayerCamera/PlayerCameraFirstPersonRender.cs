using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private PlayerCameraStateMachineController _playerStateMachineCamera;

	private GameObject _playerHead;
	private GameObject _playerHatSlot;

	public void Initialize(
		Bootstrap bootstrap,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject playerHead,
		GameObject playerHatSlot)
	{
		_bootstrap = bootstrap;
		_playerStateMachineCamera = playerCameraStateMachineController;

		_playerHead = playerHead;
		_playerHatSlot = playerHatSlot;

		Debug.Log("PlayerCameraFirstPersonRender Initialized");
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerStateMachineCamera.CurrentPlayerCameraStateType == "FirstPerson") 
		{
			HideBodyPart(_playerHead);
			HideBodyPart(_playerHatSlot);
		}		
		else 
		{
			ShowBodyPart(_playerHead);
			ShowBodyPart(_playerHatSlot);
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