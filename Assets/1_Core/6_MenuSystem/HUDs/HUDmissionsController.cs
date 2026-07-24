using UnityEngine;
using TMPro;
using System.Collections;

public class HUDmissionsController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasHUDmissions;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private GameObject _HUDmission;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;

	private GameObject _textNewMissionGoal;
	private TextMeshProUGUI _textComponentNewMissionGoal;
	private GameObject _textNewMissionGoalDisplay;
	private TextMeshProUGUI _textComponentNewMissionGoalDisplay;

	private GameObject _textCurrentMissionGoal;
	private TextMeshProUGUI _textComponentCurrentMissionGoal;

	public void Initialize(
		GameController gameController,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameObject canvasHUDmissions,
		ViewModelPauseMenu viewModelPauseMenu,
		ViewModelHUDMission viewModelHUDMission)
	{
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasHUDmissions = canvasHUDmissions;
		_HUDmission = viewModelHUDMission.HUDmission;

		_textNewMissionGoal = viewModelHUDMission.TextNewMissionGoal;
		_textComponentNewMissionGoal = viewModelHUDMission.TextNewMissionGoal.GetComponent<TextMeshProUGUI>();
		_textNewMissionGoalDisplay = viewModelHUDMission.TextNewMissionGoalDisplay;
		_textComponentNewMissionGoalDisplay = viewModelHUDMission.TextNewMissionGoalDisplay.GetComponent<TextMeshProUGUI>();

		_textCurrentMissionGoal = viewModelPauseMenu.TextCurrentMissionGoalDisplay;
		_textComponentCurrentMissionGoal = _textCurrentMissionGoal.GetComponent<TextMeshProUGUI>();

		_menuManager.OnOpenPauseMenu += HideCanvasHUDmissions;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenCutsceneMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseCutsceneMenu += ShowCanvasHUDmissions;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDmission;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDmissions;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDmissions;

		_gameController.OnPlayerEarlyDeath += HideCanvasHUDmissions;
	}

	public void SetCurrentMissionGoalText(string textGoal)
	{
		_textComponentCurrentMissionGoal.text = textGoal;
	}

	public void ShowNewMissionGoalHUDnotification(string textGoal)
	{
		StartCoroutine(ShowNewMissionGoalHUDnotificationCoroutine(textGoal));
	}

	private IEnumerator ShowNewMissionGoalHUDnotificationCoroutine(string textGoal)
	{
		_textNewMissionGoal.SetActive(true);
		_textNewMissionGoalDisplay.SetActive(true);

		_textComponentNewMissionGoalDisplay.text = textGoal;

		yield return new WaitForSeconds(3);

		HideNewMissionGoalHUDnotification();
	}

	private void HideNewMissionGoalHUDnotification()
	{
		_textNewMissionGoal.SetActive(false);
		_textNewMissionGoalDisplay.SetActive(false);
	}

	private void ShowCanvasHUDmissions()
	{
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOpen && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded)
		{
			_canvasHUDmissions.SetActive(true);

			Debug.Log("Show canvasMissions");
		}
	}

	private void HideCanvasHUDmissions()
	{
		HideNewMissionGoalHUDnotification();

		_canvasHUDmissions.SetActive(false);

		Debug.Log("Hide canvasMissions");
	}

	private void ShowHUDmission()
	{
		_HUDmission.SetActive(true);
	}

	private void HideHUDmission()
	{
		_HUDmission.SetActive(false);
	}
}