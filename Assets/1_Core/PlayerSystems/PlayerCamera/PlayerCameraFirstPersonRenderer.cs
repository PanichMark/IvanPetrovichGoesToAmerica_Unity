using UnityEngine;

public class PlayerCameraFirstPersonRenderer : MonoBehaviour
{
	private PlayerCameraStateMachineController _playerStateMachineCamera;

	private GameObject _playerHead;
	private GameObject _playerHatSlot;

	public void Initialize(
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject playerHead,
		GameObject playerHatSlot)
	{
		_playerStateMachineCamera = playerCameraStateMachineController;

		_playerHead = playerHead;
		_playerHatSlot = playerHatSlot;

		_playerStateMachineCamera.OnFirstPersonCameraState += HidePlayerHead;
		_playerStateMachineCamera.OnThirdPersonCameraState += ShowPlayerHead;

		Debug.Log("PlayerCameraFirstPersonRender Initialized");
	}

	private void ShowPlayerHead()
	{
		ShowBodyPart(_playerHead);
		ShowBodyPart(_playerHatSlot);
	}

	private void HidePlayerHead()
	{
		HideBodyPart(_playerHead);
		HideBodyPart(_playerHatSlot);
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