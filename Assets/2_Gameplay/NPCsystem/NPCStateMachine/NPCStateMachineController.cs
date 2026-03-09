using UnityEngine;
using System.Collections;

public class NPCStateMachineController : MonoBehaviour
{
	[SerializeField] private NPCStateTypes initialState = NPCStateTypes.Default;

	private AbstractNPCState NPCstate;
	private NPCStateTypes NPCStateType;
	private NPCAbstract NPCabstract;
	public string CurrentNPCState { get; private set; } = "PlayerIdle";
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	void Start()
    {
	
		
		
		NPCabstract = GetComponent<NPCAbstract>();

		//SetPlayerMovementState(NPCStateTypes.Default);

		SetNPCState(initialState); // Применяем выбранное состояние
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
	public void SetNPCState(NPCStateTypes playerMovementStateType)
	{
		
			AbstractNPCState newState;

			if (playerMovementStateType == NPCStateTypes.Default)
			{
				newState = new DefaultNPCState(this);
				CurrentNPCState = "Default";
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
			}
			NPCstate = newState;

			//Debug.Log("MovementState: " + CurrentNPCState);
		}
	}

