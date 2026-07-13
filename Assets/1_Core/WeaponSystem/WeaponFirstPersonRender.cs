using UnityEngine;
public class WeaponFirstPersonRender : MonoBehaviour
{
	private WeaponAbstract _leftHandWeaponComponent;
	private Bootstrap _bootstrap;
	private WeaponAbstract _rightHandWeaponComponent;
	private PlayerCameraStateTypes _playerCameraStateType;
	private PlayerCameraStateMachineController _playerCameraStateMachine;
	private PlayerWeaponController _weaponController;
	private GameSceneManager _gameSceneManager;


	private GameObject _playerFirstPersonHandRight;
	private GameObject _playerFirstPersonHandLeft;
	private GameObject _playerHandRightParent;
	private GameObject _playerHandLeftParent;

	public void Initialize(
		Bootstrap bootstrap,
		GameSceneManager gameSceneManager,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		GameObject playerFirstPersonHandRight,
		GameObject playerFirstPersonHandLeft,
		GameObject playerHandRightParent,
		GameObject playerHandLeftParent)
	{
		_bootstrap = bootstrap;
		_gameSceneManager = gameSceneManager;
		_playerCameraStateMachine = playerCameraStateMachineController;
		_weaponController = weaponController;

		_playerFirstPersonHandRight = playerFirstPersonHandRight;
		_playerFirstPersonHandLeft = playerFirstPersonHandLeft;
		
		_playerHandRightParent = playerHandRightParent;
		_playerHandLeftParent = playerHandLeftParent;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(_playerFirstPersonHandRight);
		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(_playerFirstPersonHandLeft);
		_weaponController.OnWeaponChanged += RegisterWeapons;

		_weaponController.OnWeaponChanged += ChangeWeaponsVisibility;

		Debug.Log("WeaponFirstPersonRender Initialized!");
	}

	private void ChangeWeaponsVisibility(WeaponHandsEnum handType)
	{
		if (_playerCameraStateMachine.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			if (handType == WeaponHandsEnum.HandRight)
			{
				ShowPlayerWeapon(_rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(_rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}
			else
			{
				ShowPlayerWeapon(_leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
				HidePlayerWeapon(_leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
			}
		}
		else
		{
			if (handType == WeaponHandsEnum.HandRight)
			{
				ShowPlayerWeapon(_rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(_rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}
			else
			{
				ShowPlayerWeapon(_leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
				HidePlayerWeapon(_leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
			}
		}
	}



	void FixedUpdate()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_playerCameraStateMachine.CurrentPlayerCameraStateType == "FirstPerson")
		{
			if (_weaponController.RightHandWeapon != null)
			{
				if (_rightHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
				{
					HideBodyPart(_playerHandRightParent);
					ShowFirstPersonHand(_playerFirstPersonHandRight);
				}
				else
				{
					ShowBodyPart(_playerHandRightParent);
					HideFirstPersonHand(_playerFirstPersonHandRight);
				}
			}
			else
			{
				ShowBodyPart(_playerHandRightParent);
				HideFirstPersonHand(_playerFirstPersonHandRight);
			}

			if (_weaponController.LeftHandWeapon != null)
			{
				if (_leftHandWeaponComponent.FirstPersonWeaponModelInstance.activeInHierarchy)
				{
					HideBodyPart(_playerHandLeftParent);
					ShowFirstPersonHand(_playerFirstPersonHandLeft);
				}
				else
				{
					ShowBodyPart(_playerHandLeftParent);
					HideFirstPersonHand(_playerFirstPersonHandLeft);
				}
			}
			else
			{
				ShowBodyPart(_playerHandLeftParent);
				HideFirstPersonHand(_playerFirstPersonHandLeft);
			}
		}
		else
		{
			ShowBodyPart(_playerHandRightParent);
			ShowBodyPart(_playerHandLeftParent);

			HideFirstPersonHand(_playerFirstPersonHandRight);
			HideFirstPersonHand(_playerFirstPersonHandLeft);
		}
	}

	private void RegisterWeapons(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.HandLeft)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				_leftHandWeaponComponent = _weaponController.LeftHandWeapon.GetComponent<WeaponAbstract>();
			}
			else _leftHandWeaponComponent = null;
		}
		else
		{
			if (_weaponController.RightHandWeapon != null)
			{
				_rightHandWeaponComponent = _weaponController.RightHandWeapon.GetComponent<WeaponAbstract>();
			}
			else _rightHandWeaponComponent = null;
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

	public void ShowFirstPersonHand(GameObject rootObj)
	{
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

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
		Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);

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
				renderer.enabled = true;                                

				if (castShadows)
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;  
				}
				else
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
				if (_playerCameraStateMachine.CurrentPlayerCameraStateType == "FirstPerson")
				{
					renderer.enabled = true;
				}
				else
				{
					renderer.enabled = false;
				}

				if (!allowShadows)
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; 
				}
				else
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly; 
				}
			}
		}
	}
}