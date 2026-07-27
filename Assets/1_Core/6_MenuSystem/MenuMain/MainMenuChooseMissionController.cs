using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuChooseMissionController : MonoBehaviour
{
	private GameObject _canvasMainMenuChooseMission;
	public delegate void MainMenuChooseMissionHandler();
	public event MainMenuChooseMissionHandler OnCloseMainMenuChooseMission;
	private ViewModelMainMenuChooseMission _viewModelMainMenuChooseMission;

	private LocalizationManager _localizationManager;
	private GameScenesList _gameScenesList;

	private GameObject _textMainMenuChooseMission;
	private TextMeshProUGUI _textComponentMainMenuChooseMission;

	private GameObject[] _buttonsMissions;
	private Button[] _buttonsComponentsMissions;
	private GameObject[] _imagesMissions;
	private Image[] _imagesComponentsMissions;
	private GameObject[] _textsMissionsNames;
	private TextMeshProUGUI[] _textsComponentsMissionsNames;
	private GameObject[] _textsScenesNames;
	private TextMeshProUGUI[] _textsComponentsScenesNames;

	private GameObject _buttonCloseMainMenuChooseMission;
	private Button _buttonComponentCloseMainMenuChooseMission;
	private GameObject _textButtonCloseMainMenuChooseMission;
	private TextMeshProUGUI _textComponentButtonCloseMainMenuChooseMission;

	public bool IsMainMenuChooseMissionOpened { get; private set; }

	public void Initialize()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_canvasMainMenuChooseMission = ServiceLocator.Resolve<GameObject>("CanvasMainMenuChooseMission");
		_viewModelMainMenuChooseMission = ServiceLocator.Resolve<ViewModelMainMenuChooseMission>("ViewModelMainMenuChooseMission");
		_gameScenesList = ServiceLocator.Resolve<GameScenesList>("GameScenesList");

		_textComponentMainMenuChooseMission = _viewModelMainMenuChooseMission.TextMainMenuChooseMission.GetComponent<TextMeshProUGUI>();
		_textComponentMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextChooseDemoEpisode");

		_buttonsComponentsMissions = new Button[_viewModelMainMenuChooseMission.Missions.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.Missions.Length; i++)
		{
			_buttonsComponentsMissions[i] = _viewModelMainMenuChooseMission.Missions[i].GetComponent<Button>();
		}

		_imagesComponentsMissions = new Image[_viewModelMainMenuChooseMission.Missions.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.Missions.Length; i++)
		{
			_imagesComponentsMissions[i] = _viewModelMainMenuChooseMission.Missions[i].GetComponent<Image>();
		}
		_imagesComponentsMissions[0].sprite = _gameScenesList.GameScenes[3].SceneLoadingScreenImage;
		_imagesComponentsMissions[1].sprite = _gameScenesList.GameScenes[5].SceneLoadingScreenImage;
		_imagesComponentsMissions[2].sprite = _gameScenesList.GameScenes[6].SceneLoadingScreenImage;

		_textsComponentsMissionsNames = new TextMeshProUGUI[_viewModelMainMenuChooseMission.TextsMissionsNames.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.TextsMissionsNames.Length; i++)
		{
			_textsComponentsMissionsNames[i] = _viewModelMainMenuChooseMission.TextsMissionsNames[i].GetComponent<TextMeshProUGUI>();
		}
		_textsComponentsMissionsNames[0].text =  _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[3].GameMissionName.ToString());
		_textsComponentsMissionsNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameMissionName.ToString());
		_textsComponentsMissionsNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameMissionName.ToString());

		_textsComponentsScenesNames = new TextMeshProUGUI[_viewModelMainMenuChooseMission.TextsScenesNames.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.TextsScenesNames.Length; i++)
		{
			_textsComponentsScenesNames[i] = _viewModelMainMenuChooseMission.TextsScenesNames[i].GetComponent<TextMeshProUGUI>();
		}
		_textsComponentsScenesNames[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextPrologue");
		_textsComponentsScenesNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameScene.ToString());
		_textsComponentsScenesNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameScene.ToString());

		_buttonComponentCloseMainMenuChooseMission = _viewModelMainMenuChooseMission.ButtonCloseMainMenuChooseMission.GetComponent<Button>();
		_buttonComponentCloseMainMenuChooseMission.onClick.AddListener(() => HideCanvasMainMenuChooseMission());

		_textComponentButtonCloseMainMenuChooseMission = _viewModelMainMenuChooseMission.TextButtonCloseMainMenuChooseMission.GetComponent<TextMeshProUGUI>();
		_textComponentButtonCloseMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextButtonClose");

		_localizationManager.OnLanguageChanged += ChangeLanguage;

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

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextChooseDemoEpisode");

		_textsComponentsMissionsNames[0].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[3].GameMissionName.ToString());
		_textsComponentsMissionsNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameMissionName.ToString());
		_textsComponentsMissionsNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameMissionName.ToString());

		_textsComponentsScenesNames[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextPrologue");
		_textsComponentsScenesNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameScene.ToString());
		_textsComponentsScenesNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameScene.ToString());

		_textComponentButtonCloseMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextButtonClose");
	}
}
