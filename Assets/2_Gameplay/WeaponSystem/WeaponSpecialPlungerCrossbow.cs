using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeaponSpecialPlungerCrossbow : WeaponAbstract
{
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private PlayerBehaviourController _playerBehaviour;

	//private GameObject _currentProjectile;

	private GameObject _projectile1stPerson;
	private Transform _projectileParent1stPerson;
	private GameObject _projectileStringStartPoint1stPerson;
	private GameObject _projectileStringEndPoint1stPerson;
	private LineRenderer _lineRenderer1stPerson;
	private PlayerWeaponFirstPersonRenderer _playerWeaponFirstPersonRenderer;
	private GameObject _projectile3rdPerson;
	private Transform _projectileParent3rdPerson;
	private GameObject _projectileStringStartPoint3rdPerson;
	private GameObject _projectileStringEndPoint3rdPerson;
	private LineRenderer _lineRenderer3rdPerson;

	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private bool _isCrossbowAttacking;
	private GameObject _player;
	private GameObject _playerCollider;
	private GameObject _playerCamera;
	private Rigidbody _playerRigidbody;
	private Vector3 _projectile1stPersonRestPosition;
	private Quaternion _projectile1stPersonRestDirection;
	private Vector3 _projectile3rdPersonRestPosition;
	private Quaternion _projectile3rdPersonRestDirection;


	private Quaternion _projectileFlyingDirection;
	public override bool IsWeaponAuto => false;
	private GameObject _hookedObject;
	private Rigidbody _hookedObjectRigidbody;
	private Collider _hookedObjectCollider;

	private NPCStateMachineController _NPCstateMachineController;
	private NPCAbstract _NPCabstract;
	private NavMeshAgent _hookedObjectNavMeshAgent;

	private Vector3 _hookPoint;
	private float _maxHookDistance = 20f;
	private float _projectileSpeed = 15f;

	public override WeaponNames WeaponName => WeaponNames.PlungerCrossbow;
	public override WeaponTypes WeaponType => WeaponTypes.Special;
	public override float WeaponDamage => 0;

	public override void InitializeWeapon()
	{
		_projectile1stPerson = FirstPersonWeaponModelInstance.transform.Find("Projectile").gameObject;
		_projectileParent1stPerson = _projectile1stPerson.transform.parent;
		_projectileStringStartPoint1stPerson = FirstPersonWeaponModelInstance.transform.Find("ProjectileStringStartPoint").gameObject;
		_projectileStringEndPoint1stPerson = _projectile1stPerson.transform.Find("ProjectileStringEndPoint").gameObject;
		_lineRenderer1stPerson = FirstPersonWeaponModelInstance.GetComponent<LineRenderer>();

		_projectile3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("Projectile").gameObject;
		_projectileParent3rdPerson = _projectile3rdPerson.transform.parent;
		_projectileStringStartPoint3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("ProjectileStringStartPoint").gameObject;
		_projectileStringEndPoint3rdPerson = _projectile3rdPerson.transform.Find("ProjectileStringEndPoint").gameObject;
		_lineRenderer3rdPerson = ThirdPersonWeaponModelInstance.GetComponent<LineRenderer>();

		_projectile1stPersonRestPosition = _projectile1stPerson.transform.localPosition;
		_projectile1stPersonRestDirection = _projectile1stPerson.transform.localRotation;

		_projectile3rdPersonRestPosition = _projectile3rdPerson.transform.localPosition;
		_projectile3rdPersonRestDirection = _projectile3rdPerson.transform.localRotation;
		//Debug.Log(_projectileRestPosition);
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
		_playerCamera = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
		_playerWeaponFirstPersonRenderer = ServiceLocator.Resolve<PlayerWeaponFirstPersonRenderer>("PlayerWeaponFirstPersonRenderer");
		_player = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_playerRigidbody = _player.GetComponent<Rigidbody>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_playerCollider = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		//Debug.Log(_gameSceneManager);
		_playerBehaviour = ServiceLocator.Resolve<PlayerBehaviourController>("PlayerBehaviour");

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			ChangeCrossbow1stPerson();
		}
		else
		{
			ChangeCrossbow3rdPerson();
		}

		_gameSceneManager.OnBeginLoadingMainMenuScene += FullStopPlunging;
		_gameSceneManager.OnBeginLoadingGameplayScene += FullStopPlunging;
		_playerBehaviour.OnPlayerDisarmed += StopCrossbowAttack;
		_playerCameraStateMachineController.OnFirstPersonCameraState += ChangeCrossbow1stPerson;
		_playerCameraStateMachineController.OnThirdPersonCameraState += ChangeCrossbow3rdPerson;
	}

	private void ChangeCrossbow1stPerson()
	{
		//_currentProjectile = _projectile1stPerson;
		_lineRenderer1stPerson.enabled = true;
		_lineRenderer3rdPerson.enabled = false;
	}

	private void ChangeCrossbow3rdPerson()
	{
		//_currentProjectile = _projectile3rdPerson;
		_lineRenderer1stPerson.enabled = false;
		_lineRenderer3rdPerson.enabled = true;
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
		//_lineRenderer1stPerson.enabled = true;
		//_lineRenderer3rdPerson.enabled = true;

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
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			ChangeCrossbow1stPerson();
		}
		else
		{
			ChangeCrossbow3rdPerson();
		}

		_projectile1stPerson.transform.SetParent(null);
		_projectile1stPerson.layer = LayerMask.NameToLayer("Default");

		_projectile3rdPerson.transform.SetParent(null);

		_projectileFlyingDirection = _playerCamera.transform.rotation;
		_projectile1stPerson.transform.rotation = _projectileFlyingDirection;
		_projectile3rdPerson.transform.rotation = _projectileFlyingDirection;

		while (Vector3.Distance(_projectile1stPerson.transform.position, point) > 0.1f)
		{
			Vector3 newPosFP = Vector3.MoveTowards(_projectile1stPerson.transform.position, point, _projectileSpeed * Time.deltaTime);
			_projectile1stPerson.transform.position = newPosFP;
			_lineRenderer1stPerson.SetPosition(0, _projectileStringStartPoint1stPerson.transform.position);
			_lineRenderer1stPerson.SetPosition(1, _projectileStringEndPoint1stPerson.transform.position);
			
			Vector3 newPosTP = Vector3.MoveTowards(_projectile3rdPerson.transform.position, point, _projectileSpeed * Time.deltaTime);
			_projectile3rdPerson.transform.position = newPosTP;
			_lineRenderer3rdPerson.SetPosition(0, _projectileStringStartPoint3rdPerson.transform.position);
			_lineRenderer3rdPerson.SetPosition(1, _projectileStringEndPoint3rdPerson.transform.position);
			
			yield return null;
		}

	}

	private IEnumerator PlungePlayer(Vector3 hookPoint)
	{
		_playerCollider.SetActive(false);
		_playerRigidbody.useGravity = false;

		while (_isCrossbowAttacking)
		{
			Vector3 directionToHook = (hookPoint - _player.transform.position).normalized;
			_playerRigidbody.linearVelocity = directionToHook * _projectileSpeed;

			float distanceToHook = Vector3.Distance(_player.transform.position, hookPoint);

			if (distanceToHook < 0.2f)
			{
				_player.transform.position = hookPoint;
				_playerRigidbody.linearVelocity = Vector3.zero;

				yield return StartCoroutine(ReturnProjectile());
			}

			_lineRenderer1stPerson.SetPosition(0, _projectileStringStartPoint1stPerson.transform.position);
			_lineRenderer1stPerson.SetPosition(1, _projectileStringEndPoint1stPerson.transform.position);

			_lineRenderer3rdPerson.SetPosition(0, _projectileStringStartPoint3rdPerson.transform.position);
			_lineRenderer3rdPerson.SetPosition(1, _projectileStringEndPoint3rdPerson.transform.position);

			yield return null;
		}
	}

	private IEnumerator HookObject(RaycastHit hit)
	{
		_projectile1stPerson.transform.SetParent(hit.collider.transform);
		_projectile3rdPerson.transform.SetParent(hit.collider.transform);
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

				yield return StartCoroutine(ReturnProjectile());
			}

			_lineRenderer1stPerson.SetPosition(0, _projectileStringStartPoint1stPerson.transform.position);
			_lineRenderer1stPerson.SetPosition(1, _projectileStringEndPoint1stPerson.transform.position);

			_lineRenderer3rdPerson.SetPosition(0, _projectileStringStartPoint3rdPerson.transform.position);
			_lineRenderer3rdPerson.SetPosition(1, _projectileStringEndPoint3rdPerson.transform.position);

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

		_hookedObjectCollider.enabled = false;

		_hookedObjectRigidbody.useGravity = false;
		_hookedObjectRigidbody.linearDamping = 0;
	}

	private void StopHookingObject()
	{
		if (_isCrossbowAttacking)
		{
			_hookedObjectRigidbody.linearVelocity = Vector3.zero;

			_hookedObjectRigidbody.useGravity = true;
			_hookedObjectCollider.enabled = true;
			_hookedObjectCollider = null;
			_hookedObjectNavMeshAgent = null;
			_NPCabstract = null;
			_hookedObject = null;
			_hookedObjectRigidbody = null;
			Debug.Log("Притяжение NPC завершено.");
		}
	}

	private void OnDestroy()
	{
		// Проверка на null защищает от ошибки, если эти объекты были уничтожены
		//Debug.Log(_gameSceneManager);
		if (_hookedObject != null)
		{
			StopHookingObject();
		}

		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingMainMenuScene -= FullStopPlunging;
			_gameSceneManager.OnBeginLoadingGameplayScene -= FullStopPlunging;
		}

		if (_playerBehaviour != null)
		{
			_playerBehaviour.OnPlayerDisarmed -= StopCrossbowAttack;
		}

		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnFirstPersonCameraState -= ChangeCrossbow1stPerson;
			_playerCameraStateMachineController.OnThirdPersonCameraState -= ChangeCrossbow3rdPerson;
		}

		OnCrossbowDestroyed();
	}

	private IEnumerator ReturnProjectile()
	{
		// Отсоединяем перед движением
		_projectile1stPerson.transform.SetParent(null);
		_projectile3rdPerson.transform.SetParent(null);

		SceneManager.MoveGameObjectToScene(_projectile1stPerson, SceneManager.GetSceneByBuildIndex(0));
		SceneManager.MoveGameObjectToScene(_projectile3rdPerson, SceneManager.GetSceneByBuildIndex(0));

		while (Vector3.Distance(_projectile1stPerson.transform.position, _projectile1stPersonRestPosition) > 0.001f)
		{
			Vector3 targetPos1st = Vector3.MoveTowards(_projectile1stPerson.transform.position, _projectile1stPersonRestPosition, _projectileSpeed * Time.deltaTime);
			_projectile1stPerson.transform.position = targetPos1st;

			Vector3 targetPos3rd = Vector3.MoveTowards(_projectile3rdPerson.transform.position, _projectile3rdPersonRestPosition, _projectileSpeed * Time.deltaTime);
			_projectile3rdPerson.transform.position = targetPos3rd;
		}
	
		StopCrossbowAttack();

		yield return null;
	}

	private void OnCrossbowDestroyed()
	{
		if (_isCrossbowAttacking)
		{
			Destroy(_projectile1stPerson);
			Destroy(_projectile3rdPerson);
			_playerRigidbody.useGravity = true;
			_isCrossbowAttacking = false;
			Debug.Log("Притяжение завершено.");
			_gameController.PlayerStoppedPlunging();
			_playerCollider.SetActive(true);
		}
	}

	private void StopCrossbowAttack()
	{
		if (_isCrossbowAttacking)
		{
			Debug.Log("STOP ATTACKING");
			// Возвращаем физические объекты к их родителям
			_projectile1stPerson.transform.SetParent(_projectileParent1stPerson);
			_projectile1stPerson.layer = LayerMask.NameToLayer("FirstPerson");
			_projectile1stPerson.transform.localPosition = _projectile1stPersonRestPosition;
			_projectile1stPerson.transform.localRotation = _projectile1stPersonRestDirection;

			_projectile3rdPerson.transform.SetParent(_projectileParent3rdPerson);
			_projectile3rdPerson.transform.localPosition = _projectile3rdPersonRestPosition;
			_projectile3rdPerson.transform.localRotation = _projectile3rdPersonRestDirection;

			_lineRenderer1stPerson.SetPosition(0, _projectileStringStartPoint1stPerson.transform.position);
			_lineRenderer1stPerson.SetPosition(1, _projectileStringEndPoint1stPerson.transform.position);

			_lineRenderer3rdPerson.SetPosition(0, _projectileStringStartPoint3rdPerson.transform.position);
			_lineRenderer3rdPerson.SetPosition(1, _projectileStringEndPoint3rdPerson.transform.position);

			_playerRigidbody.useGravity = true;
			_isCrossbowAttacking = false;
			Debug.Log("Притяжение завершено.");
			_gameController.PlayerStoppedPlunging();
			_playerCollider.SetActive(true);

			// Отключаем ОБЕ линии рендера
			_lineRenderer1stPerson.enabled = false;
			_lineRenderer3rdPerson.enabled = false;

			if (WeaponHandType == WeaponHandsEnum.HandRight)
			{
				_playerWeaponFirstPersonRenderer.UpdateWeaponRightVisibility();
			}
			else
			{
				_playerWeaponFirstPersonRenderer.UpdateWeaponLeftVisibility();
			}

			if (_hookedObject != null)
			{
				StopHookingObject();
			}
		}
	}

	private void FullStopPlunging()
	{
		_playerRigidbody.linearVelocity = Vector3.zero;

		StopCrossbowAttack();
	}

	public override void StopAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override void StartAutoShootingWeaponPlayer()
	{
		//throw new System.NotImplementedException();
	}

	public override IEnumerator AutoShootWeaponPlayerCourutine()
	{
		//throw new System.NotImplementedException();
		yield return null;
	}
}