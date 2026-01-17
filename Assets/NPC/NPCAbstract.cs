using UnityEngine;
using System.Collections;

public abstract class NPCAbstract : MonoBehaviour, IInteractable
{
	[SerializeField][Min(0)] private float NPC_maxhealth;
	private float NPC_currenthealth;
	protected bool IsNPCdead => NPC_currenthealth <= 0;
	[SerializeField] protected string NPC_name;

	[SerializeField] private bool KillNPC;
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();
	public string InteractionObjectNameUI => NPC_name;
	public string MainInteractionHint => $"Поговорить с {NPC_name}";
	public string AdditionalInteractionHint => throw new System.NotImplementedException();
	public virtual bool IsAdditionalInteractionHintActive => false;

	private void Start()
	{
		NPC_currenthealth = NPC_maxhealth;
		Debug.Log(NPC_currenthealth);

		if (KillNPC)
		{
			// Запускаем корутин, который через 1 секунду нанесет NPC максимальный урон
			StartCoroutine(KillAfterDelay());
		}
	}

	public abstract void Interact();

	// Метод превращения в пассивный объект
	private void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";      // Ставим тег Interactable
		enabled = false;                      // Отключаем уникальный скрипт NPC
		gameObject.AddComponent<Rigidbody>(); // Добавляем физический компонент

		// Используем фабричный метод для задания имени UI
		PickableObjectNonThrowable.CreateWithName(gameObject, NPC_name);

		Destroy(this);                        // Уничтожаем оригинальный компонент NPC
	}

	// Метод, вызываемый при получении повреждений
	public void TakeDamage(float amount)
	{
		NPC_currenthealth -= amount;
		if (IsNPCdead)
		{
			Debug.Log($"{NPC_name} is now a passive pickable object");
			ConvertToPickableObject(); // Превращаемся в pickable объект
		}
	}

	// Корутин, вызывающий смерть NPC через 1 секунду
	IEnumerator KillAfterDelay()
	{
		yield return new WaitForSeconds(0.01f); // Ждем 1 секунду
		TakeDamage(9999); // Наносим большой урон, вызывая смерть NPC
	}
}