using System.Collections;
using UnityEngine;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract, IBreakable
{
	protected bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	[SerializeField] private int _doorOpenAngle = 90;
	protected string _interactionHintMessageMain;
	private KeysManager _keysManager;
	[SerializeField] private float _doorOpeningSpeed = 200f;
	[SerializeField] protected InteractionObjectKeyData _interactionObjectKeyData;
	[SerializeField] protected InteractionObjectLockMechanical _mechanicalLockController;
	[SerializeField] protected InteractionObjectLockElectronic _electronicLockController;
	[SerializeField] protected InteractionObjectElectricalPanel _electronicElectricalPanel;
	[SerializeField] private float _maxDurability = 100f;
	[SerializeField] private float _damageThreshold = 50f;
	[SerializeField] private bool _isDestructable;
	[SerializeField] private GameObject _doorNormal;
	[SerializeField] private GameObject _doorDamaged;
	[SerializeField] private GameObject _doorBroken;
	[SerializeField] protected bool _isLockedForever;
	private Collider _collider;
	private SkinnedMeshRenderer _doorBrokenSkinnedMeshRenderer;

	public override string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	private Coroutine _currentAnimation;

	private Quaternion _openedRotation;
	private Quaternion _closedRotation;

	private string _interactionHintMessageFail;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;

	public bool IsObjectBroken => CurrentDurability <= 0f;

	public float CurrentDurability { get; private set; }

	public float DamageThreshold => _damageThreshold;

	void Start()
	{
		_collider = GetComponent<Collider>();

		if (_isDestructable)
		{
			_doorBrokenSkinnedMeshRenderer = _doorBroken.GetComponent<SkinnedMeshRenderer>();
		}

		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		CurrentDurability = _maxDurability;

		Vector3 openedEulerAngles = new Vector3(0, _doorOpenAngle, 0);
		_openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedKey")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
			{
				_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
				_mechanicalLockController.OnUnlockLock += UnlockDoor;
			}
			if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
			{
				_interactionHintMessageMain = _electronicLockController.InteractionHintMessageMain;
				_electronicLockController.OnUnlockLock += UnlockDoor;
			}
			if (_electronicElectricalPanel != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedElectricalPanel")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null && _electronicElectricalPanel == null)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| _electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true))
			{
				SetUnlockedDoorHintMessageMain();

				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedForever")}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}
	}

	public override void Interact()
	{
		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_mechanicalLockController.Interact();
			}
			if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_electronicLockController.Interact();
			}
			if (_electronicElectricalPanel != null)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null && _electronicElectricalPanel == null)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| _electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true))
			{
				PerformDoorInteraction();
			}
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	public override void InteractCutscene()
	{
		Interact();
	}

	public virtual void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedKey")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
			{
				_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
				_mechanicalLockController.OnUnlockLock += UnlockDoor;
			}
			if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
			{
				_interactionHintMessageMain = _electronicLockController.InteractionHintMessageMain;
				_electronicLockController.OnUnlockLock += UnlockDoor;
			}
			if (WasOpenableUnlocked ||
				(_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null)
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())))
			{
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedForever")}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}
	}

	protected virtual void UnlockDoor()
	{
		WasOpenableUnlocked = true;

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	protected virtual void PerformDoorInteraction()
	{
		_isAdditionalInteractionHintActive = false;

		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

		if (!IsObjectOpened)
		{
			Debug.Log($"Opened {InteractionObjectNameUI}");
			IsObjectOpened = true;
			_currentAnimation = StartCoroutine(OpenDoor());
		}
		else
		{
			Debug.Log($"Closed {InteractionObjectNameUI}");
			IsObjectOpened = false;
			_currentAnimation = StartCoroutine(CloseDoor());
		}

		SetUnlockedDoorHintMessageMain();
	}

	protected virtual void SetUnlockedDoorHintMessageMain()
	{
		if (IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	IEnumerator OpenDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _openedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	IEnumerator CloseDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _closedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	public void TakeDamage(float amount)
	{
		if (_isDestructable)
		{
			// Проверка на порог урона
			if (amount < _damageThreshold)
			{
				Debug.Log($"Недостаточно урона для break. Требуется: {_damageThreshold}, получено: {amount}");
				return;
			}

			// Нанесение урона
			CurrentDurability -= amount;
			Debug.Log($"Нанесено урона: {amount}. Осталось прочности: {CurrentDurability}");

			if (CurrentDurability <= _maxDurability / 2)
			{
				_doorNormal.SetActive(false);
				_doorDamaged.SetActive(true);
			}

			// Проверка на разрушение
			if (CurrentDurability <= 0f)
			{
				ObjectIsFullyBroken();
			}
		}
	}

	public void ObjectIsFullyBroken()
	{
		Debug.Log("Был broke!");
		_collider.enabled = false;
		_doorDamaged.SetActive(false);
		_doorBroken.SetActive(true);

		StartCoroutine(BreakBlendShapeAnimation());
	}

	IEnumerator BreakBlendShapeAnimation()
	{
		float duration = 0.5f;
		float elapsed = 0f;
		int index = 0;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			_doorBrokenSkinnedMeshRenderer.SetBlendShapeWeight(index, Mathf.Lerp(0f, 100f, elapsed / duration));
			yield return null;
		}

		_doorBrokenSkinnedMeshRenderer.SetBlendShapeWeight(index, 100f);
	}
}