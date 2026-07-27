using UnityEngine;
using UnityEngine.UI;

public class MainMenuChooseMissionController : MonoBehaviour
{
	private Button _buttonCloseMainMenuChooseMission;
	private GameObject _canvasMainMenuChooseMission;
	public delegate void MainMenuChooseMissionHandler();
	public event MainMenuChooseMissionHandler OnCloseMainMenuChooseMission;

	public bool IsMainMenuChooseMissionOpened { get; private set; }

	public void Initialize()
	{
		_canvasMainMenuChooseMission = ServiceLocator.Resolve<GameObject>("CanvasMainMenuChooseMission");

		Debug.Log("MainMenuChooseMissionController Initialized");
	}

	public void ShowCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = true;
		_canvasMainMenuChooseMission.SetActive(true);

		Debug.Log("Show ChooseMission");
	}

	public void HideCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = false;
		OnCloseMainMenuChooseMission?.Invoke();
		_canvasMainMenuChooseMission.SetActive(false);

		Debug.Log("Hide ChooseMission");
	}
}
