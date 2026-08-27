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

	private const float _STEP_VALUE_MOUSE_SENSITIVITY = 0.1f;
	private char _lastValidChar;

	private GameObject _scrollbar;
	private Scrollbar _scrollbarComponent;
	private float _scrollbarHandleSize = 0.195f;

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionControls viewModelPauseSubMenuSettings)
	{
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;
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

		_scrollbar = viewModelPauseSubMenuSettings.Scrollbar;
		_scrollbarComponent = viewModelPauseSubMenuSettings.Scrollbar.GetComponent<Scrollbar>();
		Canvas.willRenderCanvases += EnforceFixedHandleSize;

		var bindings = _inputDevice.GetCurrentKeyBindings().ToList();
		_inputFieldsControls = viewModelPauseSubMenuSettings.InputFieldsControls;
		_inputFieldsComponentsControls = new TMP_InputField[viewModelPauseSubMenuSettings.InputFieldsControls.Length];
		for (int i = 0; i < viewModelPauseSubMenuSettings.InputFieldsControls.Length; i++)
		{
			_inputFieldsComponentsControls[i] = viewModelPauseSubMenuSettings.InputFieldsControls[i].GetComponent<TMP_InputField>();
		}
		// ... первый цикл (загрузка текста из _inputDevice) остается прежним ...
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

		// ... первый цикл загрузки данных остается прежним ...

		foreach (var field in _inputFieldsComponentsControls)
		{
			// Очищаем всё перед новой подпиской
			field.onValidateInput -= ValidateAndConvertInput;
			field.onSelect.RemoveAllListeners();
			field.onDeselect.RemoveAllListeners();

			// Валидация символов во время набора (остается)
			field.onValidateInput += ValidateAndConvertInput;

			string actionName = field.name.Replace(PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString(), "");
			if (System.Enum.TryParse(typeof(InputControlsEnum), actionName, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				// Запуск физического захвата по клику
				field.onSelect.AddListener((string text) =>
					StartCoroutine(WaitForKeyPress(field, actionEnum)));

				// Проверка ТОЛЬКО ПРИ ПОТЕРЕ ФОКУСА
				field.onDeselect.AddListener((string finalText) =>
					OnInputFieldFocusLost(field, actionEnum, finalText));
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
		_playerPrefsSettingsController.OnApplySettingsSectionControlsPlayerPrefs += ApplySystemLoadedSettings;

		Debug.Log("SettingsSectionControlsController Initialized");
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
	}

	char ValidateAndConvertInput(string text, int charIndex, char addedChar)
	{
		// Блокируем любые управляющие символы (Backspace, Delete и т.д.), чтобы они не очищали поле
		if (char.IsControl(addedChar))
		{
			return '\0';
		}

		string upperCaseStr = char.ToUpperInvariant(addedChar).ToString();

		// Проверяем, входит ли введенный символ в наш разрешенный список
		if (Enum.TryParse<InputAllowedKeys>(upperCaseStr, true, out _))
		{
			_lastValidChar = addedChar;
			return addedChar;
		}

		// Если символа нет в InputAllowedKeys (например, F1, ~, [ ), блокируем его ввод
		Debug.LogWarning($"Символ {upperCaseStr} запрещен к ручному вводу.");
		return '\0';
	}

	void HandleRebinding(InputControlsEnum actionName, string newKeyStr)
	{
		Debug.Log($"[HandleRebinding] Attempting to bind Action: {actionName} to Key String: '{newKeyStr}'");

		if (string.IsNullOrWhiteSpace(newKeyStr))
		{
			return;
		}

		string cleanString = newKeyStr.Trim();
		KeyCode parsedUnityKey;

		// ВАЖНО: Сначала проверяем наш кастомный enum, а затем СТАНДАРТНЫЙ KeyCode
		if (Enum.TryParse<InputAllowedKeys>(cleanString, true, out var customKey))
		{
			if (!Enum.TryParse<KeyCode>(customKey.ToString(), out parsedUnityKey))
			{
				Debug.LogError($"CRITICAL MISMATCH: {customKey} has no Unity mapping.");
				return;
			}
		}
		else if (Enum.TryParse<KeyCode>(cleanString, out parsedUnityKey))
		{
			// === ДУБЛИРУЮЩАЯ ПРОВЕРКА ===
			// Если парсер Unity смог распознать строку (например, "F1"), но её нет в нашем списке - отменяем действие
			if (!Enum.IsDefined(typeof(InputAllowedKeys), parsedUnityKey.ToString()))
			{
				Debug.LogWarning($"[HandleRebinding] KeyCode '{parsedUnityKey}' is forbidden by InputAllowedKeys policy.");
				return;
			}
		}
		else
		{
			Debug.LogWarning($"Unknown key string: '{newKeyStr}'. Binding cancelled.");
			return;
		}

		// ... остальной код свапа биндов остается без изменений ...
		var currentBindings = _inputDevice.GetCurrentKeyBindings().ToDictionary(kvp => kvp.action, kvp => kvp.key);
		InputControlsEnum? conflictingAction = currentBindings.Where(kvp => kvp.Key != actionName && kvp.Value == parsedUnityKey).Select(kvp => (InputControlsEnum?)kvp.Key).FirstOrDefault();

		_inputDevice.RebindKey(actionName, parsedUnityKey);
		UpdateInputFieldText(actionName, parsedUnityKey);

		if (conflictingAction.HasValue)
		{
			KeyCode oldKeyOfThisAction = currentBindings[actionName];
			_inputDevice.RebindKey(conflictingAction.Value, oldKeyOfThisAction);
			UpdateInputFieldText(conflictingAction.Value, oldKeyOfThisAction);
		}

		var finalBindingsSnapshot = _inputDevice.GetCurrentKeyBindings().ToDictionary(kvp => kvp.action, kvp => kvp.key);
		string debugLog = $"[HandleRebinding] FINAL BINDINGS DUMP AFTER SWAP:\n";
		foreach (var binding in finalBindingsSnapshot.OrderBy(b => b.Key)) { debugLog += $"   {binding.Key} : {binding.Value}\n"; }
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

		_playerPrefsSettingsController.SaveSettingsControls(currentData);
	}

	public void ResetSettingsControls()
	{
		_playerPrefsSettingsController.ResetSettingsControls();

		var defaultBindingsSnapshot = _inputDevice.GetDefaultKeyBindings();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			MouseSensitivityX = 1,
			MouseSensitivityY = 1,
			KeyBindings = defaultBindingsSnapshot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		_playerPrefsSettingsController.SaveSettingsControls(defaultData);

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
		bool wasFocused = true;

		targetField.readOnly = true;
		targetField.text = "...";

		yield return null;

		float timer = 0f;
		float delay = 0.2f;

		while (timer < delay)
		{
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
			if (!targetField.isFocused)
			{
				CancelRebinding(targetField, originalText);
				yield break;
			}

			foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(kcode))
				{
					// === НОВАЯ ПРОВЕРКА ===
					// Игнорируем всё, чего нет в нашем "белом списке" InputAllowedKeys
					if (!Enum.IsDefined(typeof(InputAllowedKeys), kcode.ToString()))
					{
						Debug.Log($"[Rebind] Key '{kcode}' is not in the allowed list. Ignoring.");
						continue; // Просто ждем следующую клавишу, не выходя из режима записи
					}

					// Системные клавиши отмены/подтверждения
					if (kcode == KeyCode.Escape || kcode == KeyCode.Return || kcode == KeyCode.Tab)
					{
						if (kcode == KeyCode.Escape)
						{
							CancelRebinding(targetField, originalText);
						}
						continue;
					}

					// Блокировка ЛКМ для действий вне белого списка мыши
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

	/*
	private void OnInputFieldEditingFinished(TMP_InputField field, InputControlsEnum action, string enteredText)
	{
		Debug.Log($"[UI] Editing finished for {action}. Entered: '{enteredText}'");

		// Если строка пустая (например, игрок нажал Enter сразу после клика)
		if (string.IsNullOrWhiteSpace(enteredText))
		{
			CancelRebinding(field, _inputDevice.GetCurrentKeyBindings()
				.FirstOrDefault(b => b.action == action).key.ToString());
			return;
		}

		KeyCode parsedUnityKey;
		bool isValid = false;

		// Проверяем ровно так же, как в HandleRebinding
		if (Enum.TryParse<InputAllowedKeys>(enteredText.Trim(), true, out var customKey))
		{
			if (Enum.TryParse<KeyCode>(customKey.ToString(), out parsedUnityKey))
			{
				isValid = true;
			}
		}
		else if (Enum.TryParse<KeyCode>(enteredText.Trim(), out parsedUnityKey))
		{
			if (Enum.IsDefined(typeof(InputAllowedKeys), parsedUnityKey.ToString()))
			{
				isValid = true;
			}
		}

		if (isValid)
		{
			// Если ключ разрешен — применяем бинд программно
			HandleRebinding(action, parsedUnityKey.ToString());
		}
		else
		{
			// Если ключ запрещен — возвращаем старое значение из устройства
			var currentBinding = _inputDevice.GetCurrentKeyBindings()
				.FirstOrDefault(b => b.action == action);

			CancelRebinding(field, currentBinding.key.ToString());
		}

		// Принудительно снимаем выделение с поля, чтобы "выйти" из него
		EventSystem.current.SetSelectedGameObject(null);
	}
	*/

	// Вспомогательные методы для чистоты кода:
	private void CancelRebinding(TMP_InputField field, string fallbackText)
	{
		field.readOnly = false;
		field.text = fallbackText;
		// Selection здесь устанавливать не обязательно, так как это сделает onEndEdit -> SetSelectedGameObject(null)
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

	private void OnInputFieldFocusLost(TMP_InputField field, InputControlsEnum action, string enteredTextFromEvent)
	{
		Debug.Log($"[UI] Focus lost for {action}. Checking value...");

		// Берем актуальный текст из компонента
		string currentText = field.text.Trim();

		KeyCode parsedUnityKey;
		bool isValid = false;

		if (string.IsNullOrEmpty(currentText))
		{
			var binding = _inputDevice.GetCurrentKeyBindings().FirstOrDefault(b => b.action == action);
			CancelRebinding(field, binding.key.ToString());

			// Снимаем фокус программно
			EventSystem.current.SetSelectedGameObject(null);
			return;
		}

		// Проверяем ровно так же, как в HandleRebinding
		if (Enum.TryParse<InputAllowedKeys>(currentText, true, out var customKey))
		{
			if (Enum.TryParse<KeyCode>(customKey.ToString(), out parsedUnityKey))
			{
				isValid = true;
			}
		}
		else if (Enum.TryParse<KeyCode>(currentText, out parsedUnityKey))
		{
			if (Enum.IsDefined(typeof(InputAllowedKeys), parsedUnityKey.ToString()))
			{
				isValid = true;
			}
		}

		if (isValid)
		{
			// Если ключ разрешен — применяем бинд 
			HandleRebinding(action, parsedUnityKey.ToString());
		}
		else
		{
			// === КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ ===
			// Если введен мусор (F13, [, ;;) - откатываем к тому, что было ДО редактирования
			var oldBinding = _inputDevice.GetCurrentKeyBindings().FirstOrDefault(b => b.action == action);

			// Возвращаем старый текст
			CancelRebinding(field, oldBinding.key.ToString());

			// Принудительно снимаем выделение с ЭТОГО поля
			// Это заставит UI выйти из режима набора текста
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
	private void ChangeLanguage(LocalizationManager	localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentSliderMouseSensitivityX.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextSliderMouseSensitivityX");
		_textComponentSliderMouseSensitivityY.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextSliderMouseSensitivityY");

		_textsComponentsControls[0].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsMoveForward");
		_textsComponentsControls[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsMoveBackward");
		_textsComponentsControls[2].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsMoveRight");
		_textsComponentsControls[3].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsMoveLeft");
		_textsComponentsControls[4].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsRun");
		_textsComponentsControls[5].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsJump");
		_textsComponentsControls[6].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsCrouch");
		_textsComponentsControls[7].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsInteract");
		_textsComponentsControls[8].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsChangeCameraView");
		_textsComponentsControls[9].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsChangeCameraShoulder");
		_textsComponentsControls[10].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsWeaponWheelRightHand");
		_textsComponentsControls[11].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsWeaponWheelLeftHand");
		_textsComponentsControls[12].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsWeaponAttackRightHand");
		_textsComponentsControls[13].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsWeaponAttackLeftHand");
		_textsComponentsControls[14].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsWeaponReload");
		_textsComponentsControls[15].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionControls_TextControlsLegKick");
	}
}