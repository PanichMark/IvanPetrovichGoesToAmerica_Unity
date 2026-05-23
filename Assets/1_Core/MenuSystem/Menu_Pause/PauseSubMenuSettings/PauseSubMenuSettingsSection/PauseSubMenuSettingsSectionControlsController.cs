using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PauseSubMenuSettingsSectionControlsController : MonoBehaviour
{
	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsControlsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsControlsData;

	private IInputDevice _inputDevice;
	private TMP_InputField[] _KeyRebinds;
	private char _lastValidChar;
	private PauseMenuController _pauseMenuController;

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
		PauseMenuController pauseMenuController,
		GameObject[] KeyRebinds)
	{
		_inputDevice = inputDevice;
		_pauseMenuController = pauseMenuController;

		_KeyRebinds = new TMP_InputField[KeyRebinds.Length];

		for (int i = 0; i < _KeyRebinds.Length; i++)
		{
			_KeyRebinds[i] = KeyRebinds[i].GetComponent<TMP_InputField>();
		}

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

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionControls Initialized");
	}

	private void DisableButtons()
	{
		foreach (var field in _KeyRebinds)
		{
			if (field != null) field.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var field in _KeyRebinds)
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
		foreach (var field in _KeyRebinds)
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

	public void SaveSettingsControls()
	{
		var currentData = new PlayerPrefsData();

		currentData.KeyBindings = new Dictionary<string, KeyCode>(_inputDevice.CurrentKeyboardKeyBindings);

		OnSaveSettingsControlsData?.Invoke(currentData);
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

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
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