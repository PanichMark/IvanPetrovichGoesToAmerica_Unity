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
	
		leftHandWeaponComponent = weaponController.LeftHandWeapon.GetComponent<WeaponClass>();
		else
			rightHandWeaponComponent = weaponController.RightHandWeapon.GetComponent<WeaponClass>();
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
			if (rightHandWeaponComponent != null)
			{
				ShowPlayerWeapon(weaponController.rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(weaponController.rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);

				//ShowPlayerWeapon(weaponController.RightHandWeapon, false); // Первое лицо, оружие первого лица видно, без теней
				//HidePlayerWeapon(weaponController.RightHandWeapon, true);  // Третье лицо, оружие третьего лица скрыто, но отбрасывает тени
			}

		    if (leftHandWeaponComponent != null)
			{

				ShowPlayerWeapon(weaponController.leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(weaponController.leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				// ShowPlayerWeapon(weaponController.LeftHandWeapon, false);   // Вторая рука, оружие первого лица, аналогично первой руке
				// HidePlayerWeapon(weaponController.LeftHandWeapon, true);   // Вторая рука, оружие третьего лица, аналогично первой руке
			}
	    }
		else
		{
			if (weaponController.RightHandWeapon != null)
			{
				ShowPlayerWeapon(weaponController.rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(weaponController.rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				//ShowPlayerWeapon(weaponController.RightHandWeapon, false);

				//ShowPlayerWeapon(weaponController.rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);  // Третье лицо, оружие третьего лица, показывает и отбрасывает тени
				//HidePlayerWeapon(weaponController.RightHandWeapon, false); // Первая рука, оружие первого лица, ничего не видно и нет теней
			}

			if (weaponController.LeftHandWeapon != null)
			{
				ShowPlayerWeapon(weaponController.leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(weaponController.leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				//ShowPlayerWeapon(weaponController.LeftHandWeapon, false);
				//ShowPlayerWeapon(weaponController.leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);   // Левая рука, оружие третьего лица, аналогично правой руке
				//HidePlayerWeapon(weaponController.LeftHandWeapon, false);  // Левая рука, оружие первого лица, аналогично правой руке
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

