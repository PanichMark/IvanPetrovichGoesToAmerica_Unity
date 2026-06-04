using UnityEngine;
using System.Collections;

public class PlayerMovementController : MonoBehaviour, ISaveLoad
{
	public event System.Action<PlayerMovementStateTypes> OnPlayerMovementStateChanged;
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerBehaviourController _playerBehaviour;

	private Camera _playerCamera;

	private Vector3 _playerWorldMovement;

	private Vector3 _playerMovement;

	private Vector3 _projection;
	private Vector3 _correctedMovement;
	private GameSceneManager _gameSceneManager;
	public bool IsAbleToChangeMovementType {  get; private set; }

	private bool _jumpWaitOnSlope = false; 

	public Transform PlayerTransform {  get; private set; }
	public Rigidbody PlayerRigidBody { get; private set; }

	private string _currentPlayerCameraType = "";

	private Vector3 _playerPreviousFramePosition;
	public Vector3 PlayerPreviousFramePositionChange { get; private set; }

	private RaycastHit _hitInfo;

	public float PlayerMovementSpeed { get; private set; }
	public float PlayerRotationSpeed { get; private set; }

	public float PlayerSlidingSpeed { get; private set; }

	public float PlayerCurrentHeight { get; private set; }

	public bool IsPlayerJumping { get; private set; }
	public bool IsPlayerSliding { get; private set; }
	public bool IsPlayerGrounded { get; private set; }
	public bool IsPlayerCrouching { get; private set; }
	public bool IsPlayerAbleToStandUp { get; private set; }
	public bool IsPlayerFalling { get; private set; }

	public bool IsPlayerAbleToClimbLedge { get; private set; }
	public bool IsPlayerOnSlope { get; private set; }
	public bool IsPlayerLedgeClimbing { get; private set; }

