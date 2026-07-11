using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsController : MonoBehaviour
{
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private GameObject _canvasPauseSubMenuSettings;

	private GameObject _buttonSaveSettings;
	private Button _buttonComponentSaveSettings;
	private GameObject _textButtonSaveSettings;
	private TextMeshProUGUI _textComponentButtonSaveSettings;

	private GameObject _buttonResetSettings;
	private Button _buttonComponentResetSettings;
	private GameObject _textButtonResetSettings;
	private TextMeshProUGUI _textComponentButtonResetSettings;

	private GameObject _buttonClosePauseSubMenuSettings;
	private Button _buttonComponentClosePauseSubMenuSettings;
	private GameObject _textButtonClosePauseSubMenuSettings;
	private TextMeshProUGUI _textComponentButtonClosePauseSubMenuSettings;

	private GameObject _subSettingsSectionGeneral;
	private GameObject _imageBackgroundSectionGeneral;
	private GameObject _buttonSubSettingsSectionGeneral;
	private Button _buttonComponentSubSettingsSectionGeneral;
	private GameObject _textButtonSubSettingsSectionGeneral;
	private TextMeshProUGUI _textComponentButtonSubSettingsSectionGeneral;

	private GameObject _subSettingsSectionControls;
	private GameObject _imageBackgroundSectionControls;
	private GameObject _buttonSubSettingsSectionControls;
	private Button _buttonComponentSubSettingsSectionControls;
	private GameObject _textButtonSubSettingsSectionControls;
	private TextMeshProUGUI _textComponentButtonSubSettingsSectionControls;

	private GameObject _subSettingsSectionGraphics;
	private GameObject _imageBackgroundSectionGraphics;
	private GameObject _buttonSubSettingsSectionGraphics;
	private Button _buttonComponentSubSettingsSectionGraphics;
	private GameObject _textButtonSubSettingsSectionGraphics;
	private TextMeshProUGUI _textComponentButtonSubSettingsSectionGraphics;

	private GameObject _subSettingsSectionAudio;
	private GameObject _imageBackgroundSectionAudio;
	private GameObject _buttonSubSettingsSectionAudio;
	private Button _buttonComponentSubSettingsSectionAudio;
	private GameObject _textButtonSubSettingsSectionAudio;
	private TextMeshProUGUI _textComponentButtonSubSettingsSectionAudio;

	private string _currentOpenedSubSettingsSection;
	private bool _isPauseSubMenuSettingsOpened;

	public delegate void ConfirmChangeSettingsEventHandler();
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsAudioConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsAudioConfirmation;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSettings,
		ViewModelPauseSubMenuSettings viewModelPauseSubMenuSettings)
	{
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;

		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;

		_buttonSaveSettings = viewModelPauseSubMenuSettings.ButtonSaveGameSettings;
		_buttonComponentSaveSettings = viewModelPauseSubMenuSettings.ButtonSaveGameSettings.GetComponent<Button>();
		_buttonComponentSaveSettings.onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestSaveSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestSaveSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestSaveSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestSaveSettingsAudioConfirmation();
			}
		});
		_textButtonSaveSettings = viewModelPauseSubMenuSettings.TextButtonSaveGameSettings;
		_textComponentButtonSaveSettings = viewModelPauseSubMenuSettings.TextButtonSaveGameSettings.GetComponent<TextMeshProUGUI>();

		_buttonResetSettings = viewModelPauseSubMenuSettings.ButtonResetGameSettings;
		_buttonComponentResetSettings = viewModelPauseSubMenuSettings.ButtonResetGameSettings.GetComponent<Button>();
		_buttonComponentResetSettings.GetComponent<Button>().onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestResetSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestResetSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestResetSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestResetSettingsAudioConfirmation();
			}
		});
		_textButtonResetSettings = viewModelPauseSubMenuSettings.TextButtonResetGameSettings;
		_textComponentButtonResetSettings = viewModelPauseSubMenuSettings.TextButtonResetGameSettings.GetComponent<TextMeshProUGUI>();

		_buttonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.ButtonClosePauseSubMenuSettings;
		_buttonComponentClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.ButtonClosePauseSubMenuSettings.GetComponent<Button>();
		_buttonComponentClosePauseSubMenuSettings.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.TextButtonClosePauseSubMenuSettings;
		_textComponentButtonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.TextButtonClosePauseSubMenuSettings.GetComponent<TextMeshProUGUI>();

		_subSettingsSectionGeneral = viewModelPauseSubMenuSettings.SubSettingsSectionGeneral;
		_imageBackgroundSectionGeneral = viewModelPauseSubMenuSettings.ImageBackgroundSectionGeneral;
		_buttonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGeneral;
		_buttonComponentSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGeneral.GetComponent<Button>();
		_buttonComponentSubSettingsSectionGeneral.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGeneral));
		_textButtonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGeneral;
		_textComponentButtonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGeneral.GetComponent<TextMeshProUGUI>();

		_subSettingsSectionControls = viewModelPauseSubMenuSettings.SubSettingsSectionControls;
		_imageBackgroundSectionControls = viewModelPauseSubMenuSettings.ImageBackgroundSectionControls;
		_buttonSubSettingsSectionControls = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionControls;
		_buttonComponentSubSettingsSectionControls = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionControls.GetComponent<Button>();
		_buttonComponentSubSettingsSectionControls.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionControls));
		_textButtonSubSettingsSectionControls = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionControls;
		_textComponentButtonSubSettingsSectionControls = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionControls.GetComponent<TextMeshProUGUI>();

		_subSettingsSectionGraphics = viewModelPauseSubMenuSettings.SubSettingsSectionGraphics;
		_imageBackgroundSectionGraphics = viewModelPauseSubMenuSettings.ImageBackgroundSectionGraphics;
		_buttonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGraphics;
		_buttonComponentSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGraphics.GetComponent<Button>();
		_buttonComponentSubSettingsSectionGraphics.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGraphics));
		_textButtonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGraphics;
		_textComponentButtonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGraphics.GetComponent<TextMeshProUGUI>();

		_subSettingsSectionAudio = viewModelPauseSubMenuSettings.SubSettingsSectionAudio;
		_imageBackgroundSectionAudio = viewModelPauseSubMenuSettings.ImageBackgroundSectionAudio;
		_buttonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionAudio;
		_buttonComponentSubSettingsSectionAudio = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionAudio.GetComponent<Button>();
		_buttonComponentSubSettingsSectionAudio.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionAudio));
		_textButtonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionAudio;
		_textComponentButtonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionAudio.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenSettingsSubMenu += () => OpenSubSettingsSection(_subSettingsSectionGeneral);
		_pauseMenuController.OnOpenSettingsSubMenu += () =>
		{
			ShowSettingsSubMenuCanvas();
			_isPauseSubMenuSettingsOpened = true;
		};
		_pauseMenuController.OnCloseAnyPauseSubMenu += () =>
		{
			HideSettingsSubMenuCanvas();
			_isPauseSubMenuSettingsOpened = false;
		};
		// do not open Settings Sub Menu while closing Confirmation menu

		Debug.Log("PauseSubMenuSettingsController Initialized");
	}

	public void ShowSettingsSubMenuCanvas()
	{
		_canvasPauseSubMenuSettings.gameObject.SetActive(true);

		Debug.Log("Opened SettingsSubMenu");
	}

	public void HideSettingsSubMenuCanvas()
	{
		if (_isPauseSubMenuSettingsOpened)
		{
			_canvasPauseSubMenuSettings.gameObject.SetActive(false);

			Debug.Log("Closed SettingsSubMenu");
		}
	}

	private void OpenSubSettingsSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(true);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionGeneral.interactable = false;
			_imageBackgroundSectionGeneral.SetActive(true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.General.ToString();
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionControls.interactable = false;
			_imageBackgroundSectionControls.SetActive(true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Controls.ToString();
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionGraphics.interactable = false;
			_imageBackgroundSectionGraphics.SetActive(true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Graphics.ToString();
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);

			_buttonComponentSubSettingsSectionAudio.interactable = false;
			_imageBackgroundSectionAudio.SetActive(true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Audio.ToString();
		}
	}

	private void CloseSubSettingsSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(false);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			_buttonComponentSubSettingsSectionGeneral.interactable = true;
			_imageBackgroundSectionGeneral.SetActive(false);
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			_buttonComponentSubSettingsSectionControls.interactable = true;
			_imageBackgroundSectionControls.SetActive(false);
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			_buttonComponentSubSettingsSectionGraphics.interactable = true;
			_imageBackgroundSectionGraphics.SetActive(false);
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			_buttonComponentSubSettingsSectionAudio.interactable = true;
			_imageBackgroundSectionAudio.SetActive(false);
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentButtonSaveSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonSaveSettings");
		_textComponentButtonResetSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonResetSettings");
		_textComponentButtonClosePauseSubMenuSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonClosePauseSubMenuSettings");

		_textComponentButtonSubSettingsSectionGeneral.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonSubSettingsSectionGeneral");
		_textComponentButtonSubSettingsSectionControls.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonSubSettingsSectionControls");
		_textComponentButtonSubSettingsSectionGraphics.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonSubSettingsSectionGraphics");
		_textComponentButtonSubSettingsSectionAudio.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettings_ButtonSubSettingsSectionAudio");
	}
}