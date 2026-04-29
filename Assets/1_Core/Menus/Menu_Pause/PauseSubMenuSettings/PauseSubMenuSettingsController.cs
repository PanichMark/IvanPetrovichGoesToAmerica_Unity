using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class PauseSubMenuSettingsController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private bool isPauseSubMenuSettingsOpened;
	private GameObject canvasPauseSubMenuSettings;
	private PauseMenuController pauseMenuController;
	// Конструктор принимает зависимость
	private GameObject buttonClosePauseSubMenuSettings;
	//private GameObject[] FPSbuttons;
	private GameObject FOVSlider;
	private Button[] buttonsChangeLanguage;
	private char lastValidChar; // Переменная для хранения последнего корректного символа

	// Интерфейсы настроек

	

	// Ползунок для регулировки FOV
	private Slider fovSlider;

	// Тексты для отображения значения FOV
	private TextMeshProUGUI fovDisplayText;

	// Основные камеры
	private Camera MainCamera;
	

	// Кнопки выбора частоты кадров
	private Button[] FPSbuttons;


	// Цвет выделения активных кнопок
	private Color activeColor = Color.green;
	private Color normalColor = Color.white;

	private GameObject buttonSaveSettings;
	private GameObject buttonResetSettings;

	// Текущее выбранное ограничение FPS
	private int currentFrameRateLimit = 60;

	// Диапазон возможных значений FOV
	private const float MIN_FOV_VALUE = 60f;
	private const float MAX_FOV_VALUE = 120f;
	private Bootstrap bootstrap;
	// Список полей ввода для изменения клавиш
	private GameController gameController;
	private TMP_InputField[] KeyRebinds;

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

	public void Initialize(IInputDevice inputDevice, Bootstrap bootstrap, GameController gameController, GameObject mainCamera, GameObject fovDisplayText, MenuManager menuManager, PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSettings, GameObject buttonClosePauseSubMenuSettings, GameObject FOVSlider, GameObject[] FPSbuttons, GameObject[] buttonsChangeLanguage, GameObject[] KeyRebinds, GameObject buttonSaveSettings, GameObject buttonResetSettings)

	{
		this.gameController = gameController;
		this.bootstrap = bootstrap;
		//Debug.Log(mainCamera);
		this.MainCamera = mainCamera.GetComponent<Camera>();
		this.fovDisplayText = fovDisplayText.GetComponent<TextMeshProUGUI>();

		this.FOVSlider = FOVSlider;
		//this.FPSbuttons = FPSbuttons;
		// Инициализируй массив LimitFPS_Button
		this.FPSbuttons = new Button[FPSbuttons.Length];
		this.buttonsChangeLanguage = new Button[buttonsChangeLanguage.Length];
		this.KeyRebinds = new TMP_InputField[KeyRebinds.Length];
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		this.buttonClosePauseSubMenuSettings = buttonClosePauseSubMenuSettings;
		this.pauseMenuController.OnOpenSettingsSubMenu += ShowSettingsSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideSettingsSubMenuCanvas;



		this.buttonClosePauseSubMenuSettings.GetComponent<Button>().onClick.AddListener(() => this.pauseMenuController.ClosePauseSubMenu());
		//this.buttonSaveSettings.GetComponent<Button>().onClick.AddListener(() => SaveSettings());
		//this.buttonSaveSettings.GetComponent<Button>().onClick.AddListener(() => ResetSettings());

		fovSlider = this.FOVSlider.GetComponent<Slider>();



		
		this.fovSlider.minValue = MIN_FOV_VALUE;
		this.fovSlider.maxValue = MAX_FOV_VALUE;
		this.fovSlider.onValueChanged.AddListener(OnFovChanged);
		SetFOV(MIN_FOV_VALUE);

		//Debug.Log(this.FPSbuttons[0]);

		for (int i = 0; i < this.FPSbuttons.Length; i++)
		{
			this.FPSbuttons[i] = FPSbuttons[i].GetComponent<Button>();
		}

		for (int i = 0; i < this.buttonsChangeLanguage.Length; i++)
		{
			this.buttonsChangeLanguage[i] = buttonsChangeLanguage[i].GetComponent<Button>();
		}

		for (int i = 0; i < this.KeyRebinds.Length; i++)
		{
			this.KeyRebinds[i] = KeyRebinds[i].GetComponent<TMP_InputField>();
		}

		this.FPSbuttons[0].onClick.AddListener(() => ChangeFrameRateLimit(30));
		this.FPSbuttons[1].onClick.AddListener(() => ChangeFrameRateLimit(60));
		this.FPSbuttons[2].onClick.AddListener(() => ChangeFrameRateLimit(90));
		this.FPSbuttons[3].onClick.AddListener(() => ChangeFrameRateLimit(144));

		//Debug.Log(this.buttonsChangeLanguage[0]);

		this.buttonsChangeLanguage[0].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.Russian));
		this.buttonsChangeLanguage[1].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.English));

		// Выделяем активную кнопку FPS
		ChangeFrameRateLimit(60);
		ApplyButtonColors(currentFrameRateLimit);
		this.gameController.OnOpenMainMenu += () => SetFOV(60);
		
		// Заполняем поля ввода клавиш начальными значениями из InputManager
		var bindings = this.inputDevice.GetCurrentBindings().ToList();

		foreach (var field in this.KeyRebinds)
		{
			var matchingBinding = bindings.FirstOrDefault(b => b.action == field.name.Replace("InputField", ""));
			if (matchingBinding != default)
			{
				field.text = matchingBinding.key.ToString();
			}
		}

		// Добавляем обработчики
		foreach (var field in this.KeyRebinds)
		{
			field.onValidateInput += ValidateAndConvertInput;
			field.onEndEdit.AddListener((string text) =>
			{
				string actionName = field.name.Replace("InputField", "");
				HandleRebinding(actionName, text);
			});
			field.onValueChanged.AddListener((string text) => KeepLastCharacter(field));
		}
		
		Debug.Log("SettingsSubMenu Initialized");
	}

	// Ссылка на контроллер паузы меню



	private void ChangeLanguage(LanguagesEnum language)
	{
		bootstrap.ChangeLanguage(language);
		Debug.Log("Changed Language to: " + language);
	}
	

	private void KeepLastCharacter(TMP_InputField field)
	{
		if (!string.IsNullOrEmpty(field.text))
		{
			field.text = field.text[field.text.Length - 1].ToString(); // Оставляем только последний символ
		}
	}
	char ValidateAndConvertInput(string text, int charIndex, char addedChar)
	{
		if (char.IsControl(addedChar)) return addedChar; // Пропускаем управляющие символы

		char upperCaseChar = char.ToUpperInvariant(addedChar); // Верхний регистр

		// Если символ — английская буква, возвращаем её
		if (char.IsLetter(upperCaseChar) && upperCaseChar <= 'Z')
		{
			lastValidChar = upperCaseChar; // Запоминаем последнюю правильную букву
			return upperCaseChar;
		}

		// Проверяем соответствие русской раскладке
		foreach (var entry in layoutMap)
		{
			if (entry[0] == upperCaseChar)
			{
				lastValidChar = entry[1]; // Запоминаем последнюю правильную букву
				return entry[1];          // Возвращаем английский аналог
			}
		}

		// Если соответствие не найдено, возвращаем последний валидный символ
		Debug.LogWarning($"Символ {upperCaseChar} не обнаружен в раскладке!");
		return lastValidChar; // Восстанавливаем предыдущий корректный символ
	}
	
	public void HideSettingsSubMenuCanvas()
	{
		if (isPauseSubMenuSettingsOpened)
		{
			isPauseSubMenuSettingsOpened = false;
			canvasPauseSubMenuSettings.gameObject.SetActive(false);

			Debug.Log("SettingsSubMenu closed");
		}
	}
	public void ShowSettingsSubMenuCanvas()
	{
		isPauseSubMenuSettingsOpened = true;
		canvasPauseSubMenuSettings.gameObject.SetActive(true);
	}
	public void OnFovChanged(float value)
	{
		SetFOV(value);
	}

	private void SetFOV(float newFov)
	{
	
			MainCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);
		
			//AdditionalCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);

		
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
				HighlightButton(FPSbuttons[0]);
				break;
			case 60:
				HighlightButton(FPSbuttons[1]);
				break;
			case 90:
				HighlightButton(FPSbuttons[2]);
				break;
			case 144:
				HighlightButton(FPSbuttons[3]);
				break;
		}
	}

	private void ResetAllButtons()
	{
		FPSbuttons[0].image.color = normalColor;
		FPSbuttons[1].image.color = normalColor;
		FPSbuttons[2].image.color = normalColor;
		FPSbuttons[3].image.color = normalColor;
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
			inputDevice.RebindKey(actionName, newKey);
		}
		else
		{
			Debug.LogWarning($"Некорректная клавиша: {newKeyStr}. Введите допустимое обозначение клавиши.");
		}
	}

	// --- ДОБАВИТЬ ЭТИ МЕТОДЫ В САМЫЙ КОНЕЦ КЛАССА PauseSubMenuSettingsController ---

	// Метод для кнопки "Сохранить"
	public void SaveSettings()
	{
		// 1. Собираем все текущие настройки из контроллера в объект SettingsData
		var currentData = new SettingsData();
		//currentData.Language = bootstrap.CurrentLanguage;
		currentData.FOV = MainCamera.fieldOfView;
		currentData.FPSLimit = currentFrameRateLimit;

		// Собираем биндинги клавиш
		foreach (var binding in inputDevice.GetCurrentBindings())
		{
			currentData.KeyBindings[binding.action] = binding.key;
		}

		// 2. Передаем объект классу-хранилищу для записи на диск
		PauseSubMenuSettingsPlayerPrefs.SaveSettings(currentData);

		Debug.Log("Настройки успешно сохранены.");
	}

	// Метод для кнопки "Сбросить"
	public void ResetSettings()
	{
		// 1. Удаляем все настройки из PlayerPrefs
		PauseSubMenuSettingsPlayerPrefs.DeleteAllSettings();

		// 2. Сбрасываем настройки в самом контроллере к значениям по умолчанию

		// Язык по умолчанию
		//ChangeLanguage(LanguagesEnum.Russian);

		// FOV по умолчанию
		SetFOV(MIN_FOV_VALUE);
		fovSlider.value = MIN_FOV_VALUE;

		// Лимит FPS по умолчанию
		//ChangeFrameRateLimit(60);

		// Клавиши по умолчанию (сбрасываем через InputKeyboard)
		var defaultBindings = inputDevice.CurrentBindings;
		foreach (var field in KeyRebinds)
		{
			string actionName = field.name.Replace("InputField", "");

			if (defaultBindings.TryGetValue(actionName, out var defaultKey))
			{
				field.text = defaultKey.ToString();
				inputDevice.RebindKey(actionName, defaultKey);
			}
		}

		Debug.Log("Настройки сброшены к значениям по умолчанию.");
	}
}

