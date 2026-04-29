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
	private PauseSubMenuSettingsPlayerPrefs pauseSubMenuSettingsPlayerPrefs;
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
		GameObject canvasPauseSubMenuSettings, GameObject buttonClosePauseSubMenuSettings, GameObject FOVSlider, GameObject[] FPSbuttons, GameObject[] buttonsChangeLanguage, GameObject[] KeyRebinds, PauseSubMenuSettingsPlayerPrefs pauseSubMenuSettingsPlayerPrefs, GameObject buttonSaveSettings, GameObject buttonResetSettings)

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
		this.buttonSaveSettings = buttonSaveSettings;
		this.buttonResetSettings = buttonResetSettings;
		this.pauseSubMenuSettingsPlayerPrefs = pauseSubMenuSettingsPlayerPrefs;
		// 1. Загружаем данные из PlayerPrefs
	

		this.buttonClosePauseSubMenuSettings.GetComponent<Button>().onClick.AddListener(() => this.pauseMenuController.ClosePauseSubMenu());
		// Стало:
		this.buttonSaveSettings.GetComponent<Button>().onClick.AddListener(SaveSettings);
		this.buttonResetSettings.GetComponent<Button>().onClick.AddListener(ResetSettings);

		fovSlider = this.FOVSlider.GetComponent<Slider>();



		
		this.fovSlider.minValue = MIN_FOV_VALUE;
		this.fovSlider.maxValue = MAX_FOV_VALUE;
		this.fovSlider.onValueChanged.AddListener(OnFovChanged);
		//SetFOV(MIN_FOV_VALUE);

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

		SettingsData loadedData = pauseSubMenuSettingsPlayerPrefs.LoadSettings();

		// 2. Применяем эти данные к нашему меню
		ApplyLoadedSettings(loadedData);

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
	// В классе PauseSubMenuSettingsController

	public void SaveSettings()
	{
		// 1. Создаем объект для сохранения
		var currentData = new SettingsData();
		currentData.FOV = MainCamera.fieldOfView;
		// currentData.FPSLimit = currentFrameRateLimit; // Если нужно сохранять FPS, добавьте

		// 2. БЕРЕМ ГОТОВЫЙ СЛОВАРЬ ИЗ УСТРОЙСТВА ВВОДА
		// Это главная правка. Мы не собираем данные вручную, а берем их из inputDevice.
		currentData.KeyBindings = new Dictionary<string, KeyCode>(inputDevice.CurrentBindings);

		// --- ОТЛАДКА: Смотрим, что именно мы сохраняем ---
		Debug.Log("=== СОХРАНЯЕМЫЕ ДАННЫЕ ===");
		foreach (var kvp in currentData.KeyBindings)
		{
			Debug.Log($"Сохраняю: {kvp.Key} = {kvp.Value}");
		}
		Debug.Log("=========================");
		// ---------------------------------

		// 3. Передаем объект классу-хранилищу
		pauseSubMenuSettingsPlayerPrefs.SaveSettings(currentData);

		Debug.Log("GameSettings SAVED");
	}

	// Метод для кнопки "Сбросить"
	public void ResetSettings()
	{
		pauseSubMenuSettingsPlayerPrefs.ResetSettings();

		SetFOV(MIN_FOV_VALUE);
		fovSlider.value = MIN_FOV_VALUE;


		var defaultBindings = inputDevice.GetDefaultBindings();
		foreach (var field in KeyRebinds)
		{
			string actionName = field.name.Replace("InputField", "");

			if (defaultBindings.TryGetValue(actionName, out var defaultKey))
			{
				field.text = defaultKey.ToString();
				inputDevice.RebindKey(actionName, defaultKey);
			}
		}

		Debug.Log("GameSettings RESET");
	}
	// В классе PauseSubMenuSettingsController

	// Этот метод берет данные и выставляет их на UI элементы
	private void ApplyLoadedSettings(SettingsData data)
	{
		Debug.Log("--- ПРИМЕНЯЕМ ЗАГРУЖЕННЫЕ НАСТРОЙКИ ---");

		// 1. Применяем FOV (здесь все без изменений)
		if (PlayerPrefs.HasKey(pauseSubMenuSettingsPlayerPrefs.KEY_FOV))
		{
			SetFOV(data.FOV);
			fovSlider.value = data.FOV;
			Debug.Log($"Применен FOV: {data.FOV}");
		}

		// 2. ПРИМЕНЯЕМ БИНДИНГИ (НОВАЯ ЛОГИКА!)
		// Мы просто проходим по всем загруженным данным и применяем их.
		// Нам не важно, какое поле ввода сейчас на экране.
		// Если действие есть в данных - оно будет применено.

		if (data.KeyBindings != null && data.KeyBindings.Count > 0)
		{
			Debug.Log("Применяем биндинги клавиш:");
			foreach (var kvp in data.KeyBindings)
			{
				string actionName = kvp.Key; // Например, "Jump"
				KeyCode savedKey = kvp.Value; // Например, "Space"

				// Ищем на сцене поле ввода, которое соответствует этому действию
				bool foundField = false;
				foreach (var field in KeyRebinds)
				{
					// Получаем имя действия из имени поля ввода (например, "Jump (InputField)" -> "Jump")
					string fieldActionName = field.name.Replace(" (InputField)", "");

					if (fieldActionName == actionName)
					{
						// Если имя совпало - применяем!
						field.text = savedKey.ToString();
						inputDevice.RebindKey(actionName, savedKey);
						Debug.Log($"УСПЕШНО применено к '{actionName}': {savedKey}");
						foundField = true;
						break; // Выходим из внутреннего цикла, переходим к следующему действию
					}
				}
				if (!foundField)
				{
					Debug.LogWarning($"Поле ввода для действия '{actionName}' не найдено на сцене.");
				}
			}
		}
		else
		{
			Debug.Log("Словарь биндингов пуст. Сохраненных клавиш нет.");
		}
	}

}

