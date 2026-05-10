using UnityEngine;
using UnityEngine.AI;

public class WeaponPlungerCrossbow : WeaponAbstract
{
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private PlayerBehaviour _playerBehaviour;

	private GameObject _player;
	private GameObject _playerCollider;
	private GameObject _playerCamera;
	private Rigidbody _playerRigidbody;
	private bool _isPlayerPlungering = false;

	private GameObject _hookedObject = null;
	private Rigidbody _hookedObjectRigidbody = null;
	private bool _isObjectBeingHooked = false;
	private Collider _hookedObjectCollider = null;

	private NPCStateMachineController _NPCstateMachineController;
	private NPCAbstract _NPCabstract = null;
	private NavMeshAgent _hookedObjectNavMeshAgent = null;

	private Vector3 _hookPoint;
	private float _maxHookDistance = 17f;
	private float _pullSpeed = 12f;

	public override string WeaponNameSystem => "PlungerCrossbow";

	public override string WeaponNameUI => "Абордажный Арбалет";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponPlungerCrossbowIcon");
	public override float WeaponDamage => 0; 

	private void Start()
	{
		_playerCamera = ServiceLocator.Resolve<GameObject>("PlayerCameraGameObject");
		_player = ServiceLocator.Resolve<GameObject>("PlayerGameObject");
		_playerRigidbody = _player.GetComponent<Rigidbody>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_playerCollider = ServiceLocator.Resolve<GameObject>("PlayerColliderGameObject");
		
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_playerBehaviour = ServiceLocator.Resolve<PlayerBehaviour>("PlayerBehaviour");

		_gameSceneManager.OnBeginLoadMainMenuScene += StopPlungingCompletely;
		_gameSceneManager.OnBeginLoadGameplayScene += StopPlungingCompletely;
		_playerBehaviour.OnPlayerDisarmed += StopPlunging;
	}

	public override void WeaponAttack()
	{
		if (_isPlayerPlungering || _isObjectBeingHooked) return;

		if (_playerCamera == null || _player == null || _playerRigidbody == null) return;

		Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, _maxHookDistance))
		{
			_hookPoint = hit.point;

			if ((hit.collider.gameObject.TryGetComponent<NPCAbstract>(out _) ||
				hit.collider.gameObject.TryGetComponent<InteractionObjectPickableAbstract>(out _)))
			{
				_hookedObject = hit.collider.gameObject;
				_hookedObjectRigidbody = _hookedObject.GetComponent<Rigidbody>();
				_hookedObjectCollider = _hookedObject.GetComponent<Collider>();

				if (_hookedObjectRigidbody == null)
				{
					_hookedObjectRigidbody = _hookedObject.AddComponent<Rigidbody>();
					Debug.LogWarning($"Rigidbody добавлен к NPC: {_hookedObject.name}");
			
				}
				_hookedObjectNavMeshAgent = _hookedObject.GetComponent<NavMeshAgent>();
				if (_hookedObjectNavMeshAgent != null)
				{

					_hookedObjectNavMeshAgent.enabled = false;
				}
				_NPCabstract = _hookedObject?.GetComponent<NPCAbstract>();
				_NPCstateMachineController = _hookedObject?.GetComponent<NPCStateMachineController>();
				if (_NPCstateMachineController != null && _NPCabstract != null)
				{
					//////
					//ПОТОМ ПОМЕНЯТЬ НА BEING HOOKED !!!
					//////
					_NPCstateMachineController.SetNPCState(NPCStateTypes.Dead);
				}

				_hookedObjectRigidbody.useGravity = false;
				_hookedObjectRigidbody.linearDamping = 0;

				_isObjectBeingHooked = true;
				_isPlayerPlungering = false;
				Debug.Log("Крюк зацепил NPC!");
			}
			else 
			{
				_playerCollider.SetActive(false);
				_isPlayerPlungering = true; 
				_playerRigidbody.useGravity = false;
				_isObjectBeingHooked = false;
				Debug.Log($"Крюк зацепился за: {hit.collider.name}");
			}
		}
	}

	private void FixedUpdate()
	{
		if (_isObjectBeingHooked)
		{
			Vector3 finalPosition = _player.transform.position;
			finalPosition.y += 0.5f;
			finalPosition += _player.transform.forward * 1.5f;

			Vector3 directionToTarget = (finalPosition - _hookedObject.transform.position).normalized;

			_hookedObjectRigidbody.linearVelocity = directionToTarget * _pullSpeed;

			float distanceToTarget = Vector3.Distance(_hookedObject.transform.position, finalPosition);

			if (distanceToTarget < 0.3f)
			{
				_hookedObjectRigidbody.linearVelocity = Vector3.zero;
				_hookedObjectRigidbody.position = finalPosition;

				StopHookingObject();
			}
		}

		if (_isPlayerPlungering)
		{
			_gameController.PlayerStartedPlunging();
			Vector3 directionToHook = (_hookPoint - _player.transform.position).normalized;

			_playerRigidbody.linearVelocity = directionToHook * _pullSpeed;

			float distanceToHook = Vector3.Distance(_player.transform.position, _hookPoint);

			if (distanceToHook < 0.5f || _playerRigidbody.linearVelocity.magnitude < 0.1f)
			{
				_player.transform.position = _hookPoint;
				_playerRigidbody.linearVelocity = Vector3.zero;

				StopPlunging();
			}
		}
	}
	private void StopHookingObject()
	{
		if (_isObjectBeingHooked)
		{
			if (_hookedObjectNavMeshAgent != null)
			{
			//hookedObjectNavMeshAgent.enabled = true;
			}

			_hookedObjectRigidbody.linearVelocity = Vector3.zero;

			_hookedObjectRigidbody.useGravity = true;
			_hookedObjectCollider = null;
			_hookedObjectNavMeshAgent = null;
			_NPCabstract = null;
			_hookedObject = null;
			_hookedObjectRigidbody = null;
			_isObjectBeingHooked = false;
			Debug.Log("Притяжение NPC завершено.");
		}
	}

	private void OnDestroy()
	{
		_gameSceneManager.OnBeginLoadMainMenuScene -= StopPlungingCompletely;
		_gameSceneManager.OnBeginLoadGameplayScene -= StopPlungingCompletely;
		_playerBehaviour.OnPlayerDisarmed -= StopPlunging;

		StopPlunging();
	}

	private void StopPlunging()
	{
		if (_isPlayerPlungering)
		{
			_playerRigidbody.useGravity = true;

			_isPlayerPlungering = false;
			Debug.Log("Притяжение завершено.");
			_gameController.PlayerStoppedPlunging();
			_playerCollider.SetActive(true);
		}
	}

	private void StopPlungingCompletely()
	{
		_playerRigidbody.linearVelocity = Vector3.zero;
		StopPlunging();
	}
}