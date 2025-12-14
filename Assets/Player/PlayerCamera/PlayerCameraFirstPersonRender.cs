using UnityEngine;
public class PlayerCameraFirstPersonRender : MonoBehaviour
{
	PlayerCameraController playerCamera;
	WeaponController weaponController;
	public PlayerCameraStateType playerCameraStateType;

	public GameObject PlayerCameraObject;

	public GameObject PlayerFirstPersonHandRight;
	public GameObject PlayerFirstPersonHandLeft;
	public GameObject PlayerHeadParent;
	public GameObject PlayerHandRightParent;
	public GameObject PlayerHandLeftParent;

	void Start()
	{
		playerCamera = PlayerCameraObject.GetComponent<PlayerCameraController>();
		weaponController = GetComponent<WeaponController>();
	}

	private void Update()
	{
		if (playerCamera.CurrentPlayerCameraStateType == "FirstPerson")
		{
			if (weaponController.RightHandWeapon != null)
			{
				ShowPlayerWeapon(weaponController.RightHandWeapon.FirstPersonWeaponModelInstance, false); // Первое лицо, оружие первого лица видно, без теней
				HidePlayerWeapon(weaponController.RightHandWeapon.ThirdPersonWeaponModelInstance, true);  // Третье лицо, оружие третьего лица скрыто, но отбрасывает тени
			}

		    if (weaponController.LeftHandWeapon != null)
			{
			    ShowPlayerWeapon(weaponController.LeftHandWeapon.FirstPersonWeaponModelInstance, false);   // Вторая рука, оружие первого лица, аналогично первой руке
			    HidePlayerWeapon(weaponController.LeftHandWeapon.ThirdPersonWeaponModelInstance, true);   // Вторая рука, оружие третьего лица, аналогично первой руке
			}
	    }
		else
		{
			if (weaponController.RightHandWeapon != null)
			{
				ShowPlayerWeapon(weaponController.RightHandWeapon.ThirdPersonWeaponModelInstance, true);  // Третье лицо, оружие третьего лица, показывает и отбрасывает тени
				HidePlayerWeapon(weaponController.RightHandWeapon.FirstPersonWeaponModelInstance, false); // Первая рука, оружие первого лица, ничего не видно и нет теней
			}

			if (weaponController.LeftHandWeapon != null)
			{
				ShowPlayerWeapon(weaponController.LeftHandWeapon.ThirdPersonWeaponModelInstance, true);   // Левая рука, оружие третьего лица, аналогично правой руке
				HidePlayerWeapon(weaponController.LeftHandWeapon.FirstPersonWeaponModelInstance, false);  // Левая рука, оружие первого лица, аналогично правой руке
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
				if (weaponController.RightHandWeapon.ThirdPersonWeaponModelInstance.activeInHierarchy)
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
				if (weaponController.LeftHandWeapon.ThirdPersonWeaponModelInstance.activeInHierarchy)
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

