using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
public class PlayerMovementController : MonoBehaviour, ISaveLoad
{
	private IInputDevice inputDevice;
	private PlayerBehaviour playerBehaviour;

	private Camera playerCamera;

	private AbstractPlayerMovementState playerMovementState;
	private PlayerMovementStateTypes playerMovementStateType;

	private Vector3 playerWorldMovement;

	private Vector3 PlayerMovement;

	private Vector3 projection;
	private Vector3 correctedMovement;

	private bool isAbleToChangeMovementType;

	private bool JumpWaitOnSlope = false; // Флаг готовности прыжка

	private Transform PlayerTransform;
	private Rigidbody PlayerRigidBody;

	private string currentPlayerCameraType = "";

	//private Vector3 PlayerMovementDirectionWithCamera;
	private Vector3 _playerPreviousFramePosition;
	public Vector3 PlayerPreviousFramePositionChange { get; private set; }

	private RaycastHit hitInfo;

	public string CurrentPlayerMovementStateType { get; private set; } = "PlayerIdle";

	public float PlayerMovementSpeed { get; private set; }
	public float PlayerRotationSpeed { get; private set; }

	public float PlayerSlidingSpeed { get; private set; }

	public float PlayerCurrentHeight { get; private set; }
	//public float PlayerStandingHeight { get; private set; }
	//public float PlayerCrouchingHeight { get; private set; }

	//public bool IsPlayerMoving { get; private set; }
	//public bool IsPlayerAbleToMove { get; private set; }
	public bool IsPlayerGrounded { get; private set; }
	public bool IsPlayerCrouching { get; private set; }
	public bool IsPlayerAbleToStandUp { get; private set; }
	public bool IsPlayerFalling { get; private set; }
	//public bool IsPLayerSliding { get; private set; }
	//public bool IsPlayerAbleToSlide { get; private set; }
	public bool IsPlayerAbleToClimbLedge { get; private set; }
	public bool IsPlayerOnSlope { get; private set; }

	public float PlayerUpRayYPosition { get; private set; }
	public float PlayerDownRayYPosition { get; private set; } = 0.1f;
	private float HowMuchUp;
	//private float angle;
	//private float moveFactor;


	//public bool IsPlayerLegKicking { get; private set; }

	

	void Start()
	{
	
		//PlayerCrouchingHeight = 1;
		//PlayerStandingHeight = 1.75f;

		//IsPlayerAbleToSlide = true;

		
	}

	public void ChangePlayerRotationSpeed(float speed)
	{
		PlayerRotationSpeed = speed;
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

	public void ChangePlayerRayPosition(float up)
	{
		
		PlayerUpRayYPosition = up;
	}
	private bool _isInitialized = false;
	void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
		//Debug.Log(playerCamera.transform.eulerAngles.y);

		playerMovementState.Update();

		IsPlayerGrounded = Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out hitInfo, HowMuchUp);
		IsPlayerAbleToStandUp = !Physics.Raycast(transform.position + new Vector3(0, PlayerUpRayYPosition, 0), Vector3.up, out hitInfo, 0.3f);
		IsPlayerFalling = (PlayerPreviousFramePositionChange.y < -0.01f && IsPlayerGrounded == false);

	
		
		

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


		if (isAllBoxesColliding && (isBigRectangleClear || isSmallRectangleClear) && playerMovementStateType != PlayerMovementStateTypes.PlayerLedgeClimbing)
		{
			IsPlayerAbleToClimbLedge = true;
		}
		else
		{
			IsPlayerAbleToClimbLedge = false;
		}

		//Debug.Log(IsPlayerAbleToClimbLedge);

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

		//Debug.Log(PlayerRigidBody.linearVelocity);








