using UnityEngine;
using System.Collections;
using TMPro;

public class WeaponMeleeBaton : WeaponMeleeSwinging
{
	private Coroutine _currentStranglingCoroutine;
	private NPCstateMachineController _NPCstateMachineController;
	private bool _isAbleToStrangle = false;
	private bool _npcDetected = false;
	private GameObject _strangleHintNPCtext;
	private TextMeshProUGUI _strangleHintNPCtextComponent;
	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerWeaponController _weaponController;
	private LocalizationManager _localizationManager;
	private PlayerWeaponFirstPersonRenderer _playerWeaponFirstPersonRenderer;

	private float _strangleDuration = 3.292f;
	private ViewModelHUDInteraction _viewModelHUDInteraction;
	private GameController _gameController;

	protected override void InitializeWeaponMelee()
	{
		base.InitializeWeaponMelee();
		_gameController = ServiceLocator.Resolve<GameController>();
		_playerWeaponFirstPersonRenderer = ServiceLocator.Resolve<PlayerWeaponFirstPersonRenderer>();
		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_strangleHintNPCtext = _viewModelHUDInteraction.TextStrangleHintNPC;
		_strangleHintNPCtextComponent = _strangleHintNPCtext.GetComponent<TextMeshProUGUI>();
		_inputDevice = ServiceLocator.Resolve<IInputDevice>();
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>();
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		ChangeLanguage(_localizationManager);

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void OnDestroy()
	{
		//_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}

	public override void WeaponPlayerAttack()
	{

		if (isPlayerWeaponAttacking)
		{
			Debug.Log("Already attacking melee");
			return;
		}

		if (_isAbleToStrangle)
		{

			PerformStrangleAttack();
			return;
		}

		StartCoroutine(SingleMeleeWeaponAttack());

	}

	private void Update()
	{
		if (!_isWeaponInitialized)
			return;

		if (_currentStranglingCoroutine != null)
			return;

		Vector3 playerPosition = _attackPoint.transform.position;
		Vector3 playerForward = _attackPoint.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, _capsuleRadius);

		bool newDetection = false;
		foreach (var hit in hitColliders)
		{
			if (hit.gameObject == _attackPoint) continue;
			if (hit.GetComponent<NPClivingBeing>() != null)
			{
				newDetection = true;
				_NPCstateMachineController = hit.GetComponent<NPCstateMachineController>();
				break;
			}
		}

		_npcDetected = newDetection;

		bool isCrouching = (_playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdleCrouhcing ||
						   _playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingCrouching);

		_isAbleToStrangle = _npcDetected && isCrouching && !isPlayerWeaponAttacking;



		_strangleHintNPCtext.SetActive(_isAbleToStrangle);
	}

	private void PerformStrangleAttack()
	{
		if (_currentStranglingCoroutine != null)
		{
			StopCoroutine(_currentStranglingCoroutine);
		}

		_currentStranglingCoroutine = StartCoroutine(StranglingCoroutine());
	}

	private IEnumerator StranglingCoroutine()
	{
		_gameController.MakePlayerStartStranglingNPC();
		_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStranglingNPC);
		isPlayerWeaponAttacking = true;
		StartCoroutine(_playerWeaponAnimationController.AnimationBatonStrangle(this));
		_playerWeaponFirstPersonRenderer.Set1stPersonStranglingObjectsToLayer("Default", WeaponHandType);

		_strangleHintNPCtext.SetActive(false);
		Debug.Log("START strangle!");
		_NPCstateMachineController.SetNPCState(NPCstateTypes.Strangled);
		float elapsed = 0f;

		while (elapsed < _strangleDuration)
		{
			if ((WeaponHandType == WeaponHandType.Right && _inputDevice.GetKeyRightHandWeaponAttackReleased()) ||
				(WeaponHandType == WeaponHandType.Left && _inputDevice.GetKeyLeftHandWeaponAttackReleased()))
			{
				Debug.Log("Failed to strangle!!!");

				_NPCstateMachineController.SetNPCState(NPCstateTypes.Alarmed);
				StopStrangling();
				yield break;
			}

			elapsed += Time.deltaTime;
			yield return null;
		}

		Debug.Log("strangle SUCCESS!!!");
		_NPCstateMachineController.SetNPCState(NPCstateTypes.Unconscious);
		StopStrangling();
	}

	private void StopStrangling()
	{
		_gameController.MakePlayerStopStranglingNPC();
		_currentStranglingCoroutine = null;
		isPlayerWeaponAttacking = false;
		_playerWeaponFirstPersonRenderer.Set1stPersonStranglingObjectsToLayer("FirstPerson", WeaponHandType);
		_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdleStanding);
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (WeaponHandType == WeaponHandType.Right)
		{
			_strangleHintNPCtextComponent.text = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_MainHold")} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackRightHand)} {_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Choke")}";
		}
		else
		{
			_strangleHintNPCtextComponent.text = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_MainHold")} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackLeftHand)} {_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Choke")}";
		}
	}

	public override void OnHideWeaponPlayer()
	{
		if (_currentStranglingCoroutine != null)
		{
			_currentStranglingCoroutine = null;
			StopStrangling();
		}
	}
}
