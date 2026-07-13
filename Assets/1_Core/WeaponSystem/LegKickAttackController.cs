using UnityEngine;
using System.Collections;

public delegate void OnLegKickStateChanged(bool isKicking);

public class LegKickAttackController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private GameObject _cachedPlayer;
	private PlayerWeaponController _playerWeaponController;
	public bool IsPlayerLegKicking { get; private set; }

	public event OnLegKickStateChanged OnLegKickStateChanged;
	private float _capsuleHeight;    
	private float _capsuleRadius;  
	private float _forwardOffset;   

	public float WeaponDamage { get; private set; } = 50;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		PlayerMovementController playerMovementController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		GameObject cachedPlayer,
		PlayerWeaponController playerWeaponController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_cachedPlayer = cachedPlayer;
		_playerMovementController = playerMovementController;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerWeaponController = playerWeaponController;

		_capsuleHeight = 1.8f;   
		_capsuleRadius = 0.3f;     
		_forwardOffset = 0.5f;      
		IsPlayerLegKicking = false;

		Debug.Log("LegKickAttackController");
	}
	
	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyLegKick() && !IsPlayerLegKicking && _playerWeaponController.HasAnyWeapon && (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerIdle" || _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerWalking"
			|| _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerRunning" || _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"))
		{ 
			LegKick();
		}

		if (!_playerMovementController.IsPlayerCrouching)
		{
			_capsuleHeight = 1.8f;
		}
		else _capsuleHeight = 1;
	}
	
	public void LegKick()
	{
		Debug.Log("LegKick attack");

		if (_playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" || _playerMovementStateMachineController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
		else
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}

		StartCoroutine(_playerMovementController.DisablePlayerMovementDuringLegKickAttack());
		StartCoroutine(DisableLegKickAttackActivation());

		OnLegKickStateChanged?.Invoke(true);

		Vector3 startPoint = _cachedPlayer.transform.position + _cachedPlayer.transform.forward * _forwardOffset;
		Vector3 endPoint = _cachedPlayer.transform.position + _cachedPlayer.transform.forward * _forwardOffset + _cachedPlayer.transform.up * _capsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _capsuleRadius, transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject.CompareTag("Player"))
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayLegKickAttackDamage(damageable, 0.5f, WeaponDamage));
			}

			if (hit.collider.TryGetComponent<IBreakable>(out var breakable))
			{
				StartCoroutine(DelayLegKickAttackDamage(breakable, 0.5f, WeaponDamage));
			}
		}
	}
	
	IEnumerator DisableLegKickAttackActivation()
	{
		IsPlayerLegKicking = true;
		yield return new WaitForSeconds(0.95f);
		IsPlayerLegKicking = false;

		OnLegKickStateChanged?.Invoke(false);
	}

	IEnumerator DelayLegKickAttackDamage(IDamageable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime);

		target.TakeDamage(damageAmount);
	}

	IEnumerator DelayLegKickAttackDamage(IBreakable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime);

		target.TakeDamage(damageAmount);
	}
}