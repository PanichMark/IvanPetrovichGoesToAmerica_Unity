using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionControlsController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private GameObject[] _inputFieldsControls;
	private TMP_InputField[] _inputFieldsComponentsControls;

	private GameObject[] _textFieldsControls;
	private TextMeshProUGUI[] _textsComponentsControls;

	private GameObject _sliderMouseSensitivityX;
	private Slider _sliderComponentMouseSensitivityX;
	private float _currentValueMouseSensitivityX;
	private const float _MIN_VALUE_MOUSE_SENSITIVITY_X = 0.1f;
	private const float _MAX_VALUE_MOUSE_SENSITIVITY_X = 2.5f;
	private GameObject _textNumberSliderMouseSensitivityX;
	private TextMeshProUGUI _textComponentNumberSliderMouseSensitivityX;
	private GameObject _textSliderMouseSensitivityX;
	private TextMeshProUGUI _textComponentSliderMouseSensitivityX;

	private GameObject _sliderMouseSensitivityY;
	private Slider _sliderComponentMouseSensitivityY;
	private float _currentValueMouseSensitivityY;
	private const float _MIN_VALUE_MOUSE_SENSITIVITY_Y = 0.1f;
	private const float _MAX_VALUE_MOUSE_SENSITIVITY_Y = 2.5f;
	private GameObject _textNumberSliderMouseSensitivityY;
	private TextMeshProUGUI _textComponentNumberSliderMouseSensitivityY;
	private GameObject _textSliderMouseSensitivityY;
	private TextMeshProUGUI _textComponentSliderMouseSensitivityY;

	public delegate void MouseSensitivityHandle(float newMouseSensitivity);
	public event MouseSensitivityHandle OnMouseSensitivityXchanged;
	public event MouseSensitivityHandle OnMouseSensitivityYchanged;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsControlsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsControlsData;

	private const float _STEP_VALUE_MOUSE_SENSITIVITY = 0.1f;
	private char _lastValidChar;

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
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionControls viewModelPauseSubMenuSettings)
	{
		_localizationManager = localizationManager;
		_inputDevice = inputDevice;
		_pauseMenuController = pauseMenuController;

		_sliderMouseSensitivityX = viewModelPauseSubMenuSettings.SliderMouseSensitivityX;
		_sliderComponentMouseSensitivityX = viewModelPauseSubMenuSettings.SliderMouseSensitivityX.GetComponent<Slider>();
		_sliderComponentMouseSensitivityX.minValue = _MIN_VALUE_MOUSE_SENSITIVITY_X;
		_sliderComponentMouseSensitivityX.maxValue = _MAX_VALUE_MOUSE_SENSITIVITY_X;
		_sliderComponentMouseSensitivityX.onValueChanged.AddListener(SetMouseSensitivityX);
		_textNumberSliderMouseSensitivityX = viewModelPauseSubMenuSettings.NumberSliderMouseSensitivityX;
		_textComponentNumberSliderMouseSensitivityX = viewModelPauseSubMenuSettings.NumberSliderMouseSensitivityX.GetComponent<TextMeshProUGUI>();
		_textSliderMouseSensitivityX = viewModelPauseSubMenuSettings.TextSliderMouseSensitivityX;
		_textComponentSliderMouseSensitivityX = viewModelPauseSubMenuSettings.TextSliderMouseSensitivityX.GetComponent<TextMeshProUGUI>();

		_sliderMouseSensitivityY = viewModelPauseSubMenuSettings.SliderMouseSensitivityY;
		_sliderComponentMouseSensitivityY = viewModelPauseSubMenuSettings.SliderMouseSensitivityY.GetComponent<Slider>();
		_sliderComponentMouseSensitivityY.minValue = _MIN_VALUE_MOUSE_SENSITIVITY_Y;
		_sliderComponentMouseSensitivityY.maxValue = _MAX_VALUE_MOUSE_SENSITIVITY_Y;
		_sliderComponentMouseSensitivityY.onValueChanged.AddListener(SetMouseSensitivityY);
		_textNumberSliderMouseSensitivityY = viewModelPauseSubMenuSettings.NumberSliderMouseSensitivityY;
		_textComponentNumberSliderMouseSensitivityY = viewModelPauseSubMenuSettings.NumberSliderMouseSensitivityY.GetComponent<TextMeshProUGUI>();
		_textSliderMouseSensitivityY = viewModelPauseSubMenuSettings.TextSliderMouseSensitivityY;
		_textComponentSliderMouseSensitivityY = viewModelPauseSubMenuSettings.TextSliderMouseSensitivityY.GetComponent<TextMeshProUGUI>();

		var bindings = _inputDevice.GetCurrentKeyBindings().ToList();
		_inputFieldsControls = viewModelPauseSubMenuSettings.InputFieldsControls;
		_inputFieldsComponentsControls = new TMP_InputField[viewModelPauseSubMenuSettings.InputFieldsControls.Length];
		for (int i = 0; i < viewModelPauseSubMenuSettings.InputFieldsControls.Length; i++)
		{
			_inputFieldsComponentsControls[i] = viewModelPauseSubMenuSettings.InputFieldsControls[i].GetComponent<TMP_InputField>();
		}
		foreach (var field in _inputFieldsComponentsControls)
		{
			var matchingBinding = bindings.FirstOrDefault(b => b.action == field.name.Replace("InputFieldControl", ""));
			if (matchingBinding != default)
			{
				field.text = matchingBinding.key.ToString();
			}
		}
		foreach (var field in _inputFieldsComponentsControls)
		{
			field.onValidateInput += ValidateAndConvertInput;
			field.onEndEdit.AddListener((string text) =>
			{
				string actionName = field.name.Replace("InputFieldControl", "");
				HandleRebinding(actionName, text);
			});
			field.onValueChanged.AddListener((string text) => KeepLastCharacter(field));
		}
		_textFieldsControls = viewModelPauseSubMenuSettings.InputFieldsControls;
		_textsComponentsControls = new TextMeshProUGUI[viewModelPauseSubMenuSettings.InputFieldsControls.Length];
		for (int i = 0; i < viewModelPauseSubMenuSettings.InputFieldsControls.Length; i++)
		{
			_textsComponentsControls[i] = viewModelPauseSubMenuSettings.TextControls[i].GetComponent<TextMeshProUGUI>();
		}

		SetMouseSensitivityX(1);
		SetMouseSensitivityY(1);

		_sliderComponentMouseSensitivityX.value = 1;
		_sliderComponentMouseSensitivityY.value = 1;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionControlsController Initialized");
	}

	private void DisableButtons()
	{
		foreach (var field in _inputFieldsComponentsControls)
		{
			if (field != null) field.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var field in _inputFieldsComponentsControls)
		{
			if (field != null) field.interactable = true;
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
		foreach (var field in _inputFieldsComponentsControls)
		{
			if (field.name.StartsWith(actionName))
			{
				field.text = key.ToString();
				break;
			}
		}
	}

	private void KeepLastCharacter(TMP_InputField field)
	{
		if (!string.IsNullOrEmpty(field.text))
		{
			field.text = field.text[field.text.Length - 1].ToString();
		}
	}

	public void SetMouseSensitivityX(float newMouseSensitivityX)
	{
		float roundedValue = Mathf.Round(newMouseSensitivityX / _STEP_VALUE_MOUSE_SENSITIVITY) * _STEP_VALUE_MOUSE_SENSITIVITY;
		_currentValueMouseSensitivityX = roundedValue;

		_textComponentNumberSliderMouseSensitivityX.text = _currentValueMouseSensitivityX.ToString("G0");

		OnMouseSensitivityXchanged?.Invoke(_currentValueMouseSensitivityX);
	}

	public void SetMouseSensitivityY(float newMouseSensitivityY)
	{
		float roundedValue = Mathf.Round(newMouseSensitivityY / _STEP_VALUE_MOUSE_SENSITIVITY) * _STEP_VALUE_MOUSE_SENSITIVITY;
		_currentValueMouseSensitivityY = roundedValue;

		_textComponentNumberSliderMouseSensitivityY.text = _currentValueMouseSensitivityY.ToString("G0");

		OnMouseSensitivityYchanged?.Invoke(_currentValueMouseSensitivityY);
	}

	public void SaveSettingsControls()
	{
		var currentData = new PlayerPrefsData();

		currentData.MouseSensitivityX = _currentValueMouseSensitivityX;
		currentData.MouseSensitivityY = _currentValueMouseSensitivityY;
		currentData.KeyBindings = new Dictionary<string, KeyCode>(_inputDevice.CurrentKeyboardKeyBindings);

		OnSaveSettingsControlsData?.Invoke(currentData);
	}

	public void ResetSettingsControls()
	{
		OnResetSettingsControlsData?.Invoke();

		var defaultBindingsSnapshot = _inputDevice.GetDefaultKeyBindings();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			MouseSensitivityX = 1,
			MouseSensitivityY = 1,
			KeyBindings = defaultBindingsSnapshot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		OnSaveSettingsControlsData?.Invoke(defaultData);

		SetMouseSensitivityX(1);
		_sliderComponentMouseSensitivityX.value = 1;
		_textComponentNumberSliderMouseSensitivityX.text = 1.ToString();
		OnMouseSensitivityXchanged(1);

		SetMouseSensitivityY(1);
		_sliderComponentMouseSensitivityY.value = 1;
		_textComponentNumberSliderMouseSensitivityY.text = 1.ToString();
		OnMouseSensitivityYchanged(1);

		foreach (var field in _inputFieldsComponentsControls)
		{
			string actionName = field.name.Replace("InputFieldControl", "");

			if (defaultData.KeyBindings.TryGetValue(actionName, out var key))
			{
				field.text = key.ToString();
				_inputDevice.RebindKey(actionName, key);
			}
		}
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetMouseSensitivityX(data.MouseSensitivityX);
		_sliderComponentMouseSensitivityX.value = data.MouseSensitivityX;
		_textComponentNumberSliderMouseSensitivityX.text = data.MouseSensitivityX.ToString();
		OnMouseSensitivityXchanged(data.MouseSensitivityX);

		SetMouseSensitivityY(data.MouseSensitivityY);
		_sliderComponentMouseSensitivityY.value = data.MouseSensitivityY;
		_textComponentNumberSliderMouseSensitivityY.text = data.MouseSensitivityY.ToString();
		OnMouseSensitivityXchanged(data.MouseSensitivityY);

		if (data.KeyBindings != null && data.KeyBindings.Count > 0)
		{
			foreach (var kvp in data.KeyBindings)
			{
				string actionName = kvp.Key;
				KeyCode savedKey = kvp.Value;

				foreach (var field in _inputFieldsComponentsControls)
				{
					string fieldActionName = field.name.Replace("InputFieldControl", "");

					if (fieldActionName == actionName)
					{
						field.text = savedKey.ToString();
						_inputDevice.RebindKey(actionName, savedKey);
						break;
					}
				}
			}
		}
	}

	private void ChangeLanguage(LocalizationManager	localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentSliderMouseSensitivityX.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextSliderMouseSensitivityX");
		_textComponentSliderMouseSensitivityY.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextSliderMouseSensitivityY");

		_textsComponentsControls[0].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsMoveForward");
		_textsComponentsControls[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsMoveBackward");
		_textsComponentsControls[2].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsMoveRight");
		_textsComponentsControls[3].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsMoveLeft");
		_textsComponentsControls[4].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsRun");
		_textsComponentsControls[5].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsJump");
		_textsComponentsControls[6].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsCrouch");
		_textsComponentsControls[7].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsInteract");
		_textsComponentsControls[8].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsChangeCameraView");
		_textsComponentsControls[9].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsChangeCameraShoulder");
		_textsComponentsControls[10].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsWeaponWheelRightHand");
		_textsComponentsControls[11].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsWeaponWheelLeftHand");
		_textsComponentsControls[12].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsWeaponAttackRightHand");
		_textsComponentsControls[13].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsWeaponAttackLeftHand");
		_textsComponentsControls[14].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsWeaponReload");
		_textsComponentsControls[15].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionControls_TextControlsLegKick");
	}
}