	public float PlayerUpRayYPosition { get; private set; }
	public float PlayerDownRayYPosition { get; private set; } = 0.1f;
	private float _playerFloorDetectionRayCastLength;
	
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

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		PlayerBehaviourController playerBehaviour)
	{
		_bootstrap = bootstrap;
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_playerBehaviour = playerBehaviour;
		_playerCamera = Camera.main;

		PlayerRotationSpeed = 300f;

		PlayerTransform = GetComponent<Transform>();
		PlayerRigidBody = GetComponent<Rigidbody>();

		_playerPreviousFramePosition = transform.position;
		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerPosition(new Vector3(0, 0, -5));

		IsAbleToChangeMovementType = true;

		PlayerMovementSpeed = 3f;

		PlayerSlidingSpeed = 7.5f;

		PlayerCurrentHeight = 1.75f;

		_playerFloorDetectionRayCastLength = 0.3f;

		Debug.Log("PlayerMovementController Initialized");
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		IsPlayerGrounded = Physics.Raycast(transform.position + new Vector3(0, PlayerDownRayYPosition, 0), Vector3.down, out _hitInfo, _playerFloorDetectionRayCastLength);
		IsPlayerAbleToStandUp = !Physics.Raycast(transform.position + new Vector3(0, PlayerUpRayYPosition, 0), Vector3.up, out _hitInfo, 0.3f);
		IsPlayerFalling = (PlayerPreviousFramePositionChange.y < -0.01f && IsPlayerGrounded == false);

		if (!IsPlayerGrounded && !IsPlayerFalling)
		{
			IsPlayerJumping = true;
		}
		else
		{
			IsPlayerJumping = false;
		}

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

		if (isAllBoxesColliding && (isBigRectangleClear || isSmallRectangleClear) && !IsPlayerLedgeClimbing)
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
			PlayerRigidBody.useGravity = false;

			if (IsPlayerJumping || IsPlayerSliding)
			{
				
			}
			else PlayerRigidBody.linearVelocity = Vector3.zero;
		}
        else
        {
			PlayerRigidBody.useGravity = true;
		}

		if (IsPlayerOnSlope == true)
		{
			_correctedMovement = _playerMovement * PlayerMovementSpeed * Time.deltaTime;
			_projection = Vector3.Project(_correctedMovement, _hitInfo.normal);
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + _correctedMovement - _projection);
		}
		else
		{
			PlayerRigidBody.MovePosition(PlayerRigidBody.position + _playerMovement * PlayerMovementSpeed * Time.deltaTime);
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

	public void ChangePlayerRayPosition(float up)
	{
		PlayerUpRayYPosition = up;
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


	public float ChangePlayerMovementSpeed(float SetSpeed)
	{
		PlayerMovementSpeed = SetSpeed;
		return PlayerMovementSpeed;
	}

	public void ChangePlayerRotationSpeed(float speed)
	{
		PlayerRotationSpeed = speed;
	}

	public void StopPlayerRigidBpdyVelocity()
	{
		PlayerRigidBody.angularVelocity = Vector3.zero;
	}

	public void SetPlayerFloorDetectionRayCastLengthToDefault()
	{
		_playerFloorDetectionRayCastLength = 0.3f;
	}

	public void SetPlayerFloorDetectionRayCastLengthToZero()
	{
		_playerFloorDetectionRayCastLength = 0;
	}

	IEnumerator PlayerSlidingCourutine()
	{
		IsPlayerSliding = true;

		if (IsPlayerOnSlope)
		{
			Vector3 horizontalPlaneNormal = Vector3.ProjectOnPlane(_playerMovement, _hitInfo.normal);

			Vector3 slopeCorrection = Vector3.Project(horizontalPlaneNormal, _hitInfo.normal);

			Vector3 finalMovementDir = (horizontalPlaneNormal + slopeCorrection).normalized;

			PlayerRigidBody.AddForce(finalMovementDir * PlayerSlidingSpeed / 1.75f, ForceMode.Impulse);
		}
		else
		{
			PlayerRigidBody.AddForce(_playerMovement * PlayerSlidingSpeed, ForceMode.Impulse);
		}

		yield return new WaitForSeconds(1f);

		PlayerRigidBody.AddForce(Vector3.zero, ForceMode.Acceleration);
		PlayerRigidBody.linearVelocity = Vector3.zero;
		PlayerRigidBody.angularVelocity = Vector3.zero;
		PlayerRigidBody.MovePosition(PlayerRigidBody.transform.position);

		OnPlayerMovementStateChanged?.Invoke(PlayerMovementStateTypes.PlayerCrouchingIdle);
		IsPlayerSliding = false;
	}

	public void StartPlayerSliding()
	{
		StartCoroutine(PlayerSlidingCourutine());
	}

	IEnumerator PlayerLedgeClimbingCourutine()
	{
		IsPlayerLedgeClimbing = true;
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

		if (Big == true)
		{
			ChangePlayerRayPosition(1.9f);
			OnPlayerMovementStateChanged?.Invoke(PlayerMovementStateTypes.PlayerIdle);
		}
		else
		{
			OnPlayerMovementStateChanged?.Invoke(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}

		IsPlayerLedgeClimbing = false;
	}

	public void StartPlayerLedgeClimbing()
	{
		StartCoroutine(PlayerLedgeClimbingCourutine());
	}

	public IEnumerator DisablePlayerMovementDuringLegKickAttack()
	{
		_playerWorldMovement.z = 0;
		_playerWorldMovement.x = 0;

		IsAbleToChangeMovementType = false;

		yield return new WaitForSeconds(0.9f);
		IsAbleToChangeMovementType = true;
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
		data.PlayerPosition = PlayerTransform.position;
		data.PlayerRotation = PlayerTransform.rotation;
	}

	public void LoadData(GameData data)
	{
		PlayerTransform.position = data.PlayerPosition;
		PlayerTransform.rotation = data.PlayerRotation;
	}
}