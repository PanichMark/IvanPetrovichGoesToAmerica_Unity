using UnityEngine;
using System.Collections;

public class LegKickAttack : MonoBehaviour
{
	private IInputDevice inputDevice;
	private PlayerMovementController playerMovementController;
	private GameObject cachedPlayer;
	public bool IsPlayerLegKicking { get; private set; }

	private float CapsuleHeight;    
	private float CapsuleRadius;  
	private float ForwardOffset;   

	public float WeaponDamage { get; private set; } = 50;

	public void Initialize(IInputDevice inputDevice, GameObject cachedPlayer, PlayerMovementController playerMovementController)
	{
		this.inputDevice = inputDevice;
		this.cachedPlayer = cachedPlayer;
		this.playerMovementController = playerMovementController;

		CapsuleHeight = 1.8f;   
		CapsuleRadius = 0.3f;     
		ForwardOffset = 0.5f;      
		IsPlayerLegKicking = false;

		Debug.Log("LegKickAttack Initialized");
	}
	
	void Update()
	{
		if (inputDevice.GetKeyLegKick() && !IsPlayerLegKicking && (playerMovementController.CurrentPlayerMovementStateType == "PlayerIdle" || playerMovementController.CurrentPlayerMovementStateType == "PlayerWalking"
			|| playerMovementController.CurrentPlayerMovementStateType == "PlayerRunning" || playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"))
		{ 
			LegKick();
		}

		if (!playerMovementController.IsPlayerCrouching)
		{
			CapsuleHeight = 1.8f;
		}
		else CapsuleHeight = 1;
	}
	
	public void LegKick()
	{
		Debug.Log("LegKick attack");

		if (playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" || playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
		else
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}

		StartCoroutine(playerMovementController.DisablePlayerMovementDuringLegKickAttack());
		StartCoroutine(DisableLegKickAttackActivation());

		Vector3 startPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset;
		Vector3 endPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset + cachedPlayer.transform.up * CapsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, transform.forward, 0f);

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

