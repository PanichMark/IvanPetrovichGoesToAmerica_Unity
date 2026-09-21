using UnityEngine;
using System.Collections;

public class LoadMainMenuScene : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	public string InteractionObjectNameUI => null;

	public string InteractionHintMessageMain => null;

	public string InteractionHintMessageAction => null;

	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	public event IInteractable.InteractableObjectHandler OnInteract;

	private GameScenesManager _gameScenesManager;
	void Start()
	{
		_gameScenesManager = ServiceLocator.Resolve<GameScenesManager>();
	}

	public void Interact()
	{
		StartCoroutine(StartLoadingMainMenuScene());
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private IEnumerator StartLoadingMainMenuScene()
	{
		gameObject.transform.SetParent(null);

		DontDestroyOnLoad(gameObject);

		//yield return StartCoroutine(_saveLoadController.NewGame());
		//_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		yield return StartCoroutine(_gameScenesManager.LoadMainMenuScene());

		Destroy(gameObject);
	}

}
