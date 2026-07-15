using UnityEngine;

public class PlayerWeaponFirstPersonRenderer : MonoBehaviour
{
	private WeaponAbstract _leftHandWeaponComponent;
	private WeaponAbstract _rightHandWeaponComponent;
	private PlayerCameraStateMachineController _playerCameraStateMachine;
	private PlayerWeaponController _weaponController;
	private PlayerWeaponAnimationController _weaponAnimationController;
	private GameSceneManager _gameSceneManager;


	private GameObject _playerFirstPersonHandRight;
	private GameObject _playerFirstPersonHandLeft;
	private GameObject _playerThirdPersonHandRight;
	private GameObject _playerThirdPersonHandLeft;

	public void Initialize(
		GameSceneManager gameSceneManager,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerWeaponController weaponController,
		PlayerWeaponAnimationController weaponAnimationController,
		GameObject playerFirstPersonHandRight,
		GameObject playerFirstPersonHandLeft,
		GameObject playerThirdPersonHandRight,
		GameObject playerThirdPersonHandLeft)
	{
		_gameSceneManager = gameSceneManager;
		_playerCameraStateMachine = playerCameraStateMachineController;
		_weaponController = weaponController;
		_weaponAnimationController = weaponAnimationController;

		_playerFirstPersonHandRight = playerFirstPersonHandRight;
		_playerFirstPersonHandLeft = playerFirstPersonHandLeft;
		
		_playerThirdPersonHandRight = playerThirdPersonHandRight;
		_playerThirdPersonHandLeft = playerThirdPersonHandLeft;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(_playerFirstPersonHandRight);
		_gameSceneManager.OnBeginLoadingMainMenuScene += () => HideFirstPersonHand(_playerFirstPersonHandLeft);
		_weaponController.OnWeaponChanged += RegisterWeapons;

		_weaponController.OnShowWeaponRight += UpdateWeaponRightVisibility;
		_weaponController.OnHideWeaponRight += UpdateWeaponRightVisibility;
		_weaponController.OnShowWeaponLeft += UpdateWeaponLeftVisibility;
		_weaponController.OnHideWeaponLeft += UpdateWeaponLeftVisibility;

		_playerCameraStateMachine.OnFirstPersonCameraState += UpdateWeaponRightVisibility;
		_playerCameraStateMachine.OnThirdPersonCameraState += UpdateWeaponRightVisibility;
		_playerCameraStateMachine.OnFirstPersonCameraState += UpdateWeaponLeftVisibility;
		_playerCameraStateMachine.OnThirdPersonCameraState += UpdateWeaponLeftVisibility;

		_playerCameraStateMachine.OnFirstPersonCameraState += UpdateWeaponRightVisibility;
		_playerCameraStateMachine.OnThirdPersonCameraState += UpdateWeaponRightVisibility;

		_playerCameraStateMachine.OnFirstPersonCameraState += ShowReloadingHelpingHand;
		_playerCameraStateMachine.OnThirdPersonCameraState += ShowReloadingHelpingHand;
		_weaponAnimationController.OnPlayerReload += ShowReloadingHelpingHand;

		Debug.Log("WeaponFirstPersonRender Initialized!");
	}

	private void UpdateWeaponRightVisibility()
	{
		if (_weaponController.RightHandWeapon != null)
		{
			if (_playerCameraStateMachine.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
			{
				if (_weaponController.RightHandWeapon.activeInHierarchy)
				{
					ShowPlayerWeapon(_rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);
					HidePlayerWeapon(_rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);

					HideBodyPart(_playerThirdPersonHandRight);
					ShowFirstPersonHand(_playerFirstPersonHandRight);
				}
			}
			else
			{
				if (_weaponController.RightHandWeapon.activeInHierarchy)
				{
					ShowPlayerWeapon(_rightHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
					HidePlayerWeapon(_rightHandWeaponComponent.FirstPersonWeaponModelInstance, true);

					ShowBodyPart(_playerThirdPersonHandRight);
					HideFirstPersonHand(_playerFirstPersonHandRight);
				}
				else
				{
					ShowBodyPart(_playerThirdPersonHandRight);
					HideFirstPersonHand(_playerFirstPersonHandRight);
				}
			}
		}
		else
		{
			ShowBodyPart(_playerThirdPersonHandRight);
			HideFirstPersonHand(_playerFirstPersonHandRight);
		}
	}

	private void UpdateWeaponLeftVisibility()
	{
		if (_weaponController.LeftHandWeapon != null)
		{
			if (_playerCameraStateMachine.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
			{
				if (_weaponController.LeftHandWeapon.activeInHierarchy)
				{
					ShowPlayerWeapon(_leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);
					HidePlayerWeapon(_leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);

					HideBodyPart(_playerThirdPersonHandLeft);
					ShowFirstPersonHand(_playerFirstPersonHandLeft);
				}
			}
			else
			{
				if (_weaponController.LeftHandWeapon.activeInHierarchy)
				{
					ShowPlayerWeapon(_leftHandWeaponComponent.ThirdPersonWeaponModelInstance, true);
					HidePlayerWeapon(_leftHandWeaponComponent.FirstPersonWeaponModelInstance, true);

					ShowBodyPart(_playerThirdPersonHandLeft);
					HideFirstPersonHand(_playerFirstPersonHandLeft);
				}
				else
				{
					ShowBodyPart(_playerThirdPersonHandLeft);
					HideFirstPersonHand(_playerFirstPersonHandLeft);
				}
			}
		}
		else
		{
			ShowBodyPart(_playerThirdPersonHandLeft);
			HideFirstPersonHand(_playerFirstPersonHandLeft);
		}
	}

	public void ShowReloadingHelpingHand()
	{
		if (_weaponAnimationController.IsPlayerReloading)
		{
			if (_playerCameraStateMachine.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
			{
				if (_weaponAnimationController.CurrentPlayerReloadingHelpingHand == WeaponHandsEnum.HandRight)
				{
					HideBodyPart(_playerThirdPersonHandRight);
					ShowFirstPersonHand(_playerFirstPersonHandRight);
				}
				else
				{
					HideBodyPart(_playerThirdPersonHandLeft);
					ShowFirstPersonHand(_playerFirstPersonHandLeft);
				}
			}
			else
			{
				if (_weaponAnimationController.CurrentPlayerReloadingHelpingHand == WeaponHandsEnum.HandRight)
				{
					ShowBodyPart(_playerThirdPersonHandRight);
					HideFirstPersonHand(_playerFirstPersonHandRight);
				}
				else
				{
					ShowBodyPart(_playerThirdPersonHandLeft);
					HideFirstPersonHand(_playerFirstPersonHandLeft);
				}
			}
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