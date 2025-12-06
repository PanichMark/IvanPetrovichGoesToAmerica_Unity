using UnityEngine;
using System.Collections;
using System;
public class PlayerMovementController : MonoBehaviour, IDataPersistence
{
	//public InputManager playerInputsList;

	public PlayerMovementState playerMovementState;
	public PlayerMovementStateType playerMovementStateType;

	public Vector3 PlayerWorldMovement;
	private Vector3 PlayerMovement;
	private Vector3 PlayerMovementDirectionWithCamera;

	public PlayerCameraController playerCamera;
	public GameObject PlayerCameraObject;

	PlayerBehaviour playerBehaviour;

	public Transform PlayerTransform;
	public Rigidbody PlayerRigidBody;
	public CapsuleCollider playerCapsuleCollider;
	public Transform playerCapsuleColliderMesh;

	private Vector3 _playerPreviousFramePosition;
	private Vector3 _playerPreviousFramePositionChange;

	private RaycastHit hitInfo;

	public string CurrentPlayerMovementStateType { get; private set; } = "PlayerIdle";

	public float PlayerMovementSpeed { get; private set; }
	public float PlayerRotationSpeed { get; private set; }
	public float PlayerWalkingSpeed { get; private set; }
	public float PlayerRunningSpeed { get; private set; }
	public float PlayerCrouchingSpeed { get; private set; }
	public float PlayerSlidingSpeed { get; private set; }

	public float PlayerCurrentHeight { get; private set; }
	public float PlayerStandingHeight { get; private set; }
	public float PlayerCrouchingHeight { get; private set; }

	public bool IsPlayerMoving { get; private set; }
	public bool IsPlayerAbleToMove { get; private set; }
	public bool IsPlayerGrounded { get; private set; }
	public bool IsPlayerCrouching { get; private set; }
	public bool IsPlayerAbleToStandUp { get; private set; }
	public bool IsPlayerFalling { get; private set; }
	public bool IsPLayerSliding { get; private set; }
	public bool IsPlayerAbleToSlide { get; private set; }
	public bool IsPlayerAbleToClimbLedge { get; private set; }
	public bool IsPlayerOnSlope { get; private set; }

	public float PlayerUpRayYPosition { get; private set; }
	public float PlayerDownRayYPosition { get; private set; }
	
	private float angle;
	private float moveFactor;


	public bool IsPlayerLegKicking;

	

	void Start()
	{

		//playerInputsList = GetComponent<InputManager>();
		playerCamera = PlayerCameraObject.GetComponent<PlayerCameraController>();
		playerBehaviour = GetComponent<PlayerBehaviour>();

		PlayerTransform = GetComponent<Transform>();

		_playerPreviousFramePosition = transform.position;


		playerMovementStateType = (PlayerMovementStateType)Enum.Parse(typeof(PlayerMovementStateType),CurrentPlayerMovementStateType);

		SetPlayerMovementState(playerMovementStateType);

		PlayerMovementSpeed = 3f;
		PlayerWalkingSpeed = 3f;
		PlayerRunningSpeed = 6f;
		PlayerCrouchingSpeed = 1.8f;
		PlayerSlidingSpeed = 7.5f;

		PlayerCurrentHeight = 1.75f;
		PlayerCrouchingHeight = 1;
		PlayerStandingHeight = 1.75f;

		IsPlayerAbleToSlide = true;

		
	}

	/*
	void OnDrawGizmos()
	{
		
		Gizmos.color = Color.red;
		
		Gizmos.DrawRay(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down * 0.3f);
		Gizmos.DrawRay(transform.position + new Vector3(0, PlayerUpRayYPosition, 0), Vector3.up * 0.3f);

		Gizmos.DrawCube(transform.position + transform.up * 1.75f + transform.forward * 0.75f + transform.right * -0.4f, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.DrawCube(transform.position + transform.up * 1.75f + transform.forward * 1.5f + transform.right * -0.4f, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.DrawCube(transform.position + transform.up * 1.75f + transform.forward * 0.75f + transform.right * 0.4f, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.DrawCube(transform.position + transform.up * 1.75f + transform.forward * 1.5f + transform.right * 0.4f, new Vector3(0.25f, 0.25f, 0.25f));

		Gizmos.DrawCube(transform.position + transform.forward * 1.1f + new Vector3(0, 3, 0), new Vector3(1.25f, 2.25f, 1.25f));

		Gizmos.DrawCube(transform.position + transform.forward * 1.1f + new Vector3(0, 2.5f, 0), new Vector3(1.25f, 1.25f, 1.25f));
		
	}
	*/

