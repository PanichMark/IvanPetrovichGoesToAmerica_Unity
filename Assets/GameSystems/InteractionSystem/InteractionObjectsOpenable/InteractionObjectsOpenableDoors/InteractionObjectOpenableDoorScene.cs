using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoorScene : InteractionObjectOpenableDoor
{
	private GameSceneManager gameSceneManager;
	[SerializeField] private GameScenesEnum targetScene;
	private void Start()
	{
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
	}

	protected override void PerformDoorInteraction()
	{
		StartCoroutine(LoadGameplayScene());
	}

	IEnumerator LoadGameplayScene()
	{
		Debug.Log("LOADING: " + targetScene);

		// Получаем родителя текущего объекта
		Transform parentTransform = transform.parent;

		// Если родитель найден, сохраняем его
		
		// Сохраняем родительский объект
		DontDestroyOnLoad(parentTransform.gameObject);
		

		// Асинхронно загружаем новую сцену
		yield return StartCoroutine(gameSceneManager.LoadScene(targetScene));

	
		Destroy(parentTransform.gameObject);
		
	}
}