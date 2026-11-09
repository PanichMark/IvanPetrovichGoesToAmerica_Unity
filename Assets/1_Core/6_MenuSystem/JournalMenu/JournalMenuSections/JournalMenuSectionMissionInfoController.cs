using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JournalMenuSectionMissionInfoController : MonoBehaviour
{
	private GameObject _textMissionStep;
	private TextMeshProUGUI _textComponentMissionStep;

	private GameObject _imageMissionTarget;
	private Image _imageComponentMissionTarget;

	private GameObject _textMissionDescription;
	private TextMeshProUGUI _textComponentMissionDescription;

	private GameObject _textMissionName;
	private TextMeshProUGUI _textComponentMissionName;

	private GameScenesManager _gameSceneManager;
	private LocalizationManager _localizationManager;

	private string _textGoal;

	private Mission _currentMission;

	public void Initialize(
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		ViewModelJournalMenuSectionMissionInfo viewModelJournalMenuSectionMissionInfo)
	{
		_gameSceneManager = gameSceneManager;
		_localizationManager = localizationManager;

		_textMissionStep = viewModelJournalMenuSectionMissionInfo.TextMissionStep;
		_imageMissionTarget = viewModelJournalMenuSectionMissionInfo.ImageMissionTarget;
		_textMissionDescription = viewModelJournalMenuSectionMissionInfo.TextMissionDescription;
		_textMissionName = viewModelJournalMenuSectionMissionInfo.TextMissionName;

		_textComponentMissionStep = _textMissionStep.GetComponent<TextMeshProUGUI>();
		_imageComponentMissionTarget = _imageMissionTarget.GetComponent<Image>();
		_textComponentMissionDescription = _textMissionDescription.GetComponent<TextMeshProUGUI>();
		_textComponentMissionName = _textMissionName.GetComponent<TextMeshProUGUI>();

		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			if (SceneManager.sceneCount > 1)
			{
				if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
				{
					_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current") + " " + _textGoal;
				}
				else
				{
					_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_THERE-IS-NO-MISSION-GOAL-IN-TEST-SCENE");
				}
			}
			else
			{
				_textComponentMissionStep.text = null;
			}
		};

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void SetMissionStep(string textGoal)
	{
		_textGoal = textGoal;

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current") + " " + _textGoal;
			}
			else
			{
				//._textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
				_textComponentMissionStep.text = null;
			}
		}
	}

	public void SetMissionInfo(Mission missionData)
	{
		_currentMission = missionData;

		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			_textComponentMissionDescription.text = _currentMission.MissionDescription_RU.text;
		}
		if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
		{
			_textComponentMissionDescription.text = _currentMission.MissionDescription_EN.text;
		}

		_textComponentMissionName.text = _localizationManager.GetLocalizedString(_currentMission.MissionName.ToString());

		_imageComponentMissionTarget.sprite = _currentMission.MissionTargetSprite;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current") + " " + _textGoal;

				if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
				{
					_textComponentMissionDescription.text = _currentMission.MissionDescription_RU.text;
				}
				if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
				{
					_textComponentMissionDescription.text = _currentMission.MissionDescription_EN.text;
				}

				_textComponentMissionName.text = _localizationManager.GetLocalizedString(_currentMission.MissionName.ToString());
			}
			else
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_THERE-IS-NO-MISSION-GOAL-IN-TEST-SCENE");
			}
		}
		else
		{
			_textComponentMissionStep.text = null;
		}


	}
}
