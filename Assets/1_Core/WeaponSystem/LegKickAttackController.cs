using UnityEngine;
using System.Collections;

public class LegKickAttackController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private PlayerMovementController _playerMovementController;
	private GameObject _cachedPlayer;
	public bool IsPlayerLegKicking { get; private set; }

	private float _CapsuleHeight;    
	private float _CapsuleRadius;  
	private float _ForwardOffset;   

	public float WeaponDamage { get; private set; } = 50;

	public void Initialize(IInputDevice inputDevice, GameObject cachedPlayer, PlayerMovementController playerMovementController)
	{
		_inputDevice = inputDevice;
		_cachedPlayer = cachedPlayer;
		_playerMovementController = playerMovementController;

		_CapsuleHeight = 1.8f;   
		_CapsuleRadius = 0.3f;     
		_ForwardOffset = 0.5f;      
		IsPlayerLegKicking = false;

		Debug.Log("LegKickAttack Initialized");
	}
	
	void Update()
	{
		if (_inputDevice.GetKeyLegKick() && !IsPlayerLegKicking && (_playerMovementController.CurrentPlayerMovementStateType == "PlayerIdle" || _playerMovementController.CurrentPlayerMovementStateType == "PlayerWalking"
			|| _playerMovementController.CurrentPlayerMovementStateType == "PlayerRunning" || _playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			_playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"))
		{ 
			LegKick();
		}

		if (!_playerMovementController.IsPlayerCrouching)
		{
			_CapsuleHeight = 1.8f;
		}
		else _CapsuleHeight = 1;
	}
	
	public void LegKick()
	{
		Debug.Log("LegKick attack");

		if (_playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" || _playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
		else
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}

		StartCoroutine(_playerMovementController.DisablePlayerMovementDuringLegKickAttack());
		StartCoroutine(DisableLegKickAttackActivation());

		Vector3 startPoint = _cachedPlayer.transform.position + _cachedPlayer.transform.forward * _ForwardOffset;
		Vector3 endPoint = _cachedPlayer.transform.position + _cachedPlayer.transform.forward * _ForwardOffset + _cachedPlayer.transform.up * _CapsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _CapsuleRadius, transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject.CompareTag("Player"))
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayLegKickAttackDamage(damageable, 0.5f, WeaponDamage));
			}
		}
	}
	
	IEnumerator DisableLegKickAttackActivation()
	{
		IsPlayerLegKicking = true;
		yield return new WaitForSeconds(0.95f);
		IsPlayerLegKicking = false;
	}

	IEnumerator DelayLegKickAttackDamage(IDamageable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime);

		target.TakeDamage(damageAmount);
	}
}