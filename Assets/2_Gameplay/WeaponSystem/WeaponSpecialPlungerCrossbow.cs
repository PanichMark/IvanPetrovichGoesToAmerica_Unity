using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WeaponSpecialPlungerCrossbow : WeaponAbstract
{
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private PlayerBehaviourController _playerBehaviour;

	private GameObject _currentHookProjectile;
	private GameObject _lineStartPoint;
	private float _projectileSpeed = 10f;
	private Transform _projectileOriginalParent;
	private bool _isCrossbowAttacking;
	private GameObject _player;
	private GameObject _playerCollider;
	private GameObject _playerCamera;
	private Rigidbody _playerRigidbody;
	private Vector3 _currentHookProjectileOriginalPosition;
	private Quaternion _currentHookProjectileOriginalRotation;
	private Quaternion _projectileFlyingRotation;
	public override bool IsWeaponAuto => false;
	private GameObject _hookedObject = null;
	private Rigidbody _hookedObjectRigidbody = null;
	private Collider _hookedObjectCollider = null;

	private NPCStateMachineController _NPCstateMachineController;
	private NPCAbstract _NPCabstract = null;
	private NavMeshAgent _hookedObjectNavMeshAgent = null;

	private Vector3 _hookPoint;
	private float _maxHookDistance = 17f;
	private float _pullSpeed = 12f;

	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "PlungerCrossbow";
	public override string WeaponType => WeaponTypes.Special.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 0;

	public override void InitializeWeapon()
	{
		_currentHookProjectile = transform.Find("Projectile").gameObject;
		_lineStartPoint = transform.Find("StartPoint").gameObject;
		_projectileOriginalParent = _currentHookProjectile.transform.parent;

		_currentHookProjectileOriginalPosition = _currentHookProjectile.transform.localPosition;
		_currentHookProjectileOriginalRotation = _currentHookProjectile.transform.localRotation;
		Debug.Log(_currentHookProjectileOriginalPosition);

		_playerCamera = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");

		_player = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_playerRigidbody = _player.GetComponent<Rigidbody>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_playerCollider = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_playerBehaviour = ServiceLocator.Resolve<PlayerBehaviourController>("PlayerBehaviour");

		_gameSceneManager.OnBeginLoadingMainMenuScene += StopPlungingCompletely;
		_gameSceneManager.OnBeginLoadingGameplayScene += StopPlungingCompletely;
		_playerBehaviour.OnPlayerDisarmed += StopPlunging;
	}

	public override void WeaponAttack()
	{
		if (_isCrossbowAttacking) return;

		Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, _maxHookDistance, ~LayerMask.GetMask("InvisibleWall")))
		{
			_hookPoint = hit.point;
			_isCrossbowAttacking = true;
			StartCoroutine(PerformCrossbowShoot(_hookPoint, hit));
		}
	}

	private IEnumerator PerformCrossbowShoot(Vector3 point, RaycastHit hit)
	{

		_gameController.PlayerStartedPlunging();
			
		yield return StartCoroutine(ShootProjectile(point));

		if ((hit.collider.gameObject.TryGetComponent<NPCAbstract>(out _) ||
			hit.collider.gameObject.TryGetComponent<InteractionObjectPickableAbstract>(out _)))
		{

			Debug.Log("Крюк зацепил предмет/NPC");

			_currentHookProjectile.transform.SetParent(hit.collider.transform);

			yield return StartCoroutine(HookObject(hit));
			StartCoroutine(ReturnProjectile());
			yield return StartCoroutine(PullObjectRoutine());



		}
		else
		{
			_playerCollider.SetActive(false);
			
			_playerRigidbody.useGravity = false;
			
			Debug.Log($"Крюк зацепился за: {hit.collider.name}");



			//StartCoroutine(ReturnProjectile());
			yield return StartCoroutine(PullPlayerRoutine(point));
		}
	}

	private IEnumerator ShootProjectile(Vector3 point)
	{
		_currentHookProjectile.transform.SetParent(null);
		_projectileFlyingRotation = _playerCamera.transform.rotation;
		_currentHookProjectile.transform.rotation = _projectileFlyingRotation;

		while (Vector3.Distance(_currentHookProjectile.transform.position, point) > 0.1f)
		{
			_currentHookProjectile.transform.position = Vector3.MoveTowards(_currentHookProjectile.transform.position, point, _projectileSpeed * Time.deltaTime);
			yield return null;
		}

		
	}

	private IEnumerator PullPlayerRoutine(Vector3 hookPoint)
	{
		while (_isCrossbowAttacking)
		{
			//Debug.Log("PLUNGING");

			Vector3 directionToHook = (hookPoint - _player.transform.position).normalized;
			_playerRigidbody.linearVelocity = directionToHook * _pullSpeed;

			float distanceToHook = Vector3.Distance(_player.transform.position, hookPoint);

			if (distanceToHook < 0.5f)
			{
				_player.transform.position = hookPoint;
				_playerRigidbody.linearVelocity = Vector3.zero;
				StopPlunging();
				StartCoroutine(ReturnProjectile());
				//_currentHookProjectile.transform.SetParent(_projectileOriginalParent);
				//_currentHookProjectile.transform.localPosition = Vector3.zero;
			}
			yield return null;
		}
	}

	private IEnumerator PullObjectRoutine()
	{
		while (_isCrossbowAttacking)
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

				//_currentHookProjectile.transform.SetParent(_projectileOriginalParent);
				//_currentHookProjectile.transform.localPosition = Vector3.zero;
			}
			yield return null;
		}
	}

	private IEnumerator HookObject(RaycastHit hit)
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

		yield return null;
	}

	private void StopHookingObject()
	{
		if (_isCrossbowAttacking)
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
			_isCrossbowAttacking = false;
			Debug.Log("Притяжение NPC завершено.");
		}
	}

	private void OnDestroy()
	{
		// Проверка на null защищает от ошибки, если эти объекты были уничтожены
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingMainMenuScene -= StopPlungingCompletely;
			_gameSceneManager.OnBeginLoadingGameplayScene -= StopPlungingCompletely;
		}

		if (_playerBehaviour != null)
		{
			_playerBehaviour.OnPlayerDisarmed -= StopPlunging;
		}

		StopPlunging();
	}

	private IEnumerator ReturnProjectile()
	{
		while (Vector3.Distance(_currentHookProjectile.transform.position, _currentHookProjectileOriginalPosition) > 0.001f)
		{
			//Debug.Log("RETURNING");
			_currentHookProjectile.transform.position = Vector3.MoveTowards(_currentHookProjectile.transform.position, _currentHookProjectileOriginalPosition, _pullSpeed * Time.deltaTime);
			
		}
		_currentHookProjectile.transform.SetParent(_projectileOriginalParent);
		_currentHookProjectile.transform.localPosition = _currentHookProjectileOriginalPosition;
		_currentHookProjectile.transform.localRotation = _currentHookProjectileOriginalRotation;

		yield return null;

	}

	private void StopPlunging()
	{
		if (_isCrossbowAttacking)
		{
			_playerRigidbody.useGravity = true;
			_isCrossbowAttacking = false;
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

	public override void StopAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override void StartAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override IEnumerator AutoAttackCourutine()
	{
		//throw new System.NotImplementedException();
		yield return null;
	}
}