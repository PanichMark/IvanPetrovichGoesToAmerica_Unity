using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	[SerializeField] private string _interactionObjectNameSystem;
	[SerializeField] private bool _isNextChannel;

	// Ссылка на контроллер, который управляет всем ТВ
	private InteractionObjectTVController _tvController;

	private Collider _collider;

	public event IInteractable.InteractableObjectHandler OnInteract;

	public string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => null;
	public string InteractionHintMessageAction => null;
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	void Start()
	{
		_collider = GetComponent<Collider>();

		// Находим контроллер на родительском объекте
		_tvController = transform.parent.GetComponent<InteractionObjectTVController>();

		// Изначально кнопка неактивна, так как ТВ выключен
		DisableButtonChannel();

		// --- ИСПРАВЛЕННАЯ ЛОГИКА ---
		// Подписываемся на событие изменения состояния ТВ из КОНТРОЛЛЕРА.
		// Это делает кнопку канала независимой от кнопки питания.
		_tvController.OnTVStateChanged += isOn =>
		{
			if (isOn)
			{
				EnableButtonChannel();
			}
			else
			{
				DisableButtonChannel();
			}
		};
	}

	public void Interact()
	{
		// При нажатии просим контроллер переключить канал
		_tvController.SwitchChannel(_isNextChannel);
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private void EnableButtonChannel()
	{
		_collider.enabled = true;
	}

	private void DisableButtonChannel()
	{
		_collider.enabled = false;
	}
}