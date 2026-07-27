using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
	private const float _MAX_VALUE_MOUSE_SENSITIVITY_X = 3f;
	private GameObject _textNumberSliderMouseSensitivityX;
	private TextMeshProUGUI _textComponentNumberSliderMouseSensitivityX;
	private GameObject _textSliderMouseSensitivityX;
	private TextMeshProUGUI _textComponentSliderMouseSensitivityX;

	private GameObject _sliderMouseSensitivityY;
	private Slider _sliderComponentMouseSensitivityY;
	private float _currentValueMouseSensitivityY;
	private const float _MIN_VALUE_MOUSE_SENSITIVITY_Y = 0.1f;
	private const float _MAX_VALUE_MOUSE_SENSITIVITY_Y = 3f;
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
			string actionName = field.name.Replace(PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString(), "");

			if (System.Enum.TryParse(typeof(InputControlsEnum), actionName, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;
				var matchingBinding = bindings.FirstOrDefault(b => b.action == actionEnum);
				if (matchingBinding != default)
				{
					field.text = matchingBinding.key.ToString();
				}
			}
		}
		foreach (var field in _inputFieldsComponentsControls)
		{
			field.onValidateInput += ValidateAndConvertInput;

			// УДАЛИТЕ старый onEndEdit listener из предыдущего сообщения

			// Вместо него подписываемся на клик по полю
			int indexCopy = Array.IndexOf(_inputFieldsComponentsControls, field); // Сохраняем индекс, чтобы избежать closure trap
			string actionName = field.name.Replace(PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString(), "");

			if (System.Enum.TryParse(typeof(InputControlsEnum), actionName, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				// Используем AddListener без параметров через лямбду-замыкание
				field.onSelect.AddListener((string text) =>
					StartCoroutine(WaitForKeyPress(field, actionEnum)));
			}
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

		Debug.Log("SettingsSectionControlsController Initialized");
	}

	char ValidateAndConvertInput(string text, int charIndex, char addedChar)
	{
		Debug.Log($"[Validate] Raw input: '{addedChar}' (Code: {(int)addedChar})");

		// Пробел — это печатаемый символ, поэтому IsControl его пропускает. Ловим его первым.
		if (addedChar == ' ')
		{
			_lastValidChar = InputKeysSpecialSystem.Space.ToString()[0];
			return _lastValidChar;
		}

		if (char.IsControl(addedChar))
		{
			return '\0';
		}

		char upperCaseChar = char.ToUpperInvariant(addedChar);

		if (char.IsDigit(upperCaseChar))
		{
			_lastValidChar = upperCaseChar;
			return upperCaseChar;
		}

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

		switch (upperCaseChar)
		{
			case ',': _lastValidChar = InputKeysSpecialSystem.Comma.ToString()[0]; break;
			case '.': _lastValidChar = InputKeysSpecialSystem.Period.ToString()[0]; break;
			case '/': _lastValidChar = InputKeysSpecialSystem.Slash.ToString()[0]; break;
			default:
				Debug.LogWarning($"Символ {upperCaseChar} не обнаружен в раскладке!");
				return '\0';
		}

		return _lastValidChar;
	}

	void HandleRebinding(InputControlsEnum actionName, string newKeyStr)
	{
		Debug.Log($"[HandleRebinding] Attempting to bind Action: {actionName} to Key String: '{newKeyStr}'");

		// 1. Защита от пустой строки (часто прилетает от Tab, Escape или просто потери фокуса)
		if (string.IsNullOrWhiteSpace(newKeyStr))
		{
			return;
		}

		string cleanString = newKeyStr.Trim();
		KeyCode parsedUnityKey;

		// 2. Сначала ВСЕГДА проверяем наш специальный enum (Tab, Shift, Ctrl...)
		if (Enum.TryParse<InputKeysSpecialSystem>(cleanString, true, out var customKey))
		{
			Debug.Log($"[HandleRebinding] Parsed as InputKeysSpecialSystem: {customKey}");

			// Пытаемся превратить имя вашего enum (например, "LeftShift") в системный KeyCode
			if (!Enum.TryParse<KeyCode>(customKey.ToString(), out parsedUnityKey))
			{
				Debug.LogError($"[HandleRebinding] CRITICAL: Your InputKeysSpecialSystem.{customKey} does not match any UnityEngine.KeyCode field!");
				return;
			}
		}
		// 3. Затем пробуем стандартные буквы и цифры
		else if (Enum.TryParse<KeyCode>(cleanString, out parsedUnityKey))
		{
			Debug.Log($"[HandleRebinding] Parsed as standard Unity KeyCode: {parsedUnityKey}");
		}
		else
		{
			Debug.LogWarning($"[HandleRebinding] Unknown key string: '{newKeyStr}'. Binding cancelled.");
			return;
		}

		var currentBindings = _inputDevice.GetCurrentKeyBindings()
			.ToDictionary(kvp => kvp.action, kvp => kvp.key);

		InputControlsEnum? conflictingAction = currentBindings
			.Where(kvp => kvp.Key != actionName && kvp.Value == parsedUnityKey)
			.Select(kvp => (InputControlsEnum?)kvp.Key)
			.FirstOrDefault();

		_inputDevice.RebindKey(actionName, parsedUnityKey);
		UpdateInputFieldText(actionName, parsedUnityKey);

		if (conflictingAction.HasValue)
		{
			KeyCode oldKeyOfThisAction = currentBindings[actionName];
			_inputDevice.RebindKey(conflictingAction.Value, oldKeyOfThisAction);
			UpdateInputFieldText(conflictingAction.Value, oldKeyOfThisAction);
		}

		var finalBindingsSnapshot = _inputDevice.GetCurrentKeyBindings()
			.ToDictionary(kvp => kvp.action, kvp => kvp.key);

		string debugLog = $"[HandleRebinding] FINAL BINDINGS DUMP AFTER SWAP:\n";
		foreach (var binding in finalBindingsSnapshot.OrderBy(b => b.Key))
		{
			debugLog += $"   {binding.Key} : {binding.Value}\n";
		}
		Debug.Log(debugLog);
	}

	private void UpdateInputFieldText(InputControlsEnum actionName, KeyCode key)
	{
		foreach (var field in _inputFieldsComponentsControls)
		{
			if (field.name.StartsWith(PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString() + actionName.ToString()))
			{
				// Отображаем имя ключа "как есть" (LeftShift, Space, Tab)
				field.text = key.ToString();
				break;
			}
		}
	}

	private void KeepLastCharacter(TMP_InputField field)
	{
		if (!string.IsNullOrEmpty(field.text))
		{
			if (field.text != "Space")
			{
				field.text = field.text[field.text.Length - 1].ToString();
			}
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
		currentData.KeyBindings = new Dictionary<InputControlsEnum, KeyCode>(_inputDevice.CurrentKeyboardKeyBindings);

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
			string actionNameStr = field.name.Replace(PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString(), "");

			if (System.Enum.TryParse(typeof(InputControlsEnum), actionNameStr, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				if (defaultData.KeyBindings.TryGetValue(actionEnum, out var key))
				{
					field.text = key.ToString();
					_inputDevice.RebindKey(actionEnum, key);
				}
			}
		}
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetMouseSensitivityX(data.MouseSensitivityX);
		_sliderComponentMouseSensitivityX.value = data.MouseSensitivityX;
		_textComponentNumberSliderMouseSensitivityX.text = data.MouseSensitivityX.ToString();
		OnMouseSensitivityXchanged?.Invoke(data.MouseSensitivityX);

		SetMouseSensitivityY(data.MouseSensitivityY);
		_sliderComponentMouseSensitivityY.value = data.MouseSensitivityY;
		_textComponentNumberSliderMouseSensitivityY.text = data.MouseSensitivityY.ToString();
		OnMouseSensitivityYchanged?.Invoke(data.MouseSensitivityY);

		if (data.KeyBindings != null && data.KeyBindings.Count > 0)
		{
			foreach (var kvp in data.KeyBindings)
			{
				InputControlsEnum actionEnum = kvp.Key;
				KeyCode savedKeyFromFile = kvp.Value;

				string fieldActionNameStr = PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString() + actionEnum.ToString();
				var field = _inputFieldsComponentsControls.FirstOrDefault(f => f.name == fieldActionNameStr);
				if (field != null)
				{
					field.text = savedKeyFromFile.ToString();
				}

				_inputDevice.RebindKey(actionEnum, savedKeyFromFile);
			}
		}
	}

	private System.Collections.IEnumerator WaitForKeyPress(TMP_InputField targetField, InputControlsEnum action)
	{
		Debug.Log($"[Rebind] Listening for key press for action: {action}...");

		string originalText = targetField.text;
		bool wasFocused = true; // Флаг отслеживания фокуса

		targetField.readOnly = true;
		targetField.text = "...";

		yield return null;

		float timer = 0f;
		float delay = 0.2f;

		while (timer < delay)
		{
			// Проверяем, не потеряло ли поле фокус во время задержки
			if (!targetField.isFocused && wasFocused)
			{
				CancelRebinding(targetField, originalText);
				yield break;
			}
			wasFocused = targetField.isFocused;

			timer += Time.unscaledDeltaTime;
			yield return null;
		}

		while (true)
		{
			// Постоянно проверяем потерю фокуса во время ожидания нажатия
			if (!targetField.isFocused)
			{
				CancelRebinding(targetField, originalText);
				yield break;
			}

			foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(kcode))
				{
					if (kcode == KeyCode.Escape || kcode == KeyCode.Return || kcode == KeyCode.Tab)
					{
						if (kcode == KeyCode.Escape)
						{
							CancelRebinding(targetField, originalText);
						}
						continue;
					}

					if (kcode == KeyCode.Mouse0 && !IsMouseBindingAllowed(action))
					{
						continue;
					}

					HandleRebinding(action, kcode.ToString());
					targetField.readOnly = false;
					UpdateInputFieldText(action, kcode);
					yield break;
				}
			}

			if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
			{
				CancelRebinding(targetField, originalText);
				yield break;
			}

			yield return null;
		}
	}

	// Вспомогательные методы для чистоты кода:
	private void CancelRebinding(TMP_InputField field, string text)
	{
		field.readOnly = false;
		field.text = text;
	}

	private bool IsPointerOverUI()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
		{
			position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
		};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	private bool IsMouseBindingAllowed(InputControlsEnum action)
	{
		// Здесь перечислите действия, которые МОЖНО назначать на ЛКМ
		switch (action)
		{
			case InputControlsEnum.WeaponAttackRightHand:
			case InputControlsEnum.WeaponAttackLeftHand:
			case InputControlsEnum.LegKick:
				return true;
			default:
				return false;
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