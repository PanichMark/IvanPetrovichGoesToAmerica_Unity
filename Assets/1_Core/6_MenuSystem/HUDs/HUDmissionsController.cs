using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDmissionsController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasHUDmissions;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private GameObject _HUDmission;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private LocalizationManager _localizationManager;
	private GameObject _textNewMissionGoal;
	private TextMeshProUGUI _textComponentNewMissionGoal;
	private GameObject _textNewMissionGoalDisplay;
	private TextMeshProUGUI _textComponentNewMissionGoalDisplay;

	private GameObject _textCurrentMissionGoal;
	private TextMeshProUGUI _textComponentCurrentMissionGoal;

	private string _textGoal;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameObject canvasHUDmissions,
		ViewModelPauseMenu viewModelPauseMenu,
		ViewModelHUDMission viewModelHUDMission)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
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

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasHUDmissions;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDmissions;

		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			/*
			if (SceneManager.sceneCount > 1)
			{
				//Debug.Log("MISSIONBREUH");
				//Debug.Log(SceneManager.GetSceneAt(1).name);

				if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
				{
					_textComponentCurrentMissionGoal.text = _textGoal;
					//Debug.Log("SHOW");
				}
				else
				{
					//._textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
					_textComponentCurrentMissionGoal.text = null;
					//Debug.Log("DONT SHOW");
				}
			}
			*/
		};

		_gameController.OnPlayerEarlyDeath += HideCanvasHUDmissions;
	}

	public void SetCurrentMissionGoalText(string textGoal)
	{
		_textGoal = textGoal;

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				_textComponentCurrentMissionGoal.text = _textGoal;
			}
			else
			{
				//._textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
				_textComponentCurrentMissionGoal.text = null;
			}
		}
	}

	public void ShowNewMissionGoalHUDnotification(string textGoal, bool isNewGoal)
	{
		if (SceneManager.sceneCount > 1)
		{
			Debug.Log("NEW MISSION NOTIFICATION!");
		StopAllCoroutines();

		
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				StartCoroutine(ShowNewMissionGoalHUDnotificationCoroutine(textGoal, isNewGoal));
				//Debug.Log("SHOW");
			}
		}
	}

	private IEnumerator ShowNewMissionGoalHUDnotificationCoroutine(string textGoal, bool isNewGoal)
	{
		_textNewMissionGoal.SetActive(true);
		_textNewMissionGoalDisplay.SetActive(true);

		if (isNewGoal)
		{
			_textComponentNewMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_New");
		}
		else
		{
			_textComponentNewMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
		}

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
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOrEndGameTitlesActive && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded)
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