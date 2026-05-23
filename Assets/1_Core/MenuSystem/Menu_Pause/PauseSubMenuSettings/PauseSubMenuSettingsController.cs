using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsController : MonoBehaviour
{
	private PauseMenuController _pauseMenuController;

	private GameObject _canvasPauseSubMenuSettings;

	private GameObject _subSettingsSectionGeneral;
	private GameObject _buttonSubSettingsSectionGeneral;
	private Button _buttonComponentSubSettingsSectionGeneral;

	private GameObject _subSettingsSectionControls;
	private GameObject _buttonSubSettingsSectionControls;
	private Button _buttonComponentSubSettingsSectionControls;

	private GameObject _subSettingsSectionGraphics;
	private GameObject _buttonSubSettingsSectionGraphics;
	private Button _buttonComponentSubSettingsSectionGraphics;

	private GameObject _subSettingsSectionAudio;
	private GameObject _buttonSubSettingsSectionAudio;
	private Button _buttonComponentSubSettingsSectionAudio;

	private GameObject _buttonSaveSettings;
	private Button _buttonComponentSaveSettings;
	private GameObject _buttonResetSettings;
	private Button _buttonComponentResetSettings;

	private GameObject _buttonClosePauseSubMenuSettings;
	private Button _buttonComponentClosePauseSubMenuSettings;

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
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSettings,
		GameObject subSettingsSectionGeneral,
		GameObject buttonSubSettingsSectionGeneral,
		GameObject subSettingsSectionControls,
		GameObject buttonSubSettingsSectionControls,
		GameObject subSettingsSectionGraphics,
		GameObject buttonSubSettingsSectionGraphics,
		GameObject subSettingsSectionAudio,
		GameObject buttonSubSettingsSectionAudio,
		GameObject buttonSaveSettings,
		GameObject buttonResetSettings,
		GameObject buttonClosePauseSubMenuSettings)
	{
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;

		_subSettingsSectionGeneral = subSettingsSectionGeneral;
		_buttonSubSettingsSectionGeneral = buttonSubSettingsSectionGeneral;

		_subSettingsSectionControls = subSettingsSectionControls;
		_buttonSubSettingsSectionControls = buttonSubSettingsSectionControls;

		_subSettingsSectionGraphics = subSettingsSectionGraphics;
		_buttonSubSettingsSectionGraphics = buttonSubSettingsSectionGraphics;

		_subSettingsSectionAudio = subSettingsSectionAudio;
		_buttonSubSettingsSectionAudio = buttonSubSettingsSectionAudio;

		_buttonSaveSettings = buttonSaveSettings;
		_buttonResetSettings = buttonResetSettings;
		_buttonClosePauseSubMenuSettings = buttonClosePauseSubMenuSettings;

		_buttonComponentSubSettingsSectionGeneral = _buttonSubSettingsSectionGeneral.GetComponent<Button>();
		_buttonComponentSubSettingsSectionControls = _buttonSubSettingsSectionControls.GetComponent<Button>();
		_buttonComponentSubSettingsSectionGraphics = _buttonSubSettingsSectionGraphics.GetComponent<Button>();
		_buttonComponentSubSettingsSectionAudio = _buttonSubSettingsSectionAudio.GetComponent<Button>();

		_buttonComponentSaveSettings = _buttonSaveSettings.GetComponent<Button>();
		_buttonComponentResetSettings = _buttonResetSettings.GetComponent<Button>();
		_buttonComponentClosePauseSubMenuSettings = _buttonClosePauseSubMenuSettings.GetComponent<Button>();

		_buttonComponentSubSettingsSectionGeneral.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGeneral));
		_buttonComponentSubSettingsSectionControls.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionControls));
		_buttonComponentSubSettingsSectionGraphics.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGraphics));
		_buttonComponentSubSettingsSectionAudio.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionAudio));

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

		_buttonComponentClosePauseSubMenuSettings.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_pauseMenuController.OnOpenSettingsSubMenu += () => OpenSubSettingsSection(_subSettingsSectionGeneral);
		_pauseMenuController.OnOpenSettingsSubMenu += ShowSettingsSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSettingsSubMenuCanvas;
		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSubMenu Initialized");
	}
	public void ShowSettingsSubMenuCanvas()
	{
		_isPauseSubMenuSettingsOpened = true;
		_canvasPauseSubMenuSettings.gameObject.SetActive(true);

		Debug.Log("Opened SettingsSubMenu");
	}

	public void HideSettingsSubMenuCanvas()
	{
		if (_isPauseSubMenuSettingsOpened)
		{
			_isPauseSubMenuSettingsOpened = false;
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

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.General.ToString();
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionControls.interactable = false;

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Controls.ToString();
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionGraphics.interactable = false;

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Graphics.ToString();
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);

			_buttonComponentSubSettingsSectionAudio.interactable = false;

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Audio.ToString();
		}
	}

	private void CloseSubSettingsSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(false);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			_buttonComponentSubSettingsSectionGeneral.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			_buttonComponentSubSettingsSectionControls.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			_buttonComponentSubSettingsSectionGraphics.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			_buttonComponentSubSettingsSectionAudio.interactable = true;
		}
	}

	private void DisableButtons()
	{
		if(_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
		{
			_buttonComponentSubSettingsSectionGeneral.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
		{
			_buttonComponentSubSettingsSectionControls.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
		{
			_buttonComponentSubSettingsSectionGraphics.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
		{
			_buttonComponentSubSettingsSectionAudio.interactable = false;
		}

		_buttonComponentSaveSettings.interactable = false;
		_buttonComponentResetSettings.interactable = false;
		_buttonComponentClosePauseSubMenuSettings.interactable = false;
	}

	private void EnableButtons()
	{
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
		{
			_buttonComponentSubSettingsSectionGeneral.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
		{
			_buttonComponentSubSettingsSectionControls.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
		{
			_buttonComponentSubSettingsSectionGraphics.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
		{
			_buttonComponentSubSettingsSectionAudio.interactable = true;
		}

		_buttonComponentSaveSettings.interactable = true;
		_buttonComponentResetSettings.interactable = true;
		_buttonComponentClosePauseSubMenuSettings.interactable = true;
	}
}