	void Update()
	{
		//Debug.Log(PlayerMovementSpeed);

		// Player movement State Machine methods
		playerMovementState.ChangePlayerMovementState();
		playerMovementState.ChangePlayerMovementSpeed();

		//
		if (InputManager.Instance.GetKeyRight())
		{
			PlayerWorldMovement.x = 1;
		}
		else if (InputManager.Instance.GetKeyLeft())
		{
			PlayerWorldMovement.x = -1;
		}
		else PlayerWorldMovement.x = 0;

		if (InputManager.Instance.GetKeyUp())
		{
			PlayerWorldMovement.z = 1;
		}
		else if (InputManager.Instance.GetKeyDown())
		{
			PlayerWorldMovement.z = -1;
		}
		else PlayerWorldMovement.z = 0;

		// короче тут проблема
		if (InputManager.Instance.GetKeyJump())
		{
			
			PlayerRigidBody.AddForce(transform.up * 5f, ForceMode.Impulse);
		}

		//
		IsPlayerMoving = (Mathf.Abs(_playerPreviousFramePositionChange.x) > 0.001f || Mathf.Abs(_playerPreviousFramePositionChange.z) > 0.001f);
		IsPlayerGrounded = Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out hitInfo, 0.3f);
		IsPlayerAbleToStandUp = !Physics.Raycast(transform.position + new Vector3(0, PlayerUpRayYPosition, 0), Vector3.up, out hitInfo, 0.3f);
		IsPlayerFalling = (_playerPreviousFramePositionChange.y < -0.01f && IsPlayerGrounded == false);

		//Debug.Log(IsPlayerGrounded);

		if (IsPlayerCrouching == false)
		{
			PlayerDownRayYPosition = 0.1f;
			PlayerUpRayYPosition = 1.9f;
		}
		else if (IsPlayerCrouching == true)
		{
			PlayerDownRayYPosition = 0.1f;
			PlayerUpRayYPosition = 1.2f;
		}
		
		

		// Ledge Climbing BoxCast collision check
		bool isAllBoxesColliding;
		bool isBigRectangleClear;
		bool isSmallRectangleClear;

		if (
			Physics.OverlapBox(transform.position + transform.up * 1.75f + transform.forward * 0.75f + transform.right * -0.4f, new Vector3(0.25f, 0.25f, 0.25f) * 0.5f, Quaternion.identity).Length == 0 ||
			Physics.OverlapBox(transform.position + transform.up * 1.75f + transform.forward * 1.5f + transform.right * -0.4f, new Vector3(0.25f, 0.25f, 0.25f) * 0.5f, Quaternion.identity).Length == 0 ||
			Physics.OverlapBox(transform.position + transform.up * 1.75f + transform.forward * 0.75f + transform.right * 0.4f, new Vector3(0.25f, 0.25f, 0.25f) * 0.5f, Quaternion.identity).Length == 0 ||
			Physics.OverlapBox(transform.position + transform.up * 1.75f + transform.forward * 1.5f + transform.right * 0.4f, new Vector3(0.25f, 0.25f, 0.25f) * 0.5f, Quaternion.identity).Length == 0
			)
		{
			isAllBoxesColliding = false;
		}
		else isAllBoxesColliding = true;

		if (Physics.OverlapBox(transform.position + transform.forward * 1.1f + new Vector3(0, 3, 0), new Vector3(1.25f, 2.25f, 1.25f) * 0.5f, Quaternion.identity).Length > 0)
		{
			isBigRectangleClear = false;
		}
		else isBigRectangleClear = true;

		if (Physics.OverlapBox(transform.position + transform.forward * 1.1f + new Vector3(0, 2.5f, 0), new Vector3(1.25f, 1.25f, 1.25f) * 0.5f, Quaternion.identity).Length > 0)
		{
			isSmallRectangleClear = false;
		}
		else isSmallRectangleClear = true;

