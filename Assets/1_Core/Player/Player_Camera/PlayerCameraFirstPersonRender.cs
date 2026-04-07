using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{


	private PlayerCameraController playerCamera;


	public void Initialize(PlayerCameraController playerCameraController, GameObject playerHeadParent)
	{
		
		this.playerCamera = playerCameraController;
	

		// Присваиваем полученные объекты

		this.PlayerHeadParent = playerHeadParent;



		//playerCameraFirstPersonRender.HideFirstPersonHand(playerFirstPersonHandRight);
		//playerCameraFirstPersonRender.HideFirstPersonHand(playerFirstPersonHandLeft);
		// Регистрация события смены оружия
		
		_isInitialized = true;
		Debug.Log("FirstPersonRender Initialized!");
	}

	private PlayerCameraStateTypes playerCameraStateType;

	//public GameObject PlayerCameraObject;

	
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
		// Получаем все рендеры (включая дочерние объекты)
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		// Перебираем все рендеры и включаем отбрасывание теней
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
		// Получаем все рендеры (включая дочерние объекты)
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		// Перебираем все рендеры и включаем отбрасывание теней
		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			}
		}
	}
}


