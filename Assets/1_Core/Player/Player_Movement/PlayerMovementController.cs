using UnityEngine;
using System.Collections;
using System;
public class PlayerMovementController : MonoBehaviour, ISaveLoad
{
	private IInputDevice _inputDevice;
	private PlayerBehaviour _playerBehaviour;

	private Camera _playerCamera;

	private AbstractPlayerMovementState _playerMovementState;
	private PlayerMovementStateTypes _playerMovementStateType;

	private Vector3 _playerWorldMovement;

	private Vector3 _playerMovement;

	private Vector3 _projection;
	private Vector3 _correctedMovement;
	private GameSceneManager _gameSceneManager;
	private bool _isAbleToChangeMovementType;

	private bool _jumpWaitOnSlope = false; 

	private Transform _playerTransform;
	private Rigidbody _playerRigidBody;

	private string _currentPlayerCameraType = "";

	private Vector3 _playerPreviousFramePosition;
	public Vector3 PlayerPreviousFramePositionChange { get; private set; }

	private RaycastHit _hitInfo;

	public string CurrentPlayerMovementStateType { get; private set; } = "PlayerIdle";

	public float PlayerMovementSpeed { get; private set; }
	public float PlayerRotationSpeed { get; private set; }

	public float PlayerSlidingSpeed { get; private set; }

	public float PlayerCurrentHeight { get; private set; }

	public bool IsPlayerGrounded { get; private set; }
	public bool IsPlayerCrouching { get; private set; }
	public bool IsPlayerAbleToStandUp { get; private set; }
	public bool IsPlayerFalling { get; private set; }

	public bool IsPlayerAbleToClimbLedge { get; private set; }
	public bool IsPlayerOnSlope { get; private set; }

	public float PlayerUpRayYPosition { get; private set; }
	public float PlayerDownRayYPosition { get; private set; } = 0.1f;
	private float _howMuchUp; //?????

	private bool _isInitialized = false;

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
	
	void Update()
	{
		if (!_isInitialized)
			return;
	
		_playerMovementState.Update();

		IsPlayerGrounded = Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out _hitInfo, _howMuchUp);
		IsPlayerAbleToStandUp = !Physics.Raycast(transform.position + new Vector3(0, PlayerUpRayYPosition, 0), Vector3.up, out _hitInfo, 0.3f);
		IsPlayerFalling = (PlayerPreviousFramePositionChange.y < -0.01f && IsPlayerGrounded == false);

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

		if (isAllBoxesColliding && (isBigRectangleClear || isSmallRectangleClear) && _playerMovementStateType != PlayerMovementStateTypes.PlayerLedgeClimbing)
		{
			IsPlayerAbleToClimbLedge = true;
		}
		else
		{
			IsPlayerAbleToClimbLedge = false;
		}