		if (isAllBoxesColliding && (isBigRectangleClear || isSmallRectangleClear) && playerMovementStateType != PlayerMovementStateType.PlayerLedgeClimbing)
		{
			IsPlayerAbleToClimbLedge = true;
		}
		else
		{
			IsPlayerAbleToClimbLedge = false;
		}

		// Slope 
		if (Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out hitInfo, 0.3f))
		{
			if (hitInfo.normal != Vector3.up)
			{
				IsPlayerOnSlope = true;
			}
			else
			{
				IsPlayerOnSlope = false;
			}
		}

		

		if (IsPlayerGrounded == true && IsPlayerOnSlope == true)
		{
			PlayerRigidBody.useGravity = false;

			//!!!!!!!!!!!!!!!!!!!!!!!!!!
			// IsPLayerSliding == false 

			// все еще sliding ОШИБКА!
			if (CurrentPlayerMovementStateType == "PlayerJumping" || CurrentPlayerMovementStateType == "PlayerSliding")
			{
				
			}
			else PlayerRigidBody.linearVelocity = Vector3.zero;
		
		}
        else
        {
			PlayerRigidBody.useGravity = true;
		}

		Debug.Log(PlayerRigidBody.linearVelocity);

		if (IsPlayerOnSlope == true)
		{
			Vector3 correctedMovement = PlayerMovement * PlayerMovementSpeed * Time.deltaTime;
			Vector3 projection = Vector3.Project(correctedMovement, hitInfo.normal);
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + correctedMovement - projection);
		}
		else
		{
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + PlayerMovement * PlayerMovementSpeed * Time.deltaTime);
		}

		//
		PlayerMovementDirectionWithCamera = (PlayerWorldMovement.z * playerCamera.CameraForward + PlayerWorldMovement.x * playerCamera.CameraRight);
		PlayerMovement = new Vector3(PlayerMovementDirectionWithCamera.x, 0, PlayerMovementDirectionWithCamera.z);
		PlayerMovement.Normalize();

		angle = Vector3.Angle(hitInfo.normal, Vector3.up);
		moveFactor = 1 / Mathf.Cos(Mathf.Deg2Rad * angle);
	
		//
		if (playerBehaviour.IsPlayerArmed == false && (PlayerMovement != Vector3.zero) && (playerCamera.GetCurrentPlayerCameraType() == PlayerCameraStateType.ThirdPerson.ToString()))
		{
			Quaternion CharacterRotation = Quaternion.LookRotation(PlayerMovement, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, CharacterRotation, PlayerRotationSpeed * Time.deltaTime);
		}
		else if (playerBehaviour.IsPlayerArmed == true || (playerCamera.GetCurrentPlayerCameraType() == PlayerCameraStateType.FirstPerson.ToString()))
		{
			Quaternion PlayerRotateWhereCameraIsLooking = Quaternion.Euler(transform.localEulerAngles.x, playerCamera.CameraRotationY, transform.localEulerAngles.z);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, PlayerRotateWhereCameraIsLooking, PlayerRotationSpeed * Time.deltaTime);
		}

		
	}

	private void FixedUpdate()
	{
		_playerPreviousFramePositionChange = transform.position - _playerPreviousFramePosition;
		_playerPreviousFramePosition = transform.position;
	}

	// Different player movement states scripts call this function
	public void SetPlayerMovementState(PlayerMovementStateType playerMovementStateType)
	{
		PlayerMovementState newState;

		if (playerMovementStateType == PlayerMovementStateType.PlayerIdle)
		{
			newState = new IdlePlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerIdle";
			if (IsPlayerLegKicking == false)
			{
				IsPlayerAbleToMove = true;
			}
			IsPlayerCrouching = false;
			PlayerRotationSpeed = 300f;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerWalking)
		{
			newState = new WalkingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerWalking";
			IsPlayerCrouching = false;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerRunning)
		{
			newState = new RunningPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerRunning";
			IsPlayerCrouching = false;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerJumping)
		{
			newState = new JumpingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerJumping";
			IsPlayerCrouching = false;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerFalling)
		{
			newState = new FallingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerFalling";
			IsPlayerCrouching = false;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerCrouchingIdle)
		{
			newState = new CrouchingIdlePlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerCrouchingIdle";
			IsPlayerCrouching = true;
			if (IsPlayerLegKicking == false)
			{
				IsPlayerAbleToMove = true;
			}
			PlayerRotationSpeed = 300f;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerCrouchingWalking)
		{
			newState = new CrouchingWalkingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerCrouchingWalking";
			IsPlayerCrouching = true;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerSliding)
		{
			newState = new SlidingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerSliding";
			IsPlayerCrouching = true;
			IsPlayerAbleToMove = false;
			PlayerRotationSpeed = 0;
		}
		else if (playerMovementStateType == PlayerMovementStateType.PlayerLedgeClimbing)
		{
			newState = new LedgeClimbingPlayerMovementState(this);
			CurrentPlayerMovementStateType = "PlayerLedgeClimbing";
			IsPlayerCrouching = false;
			IsPlayerAbleToMove = false;
			PlayerRotationSpeed = 0;
		}
		else
		{
			newState = null;
		}
		playerMovementState = newState;

		Debug.Log("PlayerMovementState: " + playerMovementState);
	}

	// Different player movement states scripts call this function
	public float SetPlayerMovementSpeed(float SetSpeed)
	{
		PlayerMovementSpeed = SetSpeed;
		return PlayerMovementSpeed;
	}

	IEnumerator PlayerSlidingCourutine()
	{
		IsPlayerAbleToSlide = false;
		IsPLayerSliding = true;

		PlayerRigidBody.AddForce(transform.forward * PlayerSlidingSpeed, ForceMode.Impulse);
		
		// Disable player controls during sliding
		PlayerRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		
		yield return new WaitForSeconds(1f);

		// Stop player in the sliding end
		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);

		IsPlayerAbleToSlide = true;
		IsPLayerSliding = false;
	}

	// State SlidingState calls this function with courutine as it itself is non Monobahaviour
	public void StartPlayerSliding()
	{
		StartCoroutine(PlayerSlidingCourutine());
	}

	IEnumerator PlayerLedgeClimbingCourutine()
	{
		// CHECK if player will end up in standing or crouching position after ledge climbing
		bool Big;

		if (Physics.OverlapBox(transform.position + transform.forward * 1.1f + new Vector3(0, 3, 0), new Vector3(1.25f, 2.25f, 1.25f) * 0.5f, Quaternion.identity).Length > 0)
		{
			Big = false;
		}
		else Big = true;

		PlayerRigidBody.AddForce(transform.up * 1.2f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.2f);

		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		PlayerRigidBody.AddForce(transform.forward * 1.4f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.1f);
		
		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		// DECIDE if player will end up in standing or crouching position after ledge climbing
		if (Big == true)
		{
			if (IsPlayerMoving == false)
			{
				SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
			}
		}
		else
		{
			if (IsPlayerMoving == false)
			{
				SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
			}
		}
	}

	// State LedgeClimbingState calls this function with courutine as it itself is non Monobahaviour
	public void StartPlayerLedgeClimbing()
	{
		StartCoroutine(PlayerLedgeClimbingCourutine());
	}

	public IEnumerator DisablePlayerMovementDuringLegKickAttack()
	{
		//Debug.Log("Leg Kick Attack");

		IsPlayerLegKicking = true;

		IsPlayerAbleToMove = false;

		yield return new WaitForSeconds(1f);

		IsPlayerAbleToMove = true;

		IsPlayerLegKicking = false;
	}

	public void SaveData(ref GameData data)
	{
		data.CurrentPlayerMovementStateType = this.CurrentPlayerMovementStateType;
		data.PlayerPosition = this.PlayerTransform.position;
		data.PlayerRotation = this.PlayerTransform.rotation;
	}

	public void LoadData(GameData data)
	{
		this.CurrentPlayerMovementStateType = data.CurrentPlayerMovementStateType;
		this.PlayerTransform.position = data.PlayerPosition;
		this.PlayerTransform.rotation = data.PlayerRotation;

		playerMovementStateType = (PlayerMovementStateType)Enum.Parse(typeof(PlayerMovementStateType), CurrentPlayerMovementStateType);
		SetPlayerMovementState(playerMovementStateType);
	}
}
