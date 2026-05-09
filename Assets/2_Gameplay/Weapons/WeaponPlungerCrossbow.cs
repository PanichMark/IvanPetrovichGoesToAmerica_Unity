using UnityEngine;
using UnityEngine.AI;

public class WeaponPlungerCrossbow : WeaponAbstract
{
	private GameController gameController;
	private GameSceneManager gameSceneManager;
	private PlayerBehaviour playerBehaviour;

	private GameObject player;
	private GameObject playerCollider;
	private GameObject playerCamera;
	private Rigidbody playerRigidbody;
	private bool IsPlayerPlungering = false;

	private GameObject hookedObject = null;
	private Rigidbody hookedObjectRigidbody = null;
	private bool isObjectBeingHooked = false;
	private Collider hookedObjectCollider = null;

	private NPCStateMachineController npcStateMachineController;
	private NPCAbstract NPCabstract = null;
	private NavMeshAgent hookedObjectNavMeshAgent = null;

	private Vector3 hookPoint;
	private float maxHookDistance = 17f;
	private float pullSpeed = 12f;

	public override string WeaponNameSystem => "PlungerCrossbow";

	public override string WeaponNameUI => "Абордажный Арбалет";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/CrossBow icon");
	public override float WeaponDamage => 0; 

	private void Start()
	{
		playerCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		player = ServiceLocator.Resolve<GameObject>("Player");
		playerRigidbody = player.GetComponent<Rigidbody>();
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		playerCollider = ServiceLocator.Resolve<GameObject>("playerColliderGameObject");
		
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		playerBehaviour = ServiceLocator.Resolve<PlayerBehaviour>("PlayerBehaviour");

		gameSceneManager.OnBeginLoadMainMenuScene += StopPlungingCompletely;
		gameSceneManager.OnBeginLoadGameplayScene += StopPlungingCompletely;
		playerBehaviour.OnPlayerDisarmed += StopPlunging;
	}

	public override void WeaponAttack()
	{
		if (IsPlayerPlungering || isObjectBeingHooked) return;

		if (playerCamera == null || player == null || playerRigidbody == null) return;

		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, maxHookDistance))
		{
			hookPoint = hit.point;

			if ((hit.collider.gameObject.TryGetComponent<NPCAbstract>(out _) ||
				hit.collider.gameObject.TryGetComponent<InteractionObjectPickableAbstract>(out _)))
			{
				hookedObject = hit.collider.gameObject;
				hookedObjectRigidbody = hookedObject.GetComponent<Rigidbody>();
				hookedObjectCollider = hookedObject.GetComponent<Collider>();

				if (hookedObjectRigidbody == null)
				{
					hookedObjectRigidbody = hookedObject.AddComponent<Rigidbody>();
					Debug.LogWarning($"Rigidbody добавлен к NPC: {hookedObject.name}");
			
				}
				hookedObjectNavMeshAgent = hookedObject.GetComponent<NavMeshAgent>();
				if (hookedObjectNavMeshAgent != null)
				{

					hookedObjectNavMeshAgent.enabled = false;
				}
				NPCabstract = hookedObject?.GetComponent<NPCAbstract>();
				npcStateMachineController = hookedObject?.GetComponent<NPCStateMachineController>();
				if (npcStateMachineController != null && NPCabstract != null)
				{
					//////
					//ПОТОМ ПОМЕНЯТЬ НА BEING HOOKED !!!
					//////
					npcStateMachineController.SetNPCState(NPCStateTypes.Dead);
				}

				hookedObjectRigidbody.useGravity = false;
				hookedObjectRigidbody.linearDamping = 0;


				isObjectBeingHooked = true;
				IsPlayerPlungering = false;
				Debug.Log("Крюк зацепил NPC!");
			}
			else 
			{
				playerCollider.SetActive(false);
				IsPlayerPlungering = true; 
				playerRigidbody.useGravity = false;
				isObjectBeingHooked = false;
				Debug.Log($"Крюк зацепился за: {hit.collider.name}");
			}
		}
	}

	private void FixedUpdate()
	{
		if (isObjectBeingHooked)
		{
			Vector3 finalPosition = player.transform.position;
			finalPosition.y += 0.5f;
			finalPosition += player.transform.forward * 1.5f;

			Vector3 directionToTarget = (finalPosition - hookedObject.transform.position).normalized;

			hookedObjectRigidbody.linearVelocity = directionToTarget * pullSpeed;

			float distanceToTarget = Vector3.Distance(hookedObject.transform.position, finalPosition);

			if (distanceToTarget < 0.3f)
			{
				hookedObjectRigidbody.linearVelocity = Vector3.zero;
				hookedObjectRigidbody.position = finalPosition;

				StopHookingObject();
			}
		}

		if (IsPlayerPlungering)
		{
			gameController.PlayerStartedPlunging();
			Vector3 directionToHook = (hookPoint - player.transform.position).normalized;

			playerRigidbody.linearVelocity = directionToHook * pullSpeed;

			float distanceToHook = Vector3.Distance(player.transform.position, hookPoint);

			if (distanceToHook < 0.5f || playerRigidbody.linearVelocity.magnitude < 0.1f)
			{
				player.transform.position = hookPoint;
				playerRigidbody.linearVelocity = Vector3.zero;

				StopPlunging();
			}
		}
	}
	private void StopHookingObject()
	{
		if (isObjectBeingHooked)
		{
			if (hookedObjectNavMeshAgent != null)
			{
			//hookedObjectNavMeshAgent.enabled = true;
			}

			hookedObjectRigidbody.linearVelocity = Vector3.zero;

			hookedObjectRigidbody.useGravity = true;
			hookedObjectCollider = null;
			hookedObjectNavMeshAgent = null;
			NPCabstract = null;
			hookedObject = null;
			hookedObjectRigidbody = null;
			isObjectBeingHooked = false;
			Debug.Log("Притяжение NPC завершено.");
		}
	}

	private void OnDestroy()
	{
		gameSceneManager.OnBeginLoadMainMenuScene -= StopPlungingCompletely;
		gameSceneManager.OnBeginLoadGameplayScene -= StopPlungingCompletely;
		playerBehaviour.OnPlayerDisarmed -= StopPlunging;

		StopPlunging();
	}

	private void StopPlunging()
	{
		if (IsPlayerPlungering)
		{
			playerRigidbody.useGravity = true;

			IsPlayerPlungering = false;
			Debug.Log("Притяжение завершено.");
			gameController.PlayerStoppedPlunging();
			playerCollider.SetActive(true);
		}
	}

	private void StopPlungingCompletely()
	{
		playerRigidbody.linearVelocity = Vector3.zero;
		StopPlunging();
	}
}