		if (Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out _hitInfo, 0.3f))
		{
			if (_hitInfo.normal != Vector3.up)
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
			_playerRigidBody.useGravity = false;

			if (CurrentPlayerMovementStateType == "PlayerJumping" || CurrentPlayerMovementStateType == "PlayerSliding")
			{
				
			}
			else _playerRigidBody.linearVelocity = Vector3.zero;
		}
        else
        {
			_playerRigidBody.useGravity = true;
		}

		if (IsPlayerOnSlope == true)
		{
			_correctedMovement = _playerMovement * PlayerMovementSpeed * Time.deltaTime;
			_projection = Vector3.Project(_correctedMovement, _hitInfo.normal);
			_playerRigidBody.MovePosition(_playerRigidBody.position + _correctedMovement - _projection);
		}
		else
		{
			_playerRigidBody.MovePosition(_playerRigidBody.position + _playerMovement * PlayerMovementSpeed * Time.deltaTime);
		}

		var PlayerMovementDirectionWithCamera = (_playerWorldMovement.z * _playerCamera.transform.forward + _playerWorldMovement.x * _playerCamera.transform.right);
		_playerMovement = new Vector3(PlayerMovementDirectionWithCamera.x, 0, PlayerMovementDirectionWithCamera.z);
		_playerMovement.Normalize();

		if (_playerBehaviour.IsPlayerArmed == false && (_playerMovement != Vector3.zero) && (_currentPlayerCameraType == PlayerCameraStateTypes.ThirdPerson.ToString()))
		{
			Quaternion CharacterRotation = Quaternion.LookRotation(_playerMovement, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, CharacterRotation, PlayerRotationSpeed * Time.deltaTime);
		}
		else if (_playerBehaviour.IsPlayerArmed == true || (_currentPlayerCameraType == PlayerCameraStateTypes.FirstPerson.ToString()))
		{
			Quaternion PlayerRotateWhereCameraIsLooking = Quaternion.Euler(transform.localEulerAngles.x, _playerCamera.transform.eulerAngles.y, transform.localEulerAngles.z);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, PlayerRotateWhereCameraIsLooking, PlayerRotationSpeed * Time.deltaTime);
		}
	}

	public void SetPlayerWorldMovement(Vector3 newMovement)
	{
		_playerWorldMovement = newMovement;
	}

	private void FixedUpdate()
	{
		PlayerPreviousFramePositionChange = transform.position - _playerPreviousFramePosition;
		_playerPreviousFramePosition = transform.position;
	}

	public void SetPlayerMovementState(PlayerMovementStateTypes playerMovementStateType)
	{
		if (_isAbleToChangeMovementType)
		{
			AbstractPlayerMovementState newState;

			if (playerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
			{
				_playerRigidBody.angularVelocity = Vector3.zero;
				_howMuchUp = 0.3f;
				newState = new IdlePlayerMovementState(this, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
			{
				newState = new WalkingPlayerMovementState(this, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
			{
				newState = new RunningPlayerMovementState(this, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerRunning";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
			{
				_howMuchUp = 0;
				newState = new JumpingPlayerMovementState(this, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerJumping";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
			{
				_howMuchUp = 0.3f;
				newState = new FallingPlayerMovementState(this, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerFalling";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
			{
				_playerRigidBody.angularVelocity = Vector3.zero;
				newState = new CrouchingIdlePlayerMovementState(this, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
			{
				newState = new CrouchingWalkingPlayerMovementState(this, _inputDevice, _playerTransform, _playerRigidBody);
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
			_playerMovementState = newState;

			Debug.Log("MovementState: " + CurrentPlayerMovementStateType);
		}
	}

	public float ChangePlayerMovementSpeed(float SetSpeed)
	{
		PlayerMovementSpeed = SetSpeed;
		return PlayerMovementSpeed;
	}

	IEnumerator PlayerSlidingCourutine()
	{
		if (IsPlayerOnSlope)
		{
			Vector3 horizontalPlaneNormal = Vector3.ProjectOnPlane(_playerMovement, _hitInfo.normal);

			Vector3 slopeCorrection = Vector3.Project(horizontalPlaneNormal, _hitInfo.normal);

			Vector3 finalMovementDir = (horizontalPlaneNormal + slopeCorrection).normalized;

			_playerRigidBody.AddForce(finalMovementDir * PlayerSlidingSpeed / 1.75f, ForceMode.Impulse);
		}
		else
		{
			_playerRigidBody.AddForce(_playerMovement * PlayerSlidingSpeed, ForceMode.Impulse);
		}

		yield return new WaitForSeconds(1f);

		_playerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		_playerRigidBody.linearVelocity = Vector3.zero;
		_playerRigidBody.angularVelocity = Vector3.zero;
		_playerRigidBody.MovePosition(_playerRigidBody.transform.position);

		SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
	}

	public void StartPlayerSliding()
	{
		StartCoroutine(PlayerSlidingCourutine());
	}

	IEnumerator PlayerLedgeClimbingCourutine()
	{
		bool Big;

		if (Physics.OverlapBox(transform.position + transform.forward * 1.1f + new Vector3(0, 3, 0), new Vector3(1.25f, 2.25f, 1.25f) * 0.5f, Quaternion.identity).Length > 0)
		{
			Big = false;
		}
		else Big = true;

		_playerRigidBody.AddForce(transform.up * 7f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.3f);

		_playerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		_playerRigidBody.linearVelocity = Vector3.zero;
		_playerRigidBody.angularVelocity = Vector3.zero;
		_playerRigidBody.MovePosition(_playerRigidBody.transform.position);

		_playerRigidBody.AddForce(transform.forward * 5f, ForceMode.Impulse);

		yield return new WaitForSeconds(0.2f);
		
		_playerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		_playerRigidBody.linearVelocity = Vector3.zero;
		_playerRigidBody.angularVelocity = Vector3.zero;
		_playerRigidBody.MovePosition(_playerRigidBody.transform.position);

		if (Big == true)
		{
			ChangePlayerRayPosition(1.9f);
			SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}
		else
		{
			SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
	}

	public void StartPlayerLedgeClimbing()
	{
		StartCoroutine(PlayerLedgeClimbingCourutine());
	}

	public IEnumerator DisablePlayerMovementDuringLegKickAttack()
	{
		_playerWorldMovement.z = 0;
		_playerWorldMovement.x = 0;

		_isAbleToChangeMovementType = false;

		yield return new WaitForSeconds(0.9f);
		_isAbleToChangeMovementType = true;
	}
	
	public bool JumpingStateWait()
	{
		StartCoroutine(JumpStateWaitCoroutine());
		return _jumpWaitOnSlope;

	}

	public void StopJumpingStateWait()
	{
		StopCoroutine(JumpStateWaitCoroutine());
		_jumpWaitOnSlope = false;
	}

	public IEnumerator JumpStateWaitCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		_jumpWaitOnSlope = true; 
	}

	public void SetPlayerRotation(float rotationY)
	{
		transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
	}

	public void GiveCurrentPlayerCameraType(string cameraType)
	{
		_currentPlayerCameraType = cameraType;
	}
	public void SetPlayerPosition(Vector3 position)
	{
		transform.position = position;
	}
	public void SaveData(ref GameData data)
	{
		data.CurrentPlayerMovementStateType = CurrentPlayerMovementStateType;
		data.PlayerPosition = _playerTransform.position;
		data.PlayerRotation = _playerTransform.rotation;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerMovementStateType = data.CurrentPlayerMovementStateType;
		_playerTransform.position = data.PlayerPosition;
		_playerTransform.rotation = data.PlayerRotation;

		_playerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), CurrentPlayerMovementStateType);
		SetPlayerMovementState(_playerMovementStateType);
	}

	public void Initialize(IInputDevice inputDevice, GameSceneManager gameSceneManager, PlayerBehaviour playerBehaviour)
	{
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_playerBehaviour = playerBehaviour; 
		_playerCamera = Camera.main;

		PlayerRotationSpeed = 300f;

		_playerTransform = GetComponent<Transform>();
		_playerRigidBody = GetComponent<Rigidbody>();

		_playerPreviousFramePosition = transform.position;
		_gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerPosition(new Vector3(0, 0, -5));
		_gameSceneManager.OnBeginLoadMainMenuScene += () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		_isAbleToChangeMovementType = true;
		SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		PlayerMovementSpeed = 3f;

		PlayerSlidingSpeed = 7.5f;

		PlayerCurrentHeight = 1.75f;
		_isInitialized = true;
		_howMuchUp = 0.3f;
		Debug.Log("PlayerMovement Initialized");

	}
}