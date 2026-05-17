using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class PauseSubMenuSettingsController : MonoBehaviour
{
	public delegate void ChangeSettingsOpenMenuEventHandler();
	public event ChangeSettingsOpenMenuEventHandler OnRequestSaveSettingsConfirmation;
	public event ChangeSettingsOpenMenuEventHandler OnRequestResetSettingsConfirmation;

	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private bool _isPauseSubMenuSettingsOpened;
	private GameObject _canvasPauseSubMenuSettings;
	private PauseMenuController _pauseMenuController;
	private GameObject _buttonClosePauseSubMenuSettings;
	private GameObject _sliderFOVgameObject;
	private Button[] _buttonsChangeLanguage;
	private char _lastValidChar; 
	private PauseSubMenuSettingsPlayerPrefs _pauseSubMenuSettingsPlayerPrefs;
	private LocalizationManager _localizationManager;
	private Slider _sliderFOV;

	private TextMeshProUGUI _fovDisplayText;

	private Camera _mainCamera;
	
	private Button[] _FPSbuttons;

	private Color _activeColor = Color.green;
	private Color _normalColor = Color.white;

	private GameObject _buttonSaveSettings;
	private GameObject _buttonResetSettings;

	private int _currentFrameRateLimit = 60;

	private const float _MIN_FOV_VALUE = 60f;
	private const float _MAX_FOV_VALUE = 120f;
	private Bootstrap _bootstrap;

	private GameController _gameController;
	private TMP_InputField[] _KeyRebinds;

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
		PauseSubMenuSettingsPlayerPrefs pauseSubMenuSettingsPlayerPrefs,
		GameObject canvasPauseSubMenuSettings,
		GameObject FOVSlider, 
		GameObject fovDisplayText,
		GameObject[] FPSbuttons, 
		GameObject[] buttonsChangeLanguage,
		GameObject[] KeyRebinds,
		GameObject buttonSaveSettings,
		GameObject buttonResetSettings,
		GameObject buttonClosePauseSubMenuSettings,
		GameObject mainCamera)
	{
		_gameController = gameController;
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_mainCamera = mainCamera.GetComponent<Camera>();
		_fovDisplayText = fovDisplayText.GetComponent<TextMeshProUGUI>();

		_sliderFOVgameObject = FOVSlider;
	
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
		_pauseSubMenuSettingsPlayerPrefs = pauseSubMenuSettingsPlayerPrefs;
		
		_buttonClosePauseSubMenuSettings.GetComponent<Button>().onClick.AddListener(() => this._pauseMenuController.ClosePauseSubMenu());
		
		_buttonSaveSettings.GetComponent<Button>().onClick.AddListener(() => OnRequestSaveSettingsConfirmation());
		_buttonResetSettings.GetComponent<Button>().onClick.AddListener(() => OnRequestResetSettingsConfirmation());

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

		var defaultBindings = _inputDevice.GetDefaultKeyBindings();
		List<string> actionNames = new List<string>(defaultBindings.Keys);

		SettingsData loadedData = pauseSubMenuSettingsPlayerPrefs.LoadSettings(actionNames);

		ApplyLoadedSettings(loadedData);
		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSubMenu Initialized");
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
		_mainCamera.fieldOfView = Mathf.Clamp(newFov, _MIN_FOV_VALUE, _MAX_FOV_VALUE);
		
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

	public void SaveSettings()
	{
		var currentData = new SettingsData();
		currentData.FOV = _mainCamera.fieldOfView;

		currentData.Language = _localizationManager.CurrentLanguage.ToString();

		currentData.KeyBindings = new Dictionary<string, KeyCode>(_inputDevice.CurrentKeyboardKeyBindings);

		_pauseSubMenuSettingsPlayerPrefs.SaveSettings(currentData);
	}

	public void ResetSettings()
	{
		_pauseSubMenuSettingsPlayerPrefs.ResetSettings();

		var defaultBindingsSnapshot = _inputDevice.GetDefaultKeyBindings();

		SettingsData defaultData = new SettingsData
		{
			FOV = _MIN_FOV_VALUE,
			KeyBindings = defaultBindingsSnapshot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		_pauseSubMenuSettingsPlayerPrefs.SaveSettings(defaultData);

		SetFOV(_MIN_FOV_VALUE);
		_sliderFOV.value = _MIN_FOV_VALUE;

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

	private void ApplyLoadedSettings(SettingsData data)
	{
		if (PlayerPrefs.HasKey(_pauseSubMenuSettingsPlayerPrefs.FOV))
		{
			SetFOV(data.FOV);
			_sliderFOV.value = data.FOV;
		}

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