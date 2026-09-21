using UnityEngine;
using System.Collections;

public class EndGameTitlesCountdown : MonoBehaviour
{
	[SerializeField] private IInteractable IInteractable;
	[SerializeField] private GameObject LoadMainMenuGameObject;

	void Start()
	{
		IInteractable = LoadMainMenuGameObject.GetComponent<IInteractable>();
		StartCoroutine(Countdown());
	}

	private IEnumerator Countdown()
	{
		for (int i = 1; i <= 5; i++)
		{
			Debug.Log(i.ToString());

			// Ждем ровно одну секунду перед следующей итерацией
			yield return new WaitForSeconds(1f);
		}
	
		IInteractable.Interact();
	}
}