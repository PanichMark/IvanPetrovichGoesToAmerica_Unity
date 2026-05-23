using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class PauseSubMenuSettingsController : MonoBehaviour
{
	public delegate void ConfirmChangeSettingsEventHandler();
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsAudioConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsAudioConfirmation;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGeneralData;
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsControlsData;
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGraphicsData;
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsAudioData;



	public delegate void SavePlayerPrefsCameraSettingsEventHandler();
	public event SavePlayerPrefsCameraSettingsEventHandler OnSaveCameraSettingsData;



	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGeneralData;
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsControlsData;
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGraphicsData;
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsAudioData;


	public delegate void MainCameraFOVeventHandle(float newFov, float MIN_FOV_VALUE, float MAX_FOV_VALUE);
	public event MainCameraFOVeventHandle OnMainCameraFOVchanged;


	private string _currentOpenedSubSettingsSection;
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private bool _isPauseSubMenuSettingsOpened;
	private GameObject _canvasPauseSubMenuSettings;
	private PauseMenuController _pauseMenuController;
	private GameObject _buttonClosePauseSubMenuSettings;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private float _FOV;
	private GameObject _subSettingsSectionGeneral;
	private GameObject _buttonSubSettingsSectionGeneral;
	private GameObject _sliderFOVgameObject;
	private Slider _sliderFOV;
	private TextMeshProUGUI _fovDisplayText;
	private const float _MIN_FOV_VALUE = 60f;
	private const float _MAX_FOV_VALUE = 120f;
	private Button[] _FPSbuttons;
	private int _currentFrameRateLimit = 60;

	private GameObject _subSettingsSectionControls;
	private GameObject _buttonSubSettingsSectionControls;
	private TMP_InputField[] _KeyRebinds;

	private GameObject _subSettingsSectionGraphics;
	private GameObject _buttonSubSettingsSectionGraphics;

	private GameObject _subSettingsSectionAudio;
	private GameObject _buttonSubSettingsSectionAudio;
	private Button[] _buttonsChangeLanguage;
	private char _lastValidChar;

	private Color _activeColor = Color.green;
	private Color _normalColor = Color.white;

	private GameObject _buttonSaveSettings;
	private GameObject _buttonResetSettings;

	private readonly char[][] _layoutMap = new char[][]
	{
	new char[] {'Й', 'Q'}, new char[] {'Ц', 'W'}, new char[] {'У', 'E'}, new char[] {'К', 'R'},
	new char[] {'Е', 'T'}, new char[] {'Н', 'Y'}, new char[] {'Г', 'U'}, new char[] {'Ш', 'I'},
	new char[] {'Щ', 'O'}, new char[] {'З', 'P'}, new char[] {'Х', '['}, new char[] {'Ъ', ']'},
	new char[] {'Ф', 'A'}, new char[] {'Ы', 'S'}, new char[] {'В', 'D'}, new char[] {'А', 'F'},
	new char[] {'П', 'G'}, new char[] {'Р', 'H'}, new char[] {'О', 'J'}, new char[] {'Л', 'K'},
	new char[] {'Д', 'L'}, new char[] {'Ж', ';'}, new char[] {'Э', '\''}, new char[] {'Я', 'Z'},
	new char[] {'Ч', 'X'}, new char[] {'С', 'C'}, new char[] {'М', 'V'}, new char[] {'И', 'B'},
	new char[] {'Т', 'N'}, new char[] {'Ь', 'M'}, new char[] {'Б', ','}, new char[] {'Ю', '.'},
	new char[] {'.', '/'},
	  };

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSettings,
		GameObject subSettingsSectionGeneral,
		GameObject buttonSubSettingsSectionGeneral,
		GameObject FOVSlider,
		GameObject fovDisplayText,
		GameObject[] FPSbuttons,
		GameObject subSettingsSectionControls,
		GameObject buttonSubSettingsSectionControls,
		GameObject[] KeyRebinds,
		GameObject subSettingsSectionGraphics,
		GameObject buttonSubSettingsSectionGraphics,
		GameObject subSettingsSectionAudio,
		GameObject buttonSubSettingsSectionAudio,
		GameObject[] buttonsChangeLanguage,
		GameObject buttonSaveSettings,
		GameObject buttonResetSettings,
		GameObject buttonClosePauseSubMenuSettings)
	{
		_gameController = gameController;
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_fovDisplayText = fovDisplayText.GetComponent<TextMeshProUGUI>();

		_sliderFOVgameObject = FOVSlider;

		_subSettingsSectionGeneral = subSettingsSectionGeneral;
		_buttonSubSettingsSectionGeneral = buttonSubSettingsSectionGeneral;

		_subSettingsSectionControls = subSettingsSectionControls;
		_buttonSubSettingsSectionControls = buttonSubSettingsSectionControls;

		_subSettingsSectionGraphics = subSettingsSectionGraphics;
		_buttonSubSettingsSectionGraphics = buttonSubSettingsSectionGraphics;

		_subSettingsSectionAudio = subSettingsSectionAudio;
		_buttonSubSettingsSectionAudio = buttonSubSettingsSectionAudio;

		_FPSbuttons = new Button[FPSbuttons.Length];
		_buttonsChangeLanguage = new Button[buttonsChangeLanguage.Length];
		_KeyRebinds = new TMP_InputField[KeyRebinds.Length];
		_pauseMenuController = pauseMenuController;
		_menuManager = menuManager;
		_inputDevice = inputDevice;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		_buttonClosePauseSubMenuSettings = buttonClosePauseSubMenuSettings;
		_pauseMenuController.OnOpenSettingsSubMenu += ShowSettingsSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSettingsSubMenuCanvas;
		_buttonSaveSettings = buttonSaveSettings;
		_buttonResetSettings = buttonResetSettings;

		_buttonClosePauseSubMenuSettings.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_buttonSaveSettings.GetComponent<Button>().onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestSaveSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestSaveSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestSaveSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestSaveSettingsAudioConfirmation();
			}
		});

		_buttonResetSettings.GetComponent<Button>().onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestResetSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestResetSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestResetSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestResetSettingsAudioConfirmation();
			}
		});

		_sliderFOV = _sliderFOVgameObject.GetComponent<Slider>();

		_sliderFOV.minValue = _MIN_FOV_VALUE;
		_sliderFOV.maxValue = _MAX_FOV_VALUE;
		_sliderFOV.onValueChanged.AddListener(OnFovChanged);

		for (int i = 0; i < _FPSbuttons.Length; i++)
		{
			_FPSbuttons[i] = FPSbuttons[i].GetComponent<Button>();
		}

		for (int i = 0; i < _buttonsChangeLanguage.Length; i++)
		{
			_buttonsChangeLanguage[i] = buttonsChangeLanguage[i].GetComponent<Button>();
		}

		for (int i = 0; i < _KeyRebinds.Length; i++)
		{
			_KeyRebinds[i] = KeyRebinds[i].GetComponent<TMP_InputField>();
		}

		_FPSbuttons[0].onClick.AddListener(() => ChangeFrameRateLimit(30));
		_FPSbuttons[1].onClick.AddListener(() => ChangeFrameRateLimit(60));
		_FPSbuttons[2].onClick.AddListener(() => ChangeFrameRateLimit(90));
		_FPSbuttons[3].onClick.AddListener(() => ChangeFrameRateLimit(144));

		_buttonsChangeLanguage[0].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.Russian));
		_buttonsChangeLanguage[1].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.English));

		ChangeFrameRateLimit(60);
		ApplyButtonColors(_currentFrameRateLimit);
		_gameController.OnOpenMainMenu += () => SetFOV(60);

		var bindings = _inputDevice.GetCurrentKeyBindings().ToList();

		foreach (var field in _KeyRebinds)
		{
			var matchingBinding = bindings.FirstOrDefault(b => b.action == field.name.Replace("InputField", ""));
			if (matchingBinding != default)
			{
				field.text = matchingBinding.key.ToString();
			}
		}

		foreach (var field in _KeyRebinds)
		{
			field.onValidateInput += ValidateAndConvertInput;
			field.onEndEdit.AddListener((string text) =>
			{
				string actionName = field.name.Replace("InputField", "");
				HandleRebinding(actionName, text);
			});
			field.onValueChanged.AddListener((string text) => KeepLastCharacter(field));
		}

		_buttonSubSettingsSectionGeneral.GetComponent<Button>().onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGeneral));
		_buttonSubSettingsSectionControls.GetComponent<Button>().onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionControls));
		_buttonSubSettingsSectionGraphics.GetComponent<Button>().onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGraphics));
		_buttonSubSettingsSectionAudio.GetComponent<Button>().onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionAudio));

		_pauseMenuController.OnOpenSettingsSubMenu += () => OpenSubSettingsSection(_subSettingsSectionGeneral);

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSubMenu Initialized");
	}

	private void OpenSubSettingsSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(true);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			Button buttonSubSettingsSectionGeneral = _buttonSubSettingsSectionGeneral.GetComponent<Button>();
			buttonSubSettingsSectionGeneral.interactable = false;

			_currentOpenedSubSettingsSection = "General";
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			Button buttonSubSettingsSectionControls = _buttonSubSettingsSectionControls.GetComponent<Button>();
			buttonSubSettingsSectionControls.interactable = false;

			_currentOpenedSubSettingsSection = "Controls";
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			Button buttonSubSettingsSectionGraphics = _buttonSubSettingsSectionGraphics.GetComponent<Button>();
			buttonSubSettingsSectionGraphics.interactable = false;

			_currentOpenedSubSettingsSection = "Graphics";
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);

			Button buttonSubSettingsSectionAudio = _buttonSubSettingsSectionAudio.GetComponent<Button>();
			buttonSubSettingsSectionAudio.interactable = false;

			_currentOpenedSubSettingsSection = "Audio";
		}
	}

	private void CloseSubSettingsSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(false);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			Button buttonSubSettingsSectionGeneral = _buttonSubSettingsSectionGeneral.GetComponent<Button>();
			buttonSubSettingsSectionGeneral.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			Button buttonSubSettingsSectionControls = _buttonSubSettingsSectionControls.GetComponent<Button>();
			buttonSubSettingsSectionControls.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			Button buttonSubSettingsSectionGraphics = _buttonSubSettingsSectionGraphics.GetComponent<Button>();
			buttonSubSettingsSectionGraphics.interactable = true;
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			Button buttonSubSettingsSectionAudio = _buttonSubSettingsSectionAudio.GetComponent<Button>();
			buttonSubSettingsSectionAudio.interactable = true;
		}
	}

	private void DisableButtons()
	{
		Button saveButton = _buttonSaveSettings.GetComponent<Button>();
		if (saveButton != null) saveButton.interactable = false;

		Button resetButton = _buttonResetSettings.GetComponent<Button>();
		if (resetButton != null) resetButton.interactable = false;

		Button closeButton = _buttonClosePauseSubMenuSettings.GetComponent<Button>();
		if (closeButton != null) closeButton.interactable = false;

		foreach (var button in _buttonsChangeLanguage)
		{
			if (button != null) button.interactable = false;
		}

		foreach (var button in _FPSbuttons)
		{
			if (button != null) button.interactable = false;
		}

		if (_sliderFOV != null) _sliderFOV.interactable = false;

		foreach (var field in _KeyRebinds)
		{
			if (field != null) field.interactable = false;
		}

		if(_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
		{
			Button buttonSubSettingsSectionGeneral = _buttonSubSettingsSectionGeneral.GetComponent<Button>();
			buttonSubSettingsSectionGeneral.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
		{
			Button buttonSubSettingsSectionControls = _buttonSubSettingsSectionControls.GetComponent<Button>();
			buttonSubSettingsSectionControls.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
		{
			Button buttonSubSettingsSectionGraphics = _buttonSubSettingsSectionGraphics.GetComponent<Button>();
			buttonSubSettingsSectionGraphics.interactable = false;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
		{
			Button buttonSubSettingsSectionAudio = _buttonSubSettingsSectionAudio.GetComponent<Button>();
			buttonSubSettingsSectionAudio.interactable = false;
		}
	}

	private void EnableButtons()
	{
		Button saveButton = _buttonSaveSettings.GetComponent<Button>();
		if (saveButton != null) saveButton.interactable = true;

		Button resetButton = _buttonResetSettings.GetComponent<Button>();
		if (resetButton != null) resetButton.interactable = true;

		Button closeButton = _buttonClosePauseSubMenuSettings.GetComponent<Button>();
		if (closeButton != null) closeButton.interactable = true;

		foreach (var button in _buttonsChangeLanguage)
		{
			if (button != null) button.interactable = true;
		}

		foreach (var button in _FPSbuttons)
		{
			if (button != null) button.interactable = true;
		}

		if (_sliderFOV != null) _sliderFOV.interactable = true;

		foreach (var field in _KeyRebinds)
		{
			if (field != null) field.interactable = true;
		}

		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.General.ToString())
		{
			Button buttonSubSettingsSectionGeneral = _buttonSubSettingsSectionGeneral.GetComponent<Button>();
			buttonSubSettingsSectionGeneral.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Controls.ToString())
		{
			Button buttonSubSettingsSectionControls = _buttonSubSettingsSectionControls.GetComponent<Button>();
			buttonSubSettingsSectionControls.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Graphics.ToString())
		{
			Button buttonSubSettingsSectionGraphics = _buttonSubSettingsSectionGraphics.GetComponent<Button>();
			buttonSubSettingsSectionGraphics.interactable = true;
		}
		if (_currentOpenedSubSettingsSection != PauseSubMenuSettingsSectionTypes.Audio.ToString())
		{
			Button buttonSubSettingsSectionAudio = _buttonSubSettingsSectionAudio.GetComponent<Button>();
			buttonSubSettingsSectionAudio.interactable = true;
		}
	}

	private void ChangeLanguage(LanguagesEnum language)
	{
		_bootstrap.ChangeLanguage(language);
		Debug.Log("Changed Language to: " + language);
	}
	
	private void KeepLastCharacter(TMP_InputField field)
	{
		if (!string.IsNullOrEmpty(field.text))
		{
			field.text = field.text[field.text.Length - 1].ToString(); 
		}
	}
	char ValidateAndConvertInput(string text, int charIndex, char addedChar)
	{
		if (char.IsControl(addedChar)) return addedChar; 

		char upperCaseChar = char.ToUpperInvariant(addedChar); 

		if (char.IsLetter(upperCaseChar) && upperCaseChar <= 'Z')
		{
			_lastValidChar = upperCaseChar; 
			return upperCaseChar;
		}

		foreach (var entry in _layoutMap)
		{
			if (entry[0] == upperCaseChar)
			{
				_lastValidChar = entry[1]; 
				return entry[1];        
			}
		}

		Debug.LogWarning($"Символ {upperCaseChar} не обнаружен в раскладке!");
		return _lastValidChar; 
	}
	
	public void HideSettingsSubMenuCanvas()
	{
		if (_isPauseSubMenuSettingsOpened)
		{
			_isPauseSubMenuSettingsOpened = false;
			_canvasPauseSubMenuSettings.gameObject.SetActive(false);

			Debug.Log("SettingsSubMenu closed");
		}
	}

	public void ShowSettingsSubMenuCanvas()
	{
		_isPauseSubMenuSettingsOpened = true;
		_canvasPauseSubMenuSettings.gameObject.SetActive(true);
	}

	public void OnFovChanged(float value)
	{
		SetFOV(value);
	}

	private void SetFOV(float newFov)
	{
		OnMainCameraFOVchanged?.Invoke(newFov, _MIN_FOV_VALUE, _MAX_FOV_VALUE);

		_fovDisplayText.text = ((int)newFov).ToString();
	}

	private void ChangeFrameRateLimit(int frameRate)
	{
		Application.targetFrameRate = frameRate;
		_currentFrameRateLimit = frameRate;
		ApplyButtonColors(frameRate);
		Debug.Log($"Frame rate limit set to {frameRate}");
	}

	private void ApplyButtonColors(int activeFrameRate)
	{
		ResetAllButtons();

		switch (activeFrameRate)
		{
			case 30:
				HighlightButton(_FPSbuttons[0]);
				break;
			case 60:
				HighlightButton(_FPSbuttons[1]);
				break;
			case 90:
				HighlightButton(_FPSbuttons[2]);
				break;
			case 144:
				HighlightButton(_FPSbuttons[3]);
				break;
		}
	}

	private void ResetAllButtons()
	{
		_FPSbuttons[0].image.color = _normalColor;
		_FPSbuttons[1].image.color = _normalColor;
		_FPSbuttons[2].image.color = _normalColor;
		_FPSbuttons[3].image.color = _normalColor;
	}

	private void HighlightButton(Button button)
	{
		button.image.color = _activeColor;
	}

	void HandleRebinding(string actionName, string newKeyStr)
	{
		if (!Enum.TryParse<KeyCode>(newKeyStr, out KeyCode newKey))
		{
			return;
		}

		var currentBindings = _inputDevice.GetCurrentKeyBindings().ToDictionary(kvp => kvp.action, kvp => kvp.key);

		var conflictingAction = currentBindings.FirstOrDefault(kvp => kvp.Value == newKey && kvp.Key != actionName).Key;

		if (conflictingAction != null)
		{
			KeyCode oldKeyOfThisAction = currentBindings[actionName];

			_inputDevice.RebindKey(actionName, newKey);
			_inputDevice.RebindKey(conflictingAction, oldKeyOfThisAction);

			UpdateInputFieldText(actionName, newKey);
			UpdateInputFieldText(conflictingAction, oldKeyOfThisAction);
		}
		else
		{
			_inputDevice.RebindKey(actionName, newKey);
			UpdateInputFieldText(actionName, newKey);
		}
	}

	private void UpdateInputFieldText(string actionName, KeyCode key)
	{
		foreach (var field in _KeyRebinds)
		{
			if (field.name.StartsWith(actionName))
			{
				field.text = key.ToString();
				break;
			}
		}
	}

	public void GetCameraCurrentFOV(float FOV)
	{
		_FOV = FOV;
	}

	public void SaveSettingsGeneral()
	{
		var currentData = new PlayerPrefsData();

		OnSaveCameraSettingsData?.Invoke();

		currentData.FOV = _FOV;

		OnSaveSettingsGeneralData?.Invoke(currentData);
	}

	public void SaveSettingsControls()
	{
		var currentData = new PlayerPrefsData();

		currentData.KeyBindings = new Dictionary<string, KeyCode>(_inputDevice.CurrentKeyboardKeyBindings);

		OnSaveSettingsControlsData?.Invoke(currentData);
	}

	public void SaveSettingsGraphics()
	{
		var currentData = new PlayerPrefsData();

		OnSaveSettingsGraphicsData?.Invoke(currentData);
	}
	public void SaveSettingsAudio()
	{
		var currentData = new PlayerPrefsData();

		currentData.Language = _localizationManager.CurrentLanguage.ToString();

		OnSaveSettingsAudioData?.Invoke(currentData);
	}

	public void ResetSettingsGeneral()
	{
		OnResetSettingsGeneralData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			FOV = _MIN_FOV_VALUE,
		};

		OnSaveSettingsGeneralData?.Invoke(defaultData);

		SetFOV(_MIN_FOV_VALUE);
		_sliderFOV.value = _MIN_FOV_VALUE;
	}

	public void ResetSettingsControls()
	{
		OnResetSettingsControlsData?.Invoke();

		var defaultBindingsSnapshot = _inputDevice.GetDefaultKeyBindings();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			KeyBindings = defaultBindingsSnapshot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		OnSaveSettingsControlsData?.Invoke(defaultData);

		foreach (var field in _KeyRebinds)
		{
			string actionName = field.name.Replace("InputField", "");

			if (defaultData.KeyBindings.TryGetValue(actionName, out var key))
			{
				field.text = key.ToString();
				_inputDevice.RebindKey(actionName, key);
			}
		}
	}

	public void ResetSettingsGraphics()
	{
		OnResetSettingsGraphicsData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{

		};

		OnSaveSettingsGraphicsData?.Invoke(defaultData);
	}

	public void ResetSettingsAudio()
	{
		OnResetSettingsAudioData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			Language = _localizationManager.CurrentLanguage.ToString(),
		};

		OnSaveSettingsAudioData?.Invoke(defaultData);
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetFOV(data.FOV);
		_sliderFOV.value = data.FOV;

		if (data.KeyBindings != null && data.KeyBindings.Count > 0)
		{
			foreach (var kvp in data.KeyBindings)
			{
				string actionName = kvp.Key; 
				KeyCode savedKey = kvp.Value; 

				bool foundField = false;
				foreach (var field in _KeyRebinds)
				{
					string fieldActionName = field.name.Replace(" (InputField)", "");

					if (fieldActionName == actionName)
					{
						field.text = savedKey.ToString();
						_inputDevice.RebindKey(actionName, savedKey);
						foundField = true;
						break; 
					}
				}
			}
		}
	}
}