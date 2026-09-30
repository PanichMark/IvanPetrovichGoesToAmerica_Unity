using UnityEngine;
using System.Collections;
using TMPro;

public class WeaponMeleeBaton : WeaponMeleeAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Baton;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override bool IsWeaponAuto => false;
	public override float WeaponAttackSpeedRate => 1.560f;
	[SerializeField] private AudioClip _weaponSoundSwing;
	public override float MeleeAttackDelay => 0.840f;
	private LocalizationManager	_localizationManager;
	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerWeaponController _weaponController;
	private PlayerWeaponFirstPersonRenderer _playerWeaponFirstPersonRenderer;
	private Coroutine currentStranglingCoroutine = null;

	private GameObject _strangleHintNPCtext;
	private TextMeshProUGUI _strangleHintNPCtextComponent;

	private bool _isAbleToStrangle = false;
	private bool _npcDetected = false;
	private NPCstateMachineController _NPCstateMachineController;
	private ViewModelHUDInteraction _viewModelHUDInteraction;

	protected override void InitializeWeaponMelee()
	{
		_playerWeaponFirstPersonRenderer = ServiceLocator.Resolve<PlayerWeaponFirstPersonRenderer>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_inputDevice = ServiceLocator.Resolve<IInputDevice>();
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>();
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>();

		_strangleHintNPCtext = _viewModelHUDInteraction.TextStrangleHintNPC;
		_strangleHintNPCtextComponent = _strangleHintNPCtext.GetComponent<TextMeshProUGUI>();

		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;

		ChangeLanguage(_localizationManager);

		_localizationManager.OnLanguageChanged += ChangeLanguage;
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

	protected override IEnumerator SingleMeleeWeaponAttack()
	{
		isPlayerWeaponAttacking = true;

		StartCoroutine(DelayAttackSound());

		_currentWeaponPlayerMeleeAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this, true));

		Vector3 startPoint = _attackPoint.transform.position + _attackPoint.transform.forward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _capsuleRadius, _attackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == _attackPoint)
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayMeleeAttackDamageable(damageable, MeleeAttackDelay));
			}

			if (hit.collider.TryGetComponent<IBreakable>(out var breakable))
			{
				StartCoroutine(DelayMeleeAttackBreakable(breakable, MeleeAttackDelay));
			}
		}

		yield return _currentWeaponPlayerMeleeAttackRoutine;

		isPlayerWeaponAttacking = false;

		_currentWeaponPlayerMeleeAttackRoutine = null;
	}

	protected override IEnumerator DelayAttackSound()
	{
		Debug.Log("SOUND ON HIT");
		yield return new WaitForSeconds(MeleeAttackDelay - 0.35f);
		_weaponAudioSource.PlayOneShot(_weaponSoundSwing);
		yield return new WaitForSeconds(0.18f);

		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		yield return null;
	}

	private void Update()
	{
		if (!_isWeaponInitialized)
			return;

		if (currentStranglingCoroutine != null)
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

		bool isCrouching = (_playerMovementStateMachineController.CurrentPlayerMovementStateType ==  PlayerMovementStateTypes.PlayerIdleCrouhcing ||
						   _playerMovementStateMachineController.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingCrouching);

		_isAbleToStrangle = _npcDetected && isCrouching && !isPlayerWeaponAttacking;

		

		_strangleHintNPCtext.SetActive(_isAbleToStrangle);
	}

	private void PerformStrangleAttack()
	{
		if (currentStranglingCoroutine != null)
		{
			StopCoroutine(currentStranglingCoroutine);
		}

		currentStranglingCoroutine = StartCoroutine(StranglingCoroutine());
	}

	private IEnumerator StranglingCoroutine()
	{
		isPlayerWeaponAttacking = true;
		StartCoroutine(_playerWeaponAnimationController.AnimationBatonStrangle(this));
		_playerWeaponFirstPersonRenderer.Set1stPersonStranglingObjectsToLayer("Default", WeaponHandType);
		
		_strangleHintNPCtext.SetActive(false);
		Debug.Log("START strangle!");
		_NPCstateMachineController.SetNPCState(NPCstateTypes.Strangled);
		float strangleDuration = 2f;
		float elapsed = 0f;

		while (elapsed < strangleDuration)
		{
			if ((WeaponHandType == WeaponHandType.Right && _inputDevice.GetKeyRightHandWeaponAttackReleased()) ||
				(WeaponHandType == WeaponHandType.Left && _inputDevice.GetKeyLeftHandWeaponAttackReleased()))
			{
				Debug.Log("Failed to strangle!!!");
				currentStranglingCoroutine = null;
				isPlayerWeaponAttacking = false;
				_playerWeaponFirstPersonRenderer.Set1stPersonStranglingObjectsToLayer("FirstPerson", WeaponHandType);
				_NPCstateMachineController.SetNPCState(NPCstateTypes.Alarmed);
				yield break; 
			}

			elapsed += Time.deltaTime;
			yield return null; 
		}

		Debug.Log("strangle SUCCESS!!!");
		_NPCstateMachineController.SetNPCState(NPCstateTypes.Unconscious);
		isPlayerWeaponAttacking = false;
		_playerWeaponFirstPersonRenderer.Set1stPersonStranglingObjectsToLayer("FirstPerson", WeaponHandType);
		currentStranglingCoroutine = null;
	}

	public override IEnumerator InspectWeaponAnimation()
	{
		throw new System.NotImplementedException();

		// baton isnt inspected but given straigth away during Bistro fight tutorial
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


}