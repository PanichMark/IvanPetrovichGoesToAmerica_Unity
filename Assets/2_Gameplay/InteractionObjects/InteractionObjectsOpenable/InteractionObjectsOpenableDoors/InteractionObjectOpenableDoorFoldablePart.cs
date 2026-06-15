using UnityEngine;

public class InteractionObjectOpenableDoorFoldablePart : MonoBehaviour, IInteractable
{
	// Этот флаг определяет способ поиска контроллера.
	// Ставим TRUE на ParentDoor и FALSE на ChildDoor.
	[SerializeField] private bool _isParent;

	private InteractionObjectOpenableAbstract _controller;

	void Start()
	{
		if (_isParent)
		{
			// Для ParentDoor ищем контроллер у его родителя (Door_Root)
			_controller = transform.parent.GetComponent<InteractionObjectOpenableAbstract>();
		}
		else
		{
			// Для ChildDoor ищем контроллер у родителя своего родителя (тоже Door_Root)
			_controller = transform.parent.parent.GetComponent<InteractionObjectOpenableAbstract>();
		}

		if (_controller == null)
		{
			Debug.LogError("Контроллер не найден! Проверьте иерархию объекта " + gameObject.name, this);
		}
	}

	// --- Реализация интерфейса IInteractable ---
	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => null;
	public string InteractionHintMessageAction => null;
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	public event IInteractable.InteractableObjectHandler OnInteract;

	// При взаимодействии вызываем метод у найденного контроллера
	public void Interact()
	{
		_controller?.Interact(); // Безопасный вызов через null-условный оператор
	}

	public void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}
}