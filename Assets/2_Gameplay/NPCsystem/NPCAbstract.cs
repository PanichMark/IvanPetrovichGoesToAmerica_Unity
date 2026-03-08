using UnityEngine;
using System.Collections;

public abstract class NPCAbstract : MonoBehaviour, IInteractable
{
	[SerializeField][Min(0)] private float NPCMaxHealth;
	[SerializeField][Min(0)] private float NPCCurrentHealth;
	protected bool IsNPCdead => NPCCurrentHealth <= 0;
	[SerializeField] protected string NPC_name;


	protected NPCStateMachineController _npcStateMachineController;

	public string InteractionObjectNameSystem => throw new System.NotImplementedException();
	public string InteractionObjectNameUI => NPC_name;
	public string InteractionHintMessageMain => $"Поговорить с {NPC_name}";
	public string InteractionHintMessageAdditional => throw new System.NotImplementedException();
	public virtual bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction { get; protected set; }
	private void Start()
	{
		
		//Debug.Log(NPC_currenthealth);
		_npcStateMachineController = GetComponent<NPCStateMachineController>();

		if (IsNPCdead)
		{
			_npcStateMachineController.SetNPCState(NPCStateTypes.Dead);
			ConvertToPickableObject();
		}
	}

	public abstract void Interact();

	// Метод превращения в пассивный объект
	public void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";      // Ставим тег Interactable
		enabled = false;                      // Отключаем уникальный скрипт NPC
		gameObject.AddComponent<Rigidbody>(); // Добавляем физический компонент

		// Используем фабричный метод для задания имени UI
		InteractionObjectPickable.CreateWithName(gameObject, NPC_name);

		Destroy(this);                        // Уничтожаем оригинальный компонент NPC
	}

	// Метод, вызываемый при получении повреждений
	public void TakeDamage(float amount)
	{
		NPCCurrentHealth -= amount;
		if (IsNPCdead)
		{
			Debug.Log($"{NPC_name} is now a passive pickable object");
		}
	}

	public void SetHealthToZero()
	{
		NPCCurrentHealth = 0;
	}


}