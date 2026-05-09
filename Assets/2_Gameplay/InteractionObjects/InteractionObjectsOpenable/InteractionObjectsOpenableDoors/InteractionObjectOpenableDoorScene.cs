using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoorScene : InteractionObjectOpenableDoor
{
	private GameSceneManager gameSceneManager;
	[SerializeField] private GameScenesEnum targetScene;
	[SerializeField] private Vector3 newPlayerPosition;
	[SerializeField] private int newPlayerRotation;
	private PlayerMovementController playerMovementController;

	private void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
	}

	protected override void PerformDoorInteraction()
	{
		StartCoroutine(LoadGameplayScene());
	}

	private IEnumerator LoadGameplayScene()
	{
		Debug.Log("LOADING: " + targetScene);

		Transform parentTransform = transform.parent;
		if (parentTransform == null)
			yield break;

		DontDestroyOnLoad(parentTransform.gameObject);

		yield return StartCoroutine(gameSceneManager.LoadGameplayScene(targetScene));

		playerMovementController.SetPlayerPosition(newPlayerPosition);
		playerMovementController.SetPlayerRotation(newPlayerRotation);

		Destroy(parentTransform.gameObject);
	}
}