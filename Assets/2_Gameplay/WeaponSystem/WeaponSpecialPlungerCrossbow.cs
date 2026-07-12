using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WeaponSpecialPlungerCrossbow : WeaponAbstract
{
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private PlayerBehaviourController _playerBehaviour;

	private GameObject _projectile;
	private GameObject _projectileStringStartPoint;
	
	private Transform _projectileParent;
	private bool _isCrossbowAttacking;
	private GameObject _player;
	private GameObject _playerCollider;
	private GameObject _playerCamera;
	private Rigidbody _playerRigidbody;
	private Vector3 _projectileRestPosition;
	private Quaternion _projectileRestDirection;
	private Quaternion _projectileFlyingDirection;
	public override bool IsWeaponAuto => false;
	private GameObject _hookedObject = null;
	private Rigidbody _hookedObjectRigidbody = null;
	private Collider _hookedObjectCollider = null;

	private NPCStateMachineController _NPCstateMachineController;
	private NPCAbstract _NPCabstract = null;
	private NavMeshAgent _hookedObjectNavMeshAgent = null;

	private Vector3 _hookPoint;
	private float _maxHookDistance = 20f;
	private float _projectileSpeed = 15f;

	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "PlungerCrossbow";
	public override string WeaponType => WeaponTypes.Special.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 0;

	public override void InitializeWeapon()
	{
		_projectile = transform.Find("Projectile").gameObject;
		_projectileStringStartPoint = transform.Find("StartPoint").gameObject;
		_projectileParent = _projectile.transform.parent;

		_projectileRestPosition = _projectile.transform.localPosition;
		_projectileRestDirection = _projectile.transform.localRotation;
		Debug.Log(_projectileRestPosition);

		_playerCamera = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");

		_player = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_playerRigidbody = _player.GetComponent<Rigidbody>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_playerCollider = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		//Debug.Log(_gameSceneManager);
		_playerBehaviour = ServiceLocator.Resolve<PlayerBehaviourController>("PlayerBehaviour");

		_gameSceneManager.OnBeginLoadingMainMenuScene += FullStopPlunging;
		_gameSceneManager.OnBeginLoadingGameplayScene += FullStopPlunging;
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
			yield return StartCoroutine(HookObject(hit));
		}
		else
		{
			Debug.Log($"Крюк зацепился за: {hit.collider.name}");
			yield return StartCoroutine(PlungePlayer(point));
		}
	}

	private IEnumerator ShootProjectile(Vector3 point)
	{
		_projectile.transform.SetParent(null);
		_projectileFlyingDirection = _playerCamera.transform.rotation;
		_projectile.transform.rotation = _projectileFlyingDirection;

		while (Vector3.Distance(_projectile.transform.position, point) > 0.1f)
		{
			_projectile.transform.position = Vector3.MoveTowards(_projectile.transform.position, point, _projectileSpeed * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator PlungePlayer(Vector3 hookPoint)
	{
		_playerCollider.SetActive(false);

		_playerRigidbody.useGravity = false;

		while (_isCrossbowAttacking)
		{
			//Debug.Log("PLUNGING");

			Vector3 directionToHook = (hookPoint - _player.transform.position).normalized;
			_playerRigidbody.linearVelocity = directionToHook * _projectileSpeed;

			float distanceToHook = Vector3.Distance(_player.transform.position, hookPoint);

			if (distanceToHook < 0.2f)
			{
				_player.transform.position = hookPoint;
				_playerRigidbody.linearVelocity = Vector3.zero;
				
				StartCoroutine(ReturnProjectile());
				//_currentHookProjectile.transform.SetParent(_projectileOriginalParent);
				//_currentHookProjectile.transform.localPosition = Vector3.zero;
			}

			yield return null;
		}
	}

	private IEnumerator HookObject(RaycastHit hit)
	{
		_projectile.transform.SetParent(hit.collider.transform);

		ProccessHookedObject(hit);

		while (_isCrossbowAttacking)
		{
			Vector3 finalPosition = _player.transform.position;
			finalPosition.y += 0.5f;
			finalPosition += _player.transform.forward * 1.5f;

			Vector3 directionToTarget = (finalPosition - _hookedObject.transform.position).normalized;
			_hookedObjectRigidbody.linearVelocity = directionToTarget * _projectileSpeed;

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

	private void ProccessHookedObject(RaycastHit hit)
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

			//StartCoroutine(ReturnProjectile());
		}
	}

	private void OnDestroy()
	{
		// Проверка на null защищает от ошибки, если эти объекты были уничтожены
		//Debug.Log(_gameSceneManager);
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingMainMenuScene -= FullStopPlunging;
			_gameSceneManager.OnBeginLoadingGameplayScene -= FullStopPlunging;
		}


		if (_playerBehaviour != null)
		{
			_playerBehaviour.OnPlayerDisarmed -= StopPlunging;
		}


		OnCrossbowDestroyed();
	}

	private IEnumerator ReturnProjectile()
	{
		while (Vector3.Distance(_projectile.transform.position, _projectileRestPosition) > 0.001f)
		{
			//Debug.Log("RETURNING");
			_projectile.transform.position = Vector3.MoveTowards(_projectile.transform.position, _projectileRestPosition, _projectileSpeed * Time.deltaTime);
			
		}


		StopPlunging();

		yield return null;

	}

	private void OnCrossbowDestroyed()
	{
		if (_isCrossbowAttacking)
		{

			Destroy(_projectile);
			_playerRigidbody.useGravity = true;
			_isCrossbowAttacking = false;
			Debug.Log("Притяжение завершено.");
			_gameController.PlayerStoppedPlunging();
			_playerCollider.SetActive(true);
		}
	}

	private void StopPlunging()
	{
		if (_isCrossbowAttacking)
		{
			_projectile.transform.SetParent(_projectileParent);
			_projectile.transform.localPosition = _projectileRestPosition;
			_projectile.transform.localRotation = _projectileRestDirection;

			_playerRigidbody.useGravity = true;
			_isCrossbowAttacking = false;
			Debug.Log("Притяжение завершено.");
			_gameController.PlayerStoppedPlunging();
			_playerCollider.SetActive(true);
		}
	}

	private void FullStopPlunging()
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