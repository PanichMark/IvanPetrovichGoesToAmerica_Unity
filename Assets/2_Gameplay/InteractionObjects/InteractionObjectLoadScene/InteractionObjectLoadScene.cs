using UnityEngine;
using System.Collections;

public class InteractionObjectLoadScene : MonoBehaviour, IInteractable
{
	private GameScenesManager _gameSceneManager;
	[SerializeField] private GameScenesEnum _targetScene;
	[SerializeField] private InteractionObjectOpenableDoorScenePlayerTransform _interactionObjectOpenableDoorScenePlayerTransform;
	private PlayerMovementController _playerMovementController;
	private LocalizationManager _localizationManager;

	// Реализация интерфейса IInteractable
	public string InteractionObjectNameSystem => $"LoadScene_{_targetScene}";
	public string InteractionObjectNameUI => _localizationManager?.GetLocalizedString(_targetScene.ToString()) ?? "Новая локация";
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; private set; }

	// Событие взаимодействия
	public event IInteractable.InteractableObjectHandler OnInteract;

	private void Start()
	{
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>("GameSceneManager");
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GoToScene");

		_localizationManager.OnLanguageChanged += ChangeLanguage;


	}

	private void OnDestroy()
	{
		if (_localizationManager != null)
		{
			_localizationManager.OnLanguageChanged -= ChangeLanguage;
		}
	}

	public void Interact()
	{
		// Вызываем событие для UI (например, чтобы скрыть подсказку)
		OnInteract?.Invoke();

		StartCoroutine(LoadGameplayScene());
	}

	public void InteractCutscene()
	{
		// Для загрузки сцены катсцена обычно не нужна, делаем то же самое, что и обычный интеракт
		Interact();
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GoToScene");
	}

	private IEnumerator LoadGameplayScene()
	{
		Debug.Log("LOADING: " + _targetScene);

		// Отвязываем объект от родителя двери или стены, чтобы он не уничтожился при смене сцены
		gameObject.transform.SetParent(null);
		DontDestroyOnLoad(gameObject);

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(_targetScene));

		// Устанавливаем позицию игрока уже внутри новой сцены
		if (_interactionObjectOpenableDoorScenePlayerTransform != null && _playerMovementController != null)
		{
			_playerMovementController.SetPlayerPosition(_interactionObjectOpenableDoorScenePlayerTransform.PlayerPosition);
			_playerMovementController.SetPlayerRotationY(_interactionObjectOpenableDoorScenePlayerTransform.PlayerRotation);
		}

		Destroy(gameObject);
	}
}