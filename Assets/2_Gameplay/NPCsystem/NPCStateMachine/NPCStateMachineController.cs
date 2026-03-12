using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachineController : MonoBehaviour
{
	[SerializeField] private NPCStateTypes initialState = NPCStateTypes.StationaryAction;
	private float animationDuration;
	

	// Публичные свойства для доступа
	public List<AnchorPointStop> StopConfigs => stopConfigs;

	[SerializeField]
	private List<GameObject> anchorPoints = new List<GameObject>(); // Список анкорных точек

	public List<GameObject> AnchorPoints => anchorPoints;
	public float AnimationDuration => animationDuration;        // Доступны для подклассов

	[SerializeField] private List<AnchorPointStop> stopConfigs = new List<AnchorPointStop>(); // Специальный список точек остановки

	private AbstractNPCState NPCstate;
	private NPCStateTypes NPCStateType;
	private NPCAbstract NPCabstract;
	private NavMeshAgent navMeshAgent;
	public string CurrentNPCState { get; private set; } = "StationaryAction";
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public bool IsAtPosition(Vector3 position, float tolerance = 2f)
	{
		return Vector3.Distance(transform.position, position) <= tolerance;
	}

	void Start()
    {
		navMeshAgent = GetComponent<NavMeshAgent>();
		
		
		NPCabstract = GetComponent<NPCAbstract>();

		//SetPlayerMovementState(NPCStateTypes.Default);

		SetNPCState(initialState); // Применяем выбранное состояние
	}
	public IEnumerator MoveBetweenAnchorPointsCourutine()
	{
		int nextIndex = 0;

		while (true) // Меняем условие на true, чтобы постоянно повторять цикл
		{
			if (anchorPoints.Count > 0)
			{
				GameObject targetPoint = anchorPoints[nextIndex];
				navMeshAgent.destination = targetPoint.transform.position;

				while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
				{
					yield return null; // Продолжаем ждать, пока путь вычисляется или пока цель не достигнута
				}
			}

			nextIndex++; // Переносимся к следующей точке
			if (nextIndex >= anchorPoints.Count)
			{
				nextIndex = 0; // Начинаем заново с первой точки
			}
		}
	}


	private Coroutine currentMovementCoroutine;
	private void Update()
	{
		NPCstate.Update();
	}
	public IEnumerator RandomMoveCourutine()
	{
		while (true)
		{
			// Сначала определяем случайное направление
			float randomX = Random.Range(-1f, 1f);
			float randomZ = Random.Range(-1f, 1f);
			Vector3 direction = new Vector3(randomX, 0f, randomZ).normalized;

			// Сохраняем это направление на фиксированное время
			float duration = Random.Range(1f, 3f); // Время сохранения текущего направления
			float elapsedTime = 0f;

			while (elapsedTime <= duration)
			{
				// Зафиксировали направление и двигаемся в нём в течение заданного времени
				float speed = 2f; // Скорость передвижения
				float distance = speed * Time.deltaTime;
				transform.Translate(direction * distance, Space.World);

				elapsedTime += Time.deltaTime;
				yield return null; // Позволяет игре обработать кадры
			}

			// Пауза перед выбором следующего направления
			yield return new WaitForSeconds(Random.Range(1f, 3f));
		}
	}

	public void StartRandomMove()
	{
		currentMovementCoroutine = StartCoroutine(RandomMoveCourutine());
	}
	public void StopRandomMove()
	{
		if (currentMovementCoroutine != null)
		{
			StopCoroutine(currentMovementCoroutine);
			currentMovementCoroutine = null;
		}
	}

	public void StartAnchorMove()
	{
		TurnNavmeshOn();
		currentMovementCoroutine = StartCoroutine(MoveBetweenAnchorPointsCourutine());
	}
	public void StopAnchorMove()
	{
		if (currentMovementCoroutine != null)
		{
			TurnNavmeshOff();
			StopCoroutine(currentMovementCoroutine);
			currentMovementCoroutine = null;

		}
		
	}

	public void TurnNavmeshOn()
	{
		navMeshAgent.enabled = true; // Отключает работу агента
	}
	public void TurnNavmeshOff()
	{
		navMeshAgent.enabled = false; // Отключает работу агента
	}

	public void SetNPCState(NPCStateTypes stateType,  float animDuration)
	{
		this.animationDuration = animDuration;

		SetNPCState(stateType);
	}

	public void SetNPCState(NPCStateTypes playerMovementStateType)
	{
	
		AbstractNPCState newState;

		if (playerMovementStateType == NPCStateTypes.StationaryAction)
		{
		
			newState = new StationaryActionNPCState(this, animationDuration);
	
			CurrentNPCState = "StationaryAction";
			NPCabstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Patrolling)
		{
			newState = new PatrollingNPCState(this);
			CurrentNPCState = "Patrolling";
			NPCabstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Interested)
		{
			newState = new InterestedNPCState();
			//CurrentNPCState = "PlayerWalking";
		}
		else if (playerMovementStateType == NPCStateTypes.Alarmed)
		{
			newState = new AlarmedNPCState();
			//CurrentNPCState = "PlayerRunning";
		}
		else if (playerMovementStateType == NPCStateTypes.Chasing)
		{

			newState = new ChasingNPCState();
			//	CurrentNPCState = "PlayerJumping";
		}
		else if (playerMovementStateType == NPCStateTypes.Attacking)
		{

			newState = new AttackingNPCState();
			//CurrentNPCState = "PlayerFalling";
		}
		else if (playerMovementStateType == NPCStateTypes.Searching)
		{
			newState = new SearchingNPCState();
			//CurrentNPCState = "PlayerCrouchingIdle";
		}
		else if (playerMovementStateType == NPCStateTypes.Scared)
		{
			newState = new ScaredNPCState();
			CurrentNPCState = "Scared";
			NPCabstract.gameObject.tag = "Untagged";
		}
		else if (playerMovementStateType == NPCStateTypes.Fleeing)
		{
			newState = new FleeingNPCState();
			//CurrentNPCState = "PlayerCrouchingWalking";
		}
		else if (playerMovementStateType == NPCStateTypes.Choked)
		{
			newState = new ChokedNPCState();
			//CurrentNPCState = "PlayerCrouchingWalking";
		}
		else if (playerMovementStateType == NPCStateTypes.Dead)
		{
			if (!NPCabstract.IsNPCdead)
			{
				NPCabstract.SetHealthToZero();
			}
			//NPCabstract.SetHealthToZero();
			//NPCabstract.ConvertToPickableObject();
			newState = new DeadNPCState(this);
			//CurrentNPCState = "PlayerCrouchingWalking";
		}
		else
		{
			newState = null;
			Debug.Log("ITS NULL!!!");
		}
			NPCstate = newState;

			//Debug.Log("MovementState: " + CurrentNPCState);
		}
	}

