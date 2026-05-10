using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoorScene : InteractionObjectOpenableDoor
{
	private GameSceneManager _gameSceneManager;
	[SerializeField] private GameScenesEnum _targetScene;
	[SerializeField] private Vector3 _newPlayerPosition;
	[SerializeField] private int _newPlayerRotation;
	private PlayerMovementController _playerMovementController;

	private void Start()
	{
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
	}

	protected override void PerformDoorInteraction()
	{
		StartCoroutine(LoadGameplayScene());
	}

	private IEnumerator LoadGameplayScene()
	{
		Debug.Log("LOADING: " + _targetScene);

		Transform parentTransform = transform.parent;
		if (parentTransform == null)
			yield break;

		DontDestroyOnLoad(parentTransform.gameObject);

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(_targetScene));

		_playerMovementController.SetPlayerPosition(_newPlayerPosition);
		_playerMovementController.SetPlayerRotation(_newPlayerRotation);

		Destroy(parentTransform.gameObject);
	}
}