		if (IsPlayerOnSlope == true)
		{
			correctedMovement = PlayerMovement * PlayerMovementSpeed * Time.deltaTime;
			projection = Vector3.Project(correctedMovement, hitInfo.normal);
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + correctedMovement - projection);
			//var newPosition = PlayerRigidBody.position + correctedMovement - projection;
			//PlayerRigidBody.MovePosition(newPosition);
		}
		else
		{
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + PlayerMovement * PlayerMovementSpeed * Time.deltaTime);
			//var newPosition = PlayerRigidBody.position + PlayerMovement * PlayerMovementSpeed * Time.deltaTime;
			//PlayerRigidBody.MovePosition(newPosition);
		}













		//
		var PlayerMovementDirectionWithCamera = (playerWorldMovement.z * playerCamera.transform.forward + playerWorldMovement.x * playerCamera.transform.right);
		PlayerMovement = new Vector3(PlayerMovementDirectionWithCamera.x, 0, PlayerMovementDirectionWithCamera.z);
		PlayerMovement.Normalize();

		//angle = Vector3.Angle(hitInfo.normal, Vector3.up);
		//moveFactor = 1 / Mathf.Cos(Mathf.Deg2Rad * angle);

		//Debug.Log(PlayerMovement);







		if (playerBehaviour.IsPlayerArmed == false && (PlayerMovement != Vector3.zero) && (currentPlayerCameraType == PlayerCameraStateTypes.ThirdPerson.ToString()))
		{
			Quaternion CharacterRotation = Quaternion.LookRotation(PlayerMovement, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, CharacterRotation, PlayerRotationSpeed * Time.deltaTime);
			//Debug.Log("3333");
			//Debug.Log(transform.rotation);
		}
		else if (playerBehaviour.IsPlayerArmed == true || (currentPlayerCameraType == PlayerCameraStateTypes.FirstPerson.ToString()))
		{
			Quaternion PlayerRotateWhereCameraIsLooking = Quaternion.Euler(transform.localEulerAngles.x, playerCamera.transform.eulerAngles.y, transform.localEulerAngles.z);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, PlayerRotateWhereCameraIsLooking, PlayerRotationSpeed * Time.deltaTime);
			//Debug.Log("1111");
			//Debug.Log(transform.rotation);
		}
		//Debug.Log(transform.rotation);
		


	
	}


	public void SetPlayerWorldMovement(Vector3 newMovement)
	{
		playerWorldMovement = newMovement;
	}

	private void FixedUpdate()
	{
		PlayerPreviousFramePositionChange = transform.position - _playerPreviousFramePosition;
		_playerPreviousFramePosition = transform.position;
	}

	// Different player movement states scripts call this function
	public void SetPlayerMovementState(PlayerMovementStateTypes playerMovementStateType)
	{
		if (isAbleToChangeMovementType)
		{
			AbstractPlayerMovementState newState;

			if (playerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
			{
				PlayerRigidBody.angularVelocity = Vector3.zero;
				HowMuchUp = 0.3f;
				newState = new IdlePlayerMovementState(this, inputDevice, PlayerTransform, PlayerRigidBody);
				CurrentPlayerMovementStateType = "PlayerIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
			{
				newState = new WalkingPlayerMovementState(this, inputDevice, PlayerTransform, PlayerRigidBody);
				CurrentPlayerMovementStateType = "PlayerWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
			{
				newState = new RunningPlayerMovementState(this, inputDevice, PlayerTransform, PlayerRigidBody);
				CurrentPlayerMovementStateType = "PlayerRunning";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
			{
				HowMuchUp = 0;
				newState = new JumpingPlayerMovementState(this, inputDevice);
				CurrentPlayerMovementStateType = "PlayerJumping";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
			{
				HowMuchUp = 0.3f;
				newState = new FallingPlayerMovementState(this, inputDevice);
				CurrentPlayerMovementStateType = "PlayerFalling";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
			{
				PlayerRigidBody.angularVelocity = Vector3.zero;
				newState = new CrouchingIdlePlayerMovementState(this, inputDevice, PlayerTransform, PlayerRigidBody);
				CurrentPlayerMovementStateType = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
			{
				newState = new CrouchingWalkingPlayerMovementState(this, inputDevice, PlayerTransform, PlayerRigidBody);
				CurrentPlayerMovementStateType = "PlayerCrouchingWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerSliding)
			{
				newState = new SlidingPlayerMovementState(this);
				CurrentPlayerMovementStateType = "PlayerSliding";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbing)
			{
				newState = new LedgeClimbingPlayerMovementState(this);
				CurrentPlayerMovementStateType = "PlayerLedgeClimbing";
			}
			else
			{
				newState = null;
			}
			playerMovementState = newState;

			Debug.Log("MovementState: " + CurrentPlayerMovementStateType);
		}
	}

	// Different player movement states scripts call this function
	public float ChangePlayerMovementSpeed(float SetSpeed)
	{
		PlayerMovementSpeed = SetSpeed;
		return PlayerMovementSpeed;
	}

	IEnumerator PlayerSlidingCourutine()
	{
		if (IsPlayerOnSlope)
		{
			// Сначала получаем плоскость движения по нормали склона
			Vector3 horizontalPlaneNormal = Vector3.ProjectOnPlane(PlayerMovement, hitInfo.normal);

			// Добавляем небольшую поправку для учета уклона
			Vector3 slopeCorrection = Vector3.Project(horizontalPlaneNormal, hitInfo.normal);

			// Суммируем и нормализуем получившиеся векторы
			Vector3 finalMovementDir = (horizontalPlaneNormal + slopeCorrection).normalized;

			// Применяем импульс строго по этому направлению
			PlayerRigidBody.AddForce(finalMovementDir * PlayerSlidingSpeed / 1.75f, ForceMode.Impulse);
		}
		else
		{
			PlayerRigidBody.AddForce(PlayerMovement * PlayerSlidingSpeed, ForceMode.Impulse);
		}


		yield return new WaitForSeconds(1f);

		// Stop player in the sliding end
		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		//Debug.Log("bruh");
	}

	// State SlidingState calls this function with courutine as it itself is non Monobahaviour
	public void StartPlayerSliding()
	{
		StartCoroutine(PlayerSlidingCourutine());
		//Debug.Log("bruh");
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

		PlayerRigidBody.AddForce(transform.up * 7f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.3f);

		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		PlayerRigidBody.AddForce(transform.forward * 5f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.2f);
		
		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		// DECIDE if player will end up in standing or crouching position after ledge climbing
		if (Big == true)
		{
			//if (IsPlayerMoving == false)
			//{
			ChangePlayerRayPosition(1.9f);
			SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
			//}
		}
		else
		{
			//if (IsPlayerMoving == false)
			//{
				SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
			//}
		}
	}

	// State LedgeClimbingState calls this function with courutine as it itself is non Monobahaviour
	public void StartPlayerLedgeClimbing()
	{
		StartCoroutine(PlayerLedgeClimbingCourutine());
	}


	
	public IEnumerator DisablePlayerMovementDuringLegKickAttack()
	{
		playerWorldMovement.z = 0;
		playerWorldMovement.x = 0;
		//Debug.Log("Leg Kick Attack");
		isAbleToChangeMovementType = false;
		//IsPlayerLegKicking = true;

		//IsPlayerAbleToMove = false;

		yield return new WaitForSeconds(0.9f);
		isAbleToChangeMovementType = true;
		//IsPlayerAbleToMove = true;

		//IsPlayerLegKicking = false;
	}
	

	public bool JumpingStateWait()
	{
	
		StartCoroutine(JumpStateWaitCoroutine());
		return JumpWaitOnSlope; // Сразу вернем false, флаг изменится позже

	}

	public void StopJumpingStateWait()
	{
		StopCoroutine(JumpStateWaitCoroutine());
		JumpWaitOnSlope = false;
	}

	public IEnumerator JumpStateWaitCoroutine()
	{
		yield return new WaitForSeconds(0.5f); // Ждем 0.5 секунды
		JumpWaitOnSlope = true; // Готовность установлена
	}

	public void SetPlayerRotation(float rotationY)
	{
		transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
	}

	public void GiveCurrentPlayerCameraType(string cameraType)
	{
		currentPlayerCameraType = cameraType;
	}
	public void SetPlayerPosition(Vector3 position)
	{
		transform.position = position;
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

		playerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), CurrentPlayerMovementStateType);
		SetPlayerMovementState(playerMovementStateType);
	}


	public void Initialize(IInputDevice inputDevice, GameSceneManager gameSceneManager, PlayerBehaviour playerBehaviour)
	{
		this.gameSceneManager = gameSceneManager;
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour; // Новый аргумент
		playerCamera = Camera.main;

		PlayerRotationSpeed = 300f;

		PlayerTransform = GetComponent<Transform>();
		PlayerRigidBody = GetComponent<Rigidbody>();

		_playerPreviousFramePosition = transform.position;
		this.gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerPosition(new Vector3(0, 0, -5));
		this.gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		isAbleToChangeMovementType = true;
		SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		PlayerMovementSpeed = 3f;

		PlayerSlidingSpeed = 7.5f;

		PlayerCurrentHeight = 1.75f;
		_isInitialized = true;
		HowMuchUp = 0.3f;
		Debug.Log("PlayerMovement Initialized");



	}
	private GameSceneManager gameSceneManager;
}


