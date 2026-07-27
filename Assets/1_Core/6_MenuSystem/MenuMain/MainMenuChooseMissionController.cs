using UnityEngine;
using UnityEngine.UI;

public class MainMenuChooseMissionController : MonoBehaviour
{
	private Button _buttonCloseMainMenuChooseMission;
	private GameObject _canvasMainMenuChooseMission;
	public delegate void MainMenuChooseMissionHandler();
	public event MainMenuChooseMissionHandler OnCloseMainMenuChooseMission;

	public bool IsMainMenuChooseMissionOpened { get; private set; }

	public void Initialize(
	GameObject canvasMainMenuChooseMission,
	ViewModelMainMenuChooseMission viewModelMainMenuChooseMission)
	{
		_canvasMainMenuChooseMission = canvasMainMenuChooseMission;
	}

	public void ShowCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = true;
		_canvasMainMenuChooseMission.SetActive(true);
	}

	public void HideCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = false;
		OnCloseMainMenuChooseMission?.Invoke();
		_canvasMainMenuChooseMission.SetActive(false);
	}
}
