using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class SettingsSubMenuController : MonoBehaviour
{
	// Ссылка на контроллер паузы меню
	PauseMenuController pauseMenuController;

	// Интерфейсы настроек
	public Canvas SettingsSubMenuCanvas;
	public Button CloseSettingsSubMenuButton;

	// Ползунок для регулировки FOV
	public Slider fovSlider;

	// Тексты для отображения значения FOV
	public TextMeshProUGUI fovDisplayText;

	// Основные камеры
	public Camera MainCamera;
	public Camera AdditionalCamera;

	// Кнопки выбора частоты кадров
	public Button LimitFPS_30_Button;
	public Button LimitFPS_60_Button;
	public Button LimitFPS_90_Button;
	public Button LimitFPS_144_Button;

	// Цвет выделения активных кнопок
	public Color activeColor = Color.green;
	public Color normalColor = Color.white;

	// Текущее выбранное ограничение FPS
	int currentFrameRateLimit = 60;

	// Диапазон возможных значений FOV
	private const float MIN_FOV_VALUE = 60f;
	private const float MAX_FOV_VALUE = 120f;

	// Список полей ввода для изменения клавиш
	[SerializeField]
	public TMP_InputField[] inputFields;

	private readonly char[][] layoutMap = new char[][]
	{
		new char[] {'Й', 'Q'}, new char[] {'Ц', 'W'}, new char[] {'У', 'E'}, new char[] {'К', 'R'},
		new char[] {'Е', 'T'}, new char[] {'Н', 'Y'}, new char[] {'Г', 'U'}, new char[] {'Ш', 'I'},
		new char[] {'Щ', 'O'}, new char[] {'З', 'P'}, new char[] {'Х', '['}, new char[] {'Ъ', ']'},
		new char[] {'Ф', 'A'}, new char[] {'Ы', 'S'}, new char[] {'В', 'D'}, new char[] {'А', 'F'},
		new char[] {'П', 'G'}, new char[] {'Р', 'H'}, new char[] {'О', 'J'}, new char[] {'Л', 'K'},
		new char[] {'Д', 'L'}, new char[] {'Ж', ';'}, new char[] {'Э', '\''}, new char[] {'Я', 'Z'},
		new char[] {'Ч', 'X'}, new char[] {'С', 'C'}, new char[] {'М', 'V'}, new char[] {'И', 'B'},
		new char[] {'Т', 'N'}, new char[] {'Ь', 'M'}, new char[] {'Б', ','}, new char[] {'Ю', '.'},
		new char[] {'.', '/'}, // Точка соответствует слэшу '/'
    };

	void Start()
	{
		pauseMenuController = GetComponent<PauseMenuController>();
		CloseSettingsSubMenuButton.onClick.AddListener(CloseSettingsSubMenu);

		// Настройки регулировки FOV
		if (fovSlider != null)
		{
			fovSlider.minValue = MIN_FOV_VALUE;
			fovSlider.maxValue = MAX_FOV_VALUE;
			fovSlider.onValueChanged.AddListener(OnFovChanged);
			SetFOV(MIN_FOV_VALUE);
		}

		// Настройка кнопок для изменения лимитов FPS
		LimitFPS_30_Button.onClick.AddListener(() => ChangeFrameRateLimit(30));
		LimitFPS_60_Button.onClick.AddListener(() => ChangeFrameRateLimit(60));
		LimitFPS_90_Button.onClick.AddListener(() => ChangeFrameRateLimit(90));
		LimitFPS_144_Button.onClick.AddListener(() => ChangeFrameRateLimit(144));

		// Изначально выделяем кнопку 60 FPS
		ChangeFrameRateLimit(60);
		ApplyButtonColors(currentFrameRateLimit);

		// Загружаем текущие значения клавиш из InputManager и добавляем слушатели событий
		foreach (var (action, key) in InputManager.Instance.GetCurrentBindings())
		{
			var field = inputFields.FirstOrDefault(f => f.name.Equals(action + "InputField")) as TMP_InputField;
			if (field != null)
				field.text = key.ToString();
		}

		// Добавляем обработчики
		foreach (var field in inputFields)
		{
			field.onValidateInput += ValidateAndConvertInput;
			field.onEndEdit.AddListener((string text) =>
			{
				string actionName = field.name.Replace("InputField", ""); // Простая замена "InputField"
				HandleRebinding(actionName, text);
			});
		}
	}

	char ValidateAndConvertInput(string text, int charIndex, char addedChar)
	{
		if (char.IsControl(addedChar)) return addedChar;

		char upperCaseChar = char.ToUpperInvariant(addedChar);
		Debug.Log($"Преобразуется символ: {upperCaseChar}");

		foreach (var entry in layoutMap)
		{
			if (entry[0] == upperCaseChar)
			{
				Debug.Log($"Символ найден: {upperCaseChar} -> {entry[1]}");
				return entry[1];
			}
		}

		Debug.LogWarning($"Символ {upperCaseChar} не обнаружен в раскладке!");
		return upperCaseChar;
	}

	private void Update()
	{
		if (InputManager.Instance.GetKeyPauseMenu() && SettingsSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseSettingsSubMenu();
		}
	}

	public void CloseSettingsSubMenu()
	{
		SettingsSubMenuCanvas.gameObject.SetActive(false);
		pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SettingsSubMenu closed");
	}

	public void OnFovChanged(float value)
	{
		SetFOV(value);
	}

	private void SetFOV(float newFov)
	{
		if (MainCamera != null)
			MainCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);
		if (AdditionalCamera != null)
			AdditionalCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);

		if (fovDisplayText != null)
			fovDisplayText.text = ((int)newFov).ToString();
	}

	private void ChangeFrameRateLimit(int frameRate)
	{
		Application.targetFrameRate = frameRate;
		currentFrameRateLimit = frameRate;
		ApplyButtonColors(frameRate);
		Debug.Log($"Frame rate limit set to {frameRate}");
	}

	private void ApplyButtonColors(int activeFrameRate)
	{
		ResetAllButtons();

		switch (activeFrameRate)
		{
			case 30:
				HighlightButton(LimitFPS_30_Button);
				break;
			case 60:
				HighlightButton(LimitFPS_60_Button);
				break;
			case 90:
				HighlightButton(LimitFPS_90_Button);
				break;
			case 144:
				HighlightButton(LimitFPS_144_Button);
				break;
		}
	}

	private void ResetAllButtons()
	{
		LimitFPS_30_Button.image.color = normalColor;
		LimitFPS_60_Button.image.color = normalColor;
		LimitFPS_90_Button.image.color = normalColor;
		LimitFPS_144_Button.image.color = normalColor;
	}

	private void HighlightButton(Button button)
	{
		button.image.color = activeColor;
	}

	// Обработчик изменения клавиш
	void HandleRebinding(string actionName, string newKeyStr)
	{
		KeyCode newKey;
		if (Enum.TryParse<KeyCode>(newKeyStr, out newKey))
		{
			InputManager.Instance.RebindKey(actionName, newKey);
		}
		else
		{
			Debug.LogWarning($"Некорректная клавиша: {newKeyStr}. Введите допустимое обозначение клавиши.");
		}
	}
}