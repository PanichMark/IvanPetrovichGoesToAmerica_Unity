using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	public WeaponClass leftHandWeaponComponent { get; private set; }

	public WeaponClass rightHandWeaponComponent { get; private set; }

	private PlayerCameraController playerCamera;
	private WeaponController weaponController;

	public void Initialize(PlayerCameraController playerCameraController, WeaponController weaponController)
	{
		this.playerCamera = playerCameraController;
		this.weaponController = weaponController;

		this.weaponController.OnWeaponChanged += RegisterWeapons;

		
	}	

	private PlayerCameraStateType playerCameraStateType;

	//public GameObject PlayerCameraObject;

	private void RegisterWeapons(string handType)
	{
		if (handType == "left")
		{
			if (weaponController.LeftHandWeapon != null)
			{
				leftHandWeaponComponent = weaponController.LeftHandWeapon.GetComponent<WeaponClass>();
			}
			else leftHandWeaponComponent = null;
		}
		else
		{
			if (weaponController.RightHandWeapon != null)
			{
				rightHandWeaponComponent = weaponController.RightHandWeapon.GetComponent<WeaponClass>();
			}
			else rightHandWeaponComponent = null;
		}
	}


	public GameObject PlayerFirstPersonHandRight;
	public GameObject PlayerFirstPersonHandLeft;
	public GameObject PlayerHeadParent;
	public GameObject PlayerHandRightParent;
	public GameObject PlayerHandLeftParent;

	void Start()
	{
		//playerCamera = PlayerCameraObject.GetComponent<PlayerCameraController>();
		//weaponController = GetComponent<WeaponController>();
	
	}



	private void Update()
	{
		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
		{
			if (rightHandWeaponComponent != null &&
				rightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
			{
				ShowPlayerWeapon(rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}

			if (leftHandWeaponComponent != null &&
				leftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
			{
				ShowPlayerWeapon(leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}
		}
		else
		{
			if (rightHandWeaponComponent != null &&
				rightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
			{
				ShowPlayerWeapon(rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}

			if (leftHandWeaponComponent != null &&
				leftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
			{
				ShowPlayerWeapon(leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}
		}
	}


	void FixedUpdate()
	{
		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson") 
		{
			HideBodyPart(PlayerHeadParent);

			if (weaponController.RightHandWeapon != null)
			{
				if (weaponController.RightHandWeapon.activeInHierarchy)
				{
					HideBodyPart(PlayerHandRightParent);
					ShowFirstPersonHand(PlayerFirstPersonHandRight);
				}
				else
				{
					ShowBodyPart(PlayerHandRightParent);
					HideFirstPersonHand(PlayerFirstPersonHandRight);
				}

			}			
			else
			{
				ShowBodyPart(PlayerHandRightParent);
				HideFirstPersonHand(PlayerFirstPersonHandRight);
			}

			if (weaponController.LeftHandWeapon != null)
			{
				if (weaponController.LeftHandWeapon.activeInHierarchy)
				{
					HideBodyPart(PlayerHandLeftParent);
					ShowFirstPersonHand(PlayerFirstPersonHandLeft);
				}
				else
				{
					ShowBodyPart(PlayerHandLeftParent);
					HideFirstPersonHand(PlayerFirstPersonHandLeft);
				}
			}
			else
			{
				ShowBodyPart(PlayerHandLeftParent);
				HideFirstPersonHand(PlayerFirstPersonHandLeft);
			}
		}
		else 
		{
			ShowBodyPart(PlayerHeadParent);
			ShowBodyPart(PlayerHandRightParent);
			ShowBodyPart(PlayerHandLeftParent);

			HideFirstPersonHand(PlayerFirstPersonHandRight);
			HideFirstPersonHand(PlayerFirstPersonHandLeft);
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

	public void ShowFirstPersonHand(GameObject rootObj)
	{
		// Получаем все рендеры (включая дочерние объекты)
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		// Перебираем все рендеры и включаем отбрасывание теней
		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.enabled = true;
			}
		}
	}

	public void HideFirstPersonHand(GameObject rootObj)
	{
		// Получаем все рендеры (включая дочерние объекты)
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

		// Перебираем все рендеры и включаем отбрасывание теней
		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.enabled = false;
			}
		}
	}

	public void ShowPlayerWeapon(GameObject weaponRoot, bool castShadows)
	{
		//Debug.Log($"Show{weaponRoot}");
		Renderer[] renderers = weaponRoot.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.enabled = true;                                   // Включаем рендер

				if (castShadows)
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;  // Включаем отбрасывание теней
				}
				else
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Отключаем отбрасывание теней
				}
			}
		}
	}

	public void HidePlayerWeapon(GameObject weaponRoot, bool allowShadows)
	{
		//Debug.Log($"Hide{weaponRoot}");
		Renderer[] renderers = weaponRoot.GetComponentsInChildren<Renderer>(true);

		foreach (Renderer renderer in renderers)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
				{
					renderer.enabled = true;
				}
				else
				{
					renderer.enabled = false;
				}

				if (!allowShadows)
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Полностью отключаем отбрасывание теней
				}
				else
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly; // Оставляем только отбрасывание теней
				}
				
			}
		}
	}
}

