using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

	private GameObject _missionMaps;

	private GameObject _textNoMapFound;
	private TextMeshProUGUI _textComponentNoMapFound;

	private Mission _currentMission;

	private GameObject _textMissionMapName;
	private TextMeshProUGUI _textComponentMissionMapName;

	private GameObject _imageMissionMap;
	private Image _imageComponentMissionMap;

	private GameObject _textMissionMapDescription;
	private TextMeshProUGUI _textComponentMissionMapDescription;

	private GameObject _buttonNextMissionMap;
	private Button _buttonComponentNextMissionMap;

	private GameObject _buttonPreviousMissionMap;
	private Button _buttonComponentPreviousMissionMap;

	//private JournalMenuController _journalMenuController;
	private Bootstrap _bootstrap;
	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		//JournalMenuController journalMenuController,
		ViewModelJournalMenuSectionMissionInfo viewModel)
	{
		_bootstrap = bootstrap;
		_gameSceneManager = gameSceneManager;
		_localizationManager = localizationManager;
		//_journalMenuController = journalMenuController;
		// --- СУЩЕСТВУЮЩИЕ ПОЛЯ (Миссия, Шаг, Описание, Цель) ---
		_textMissionStep = viewModel.TextMissionStep;
		_imageMissionTarget = viewModel.ImageMissionTarget;
		_textMissionDescription = viewModel.TextMissionDescription;
		_textMissionName = viewModel.TextMissionName;

		_textComponentMissionStep = _textMissionStep.GetComponent<TextMeshProUGUI>();
		_imageComponentMissionTarget = _imageMissionTarget.GetComponent<Image>();
		_textComponentMissionDescription = _textMissionDescription.GetComponent<TextMeshProUGUI>();
		_textComponentMissionName = _textMissionName.GetComponent<TextMeshProUGUI>();

		// --- НОВЫЕ ПОЛЯ (Карты миссий) ---
		_textMissionMapName = viewModel.TextMissionMapName;
		_imageMissionMap = viewModel.ImageMissionMap;
		_textMissionMapDescription = viewModel.TextMissionMapDescription;
		_buttonNextMissionMap = viewModel.ButtonNextMissionMap;
		_buttonPreviousMissionMap = viewModel.ButtonPreviousMissionMap;

		_textComponentMissionMapName = _textMissionMapName.GetComponent<TextMeshProUGUI>();
		_imageComponentMissionMap = _imageMissionMap.GetComponent<Image>();
		_textComponentMissionMapDescription = _textMissionMapDescription.GetComponent<TextMeshProUGUI>();
		_buttonComponentNextMissionMap = _buttonNextMissionMap.GetComponent<Button>();
		_buttonComponentPreviousMissionMap = _buttonPreviousMissionMap.GetComponent<Button>();

		_missionMaps = viewModel.MissionMaps;
		_textNoMapFound = viewModel.TextNoMapFound;
		_textComponentNoMapFound = _textNoMapFound.GetComponent<TextMeshProUGUI>();

		// --- СУЩЕСТВУЮЩАЯ ЛОГИКА ПОДПИСОК ---
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

					_textComponentMissionName.text = _localizationManager.GetLocalizedString("Mission_Test");
				}
			}
			else
			{
				_textComponentMissionStep.text = null;
			}
		};

		_buttonComponentNextMissionMap.onClick.AddListener(ShowNextMap);
		_buttonComponentPreviousMissionMap.onClick.AddListener(ShowPreviousMap);

		//_journalMenuController.OnOpenJournaMenuSectionMissionInfo += OnSectionOpen;

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private int _currentMissionMapIndex;
	private List<MissionMapData> _currentMissionMapData;

	public void ApplyMissionMapsData(List<MissionMapData> collectedMissionMapsData)
	{
		_currentMissionMapData = collectedMissionMapsData;
		_currentMissionMapIndex = 0;
		RenderCurrentMap();
		_buttonComponentPreviousMissionMap.interactable = false;
		_buttonComponentNextMissionMap.interactable = _currentMissionMapData.Count > 1;
	}

	public void ShowNextMap()
	{
		_currentMissionMapIndex++;
		RenderCurrentMap();
		_buttonComponentPreviousMissionMap.interactable = true;
		_buttonComponentNextMissionMap.interactable = _currentMissionMapIndex < _currentMissionMapData.Count - 1;
	}

	public void ShowPreviousMap()
	{
		_currentMissionMapIndex--;
		RenderCurrentMap();
		_buttonComponentNextMissionMap.interactable = true;
		_buttonComponentPreviousMissionMap.interactable = _currentMissionMapIndex > 0;
	}

	private void RenderCurrentMap()
	{
		if (_currentMissionMapData.Count > 0)
		{
			MissionMapData currentMap = _currentMissionMapData[_currentMissionMapIndex];
			_imageComponentMissionMap.sprite = currentMap.MissionMapNoteData.NoteImage;
			_textComponentMissionMapName.text = _localizationManager.GetLocalizedString(currentMap.MissionMapID.ToString());
			_textComponentMissionMapDescription.text = _localizationManager.GetNoteLanguageSuffix(currentMap.MissionMapNoteData);

			_missionMaps.SetActive(true);
			_textNoMapFound.SetActive(false);
		}
		else
		{
			_missionMaps.SetActive(false);
			_textNoMapFound.SetActive(true);

			_textComponentNoMapFound.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_SectionMissionInfo_NoMapFound");
		}
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

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
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
			else
			{
				if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
				{
					_textComponentMissionDescription.text = _bootstrap.GameData.GameMissionsList.MissionTest.MissionDescription_RU.text;
				}
				if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
				{
					_textComponentMissionDescription.text = _bootstrap.GameData.GameMissionsList.MissionTest.MissionDescription_EN.text;
				}

				_textComponentMissionName.text = _localizationManager.GetLocalizedString(_bootstrap.GameData.GameMissionsList.MissionTest.MissionName.ToString());

				_imageComponentMissionTarget.sprite = _bootstrap.GameData.GameMissionsList.MissionTest.MissionTargetSprite;
			}
		}
	
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

				_textComponentMissionName.text = _localizationManager.GetLocalizedString("Mission_Test");

				if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
				{
					_textComponentMissionDescription.text = _bootstrap.GameData.GameMissionsList.MissionTest.MissionDescription_RU.text;
				}
				if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
				{
					_textComponentMissionDescription.text = _bootstrap.GameData.GameMissionsList.MissionTest.MissionDescription_EN.text;
				}
			}
		}

		_textComponentNoMapFound.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_SectionMissionInfo_NoMapFound");
	}
}
