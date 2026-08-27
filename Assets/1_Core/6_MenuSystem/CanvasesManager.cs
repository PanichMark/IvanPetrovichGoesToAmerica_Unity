using UnityEngine;

public class CanvasesManager : MonoBehaviour
{
	private GameObject _canvasMainMenuReadNews;
	private GameObject _canvasMainMenuChooseMission;
	private GameObject _canvasLockpickElectronicMenu;
	private GameCanvasesList _canvasesList;
	private GameObject _canvasNoteMenu;
	private GameObject _canvasLockpickMechanicalMenu;

	public void Initialize(
		 GameCanvasesList canvasesList)
	{
		_canvasesList = canvasesList;
		_canvasMainMenuChooseMission = _canvasesList.CanvasMainMenuChooseMission;
		_canvasMainMenuReadNews = _canvasesList.CanvasMainMenuReadNews;
		_canvasLockpickElectronicMenu = _canvasesList.CanvasMenuLockpickElectronic;
		_canvasNoteMenu = _canvasesList.CanvasMenuNote;
		_canvasLockpickMechanicalMenu = _canvasesList.CanvasMenuLockpickMechanical;
	}

	public void ShowCanvasMainMenuReadNews()
	{

		_canvasMainMenuReadNews.SetActive(true);

	}

	public void HideCanvasMainMenuReadNews()
	{

		_canvasMainMenuReadNews.SetActive(false);

	
	}

	public void ShowCanvasMainMenuChooseMission()
	{

		_canvasMainMenuChooseMission.SetActive(true);
	}


	public void HideCanvasMainMenuChooseMission()
	{
	
		_canvasMainMenuChooseMission.SetActive(false);

	
	}

	public void ShowPuzzleCanvas()
	{
			_canvasLockpickElectronicMenu.SetActive(true);
	}

	public void HidePuzzleCanvas()
	{
			_canvasLockpickElectronicMenu.SetActive(false);
	}

	public void HideNoteCanvas()
	{

			_canvasNoteMenu.SetActive(false);
		
	}

	public void ShowNoteCanvas()
	{
	
			_canvasNoteMenu.SetActive(true);
		
	}

	public void HidePuzzleMechanucalCanvas()
	{
	
			_canvasLockpickMechanicalMenu.SetActive(false);

		
	}

	public void ShowPuzzleMechanicalCanvas()
	{
	
			_canvasLockpickMechanicalMenu.SetActive(true);
	
		
	}
}
