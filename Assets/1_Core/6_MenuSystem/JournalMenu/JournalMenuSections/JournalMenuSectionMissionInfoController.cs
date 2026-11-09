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

	public void Initialize(
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager)
	{
		_gameSceneManager = gameSceneManager;

		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			if (SceneManager.sceneCount > 1)
			{
				if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
				{
					_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
				}
				else
				{
					_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_THERE-IS-NO-MISSION-GOAL-IN-TEST-SCENE");
				}
			}
			else
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
			}
		};

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
			}
			else
			{
				_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_THERE-IS-NO-MISSION-GOAL-IN-TEST-SCENE");
			}
		}
		else
		{
			_textComponentMissionStep.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
		}
	}
}
