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
		this.buttonSaveSettings.GetComponent<Button>().onClick.AddListener(() => OnRequestSaveSettingsConfirmation());
		this.buttonResetSettings.GetComponent<Button>().onClick.AddListener(() => OnRequestResetSettingsConfirmation());

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

		// --- НОВАЯ ЛОГИКА ЗАГРУЗКИ ---
		// 1. Получаем список всех возможных действий из устройства ввода.
		//    Это делает Контроллер, а не Хранилище.
		var defaultBindings = this.inputDevice.GetDefaultBindings();
		List<string> actionNames = new List<string>(defaultBindings.Keys);

		// 2. Вызываем метод LoadSettings и передаем ему этот список.
		//    Хранилище теперь не знает ничего об IInputDevice, оно просто получает список ключей.
		SettingsData loadedData = pauseSubMenuSettingsPlayerPrefs.LoadSettings(actionNames);

		// 3. Применяем загруженные данные к UI.
		ApplyLoadedSettings(loadedData);
		this.menuManager.OnOpenConfirmMenu += DisableButtons;
		this.menuManager.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSubMenu Initialized");

		
	}

	// Ссылка на контроллер паузы меню
	private void DisableButtons()
	{

	}
	private void EnableButtons()
	{

	}

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
		if (!Enum.TryParse<KeyCode>(newKeyStr, out KeyCode newKey))
		{
			//Debug.LogWarning($"Некорректная клавиша: {newKeyStr}. Введите допустимое обозначение клавиши.");
			return;
		}

		// Получаем текущие биндинги (словарь "Действие -> Клавиша")
		var currentBindings = inputDevice.GetCurrentBindings().ToDictionary(kvp => kvp.action, kvp => kvp.key);

		// 1. Ищем, не занята ли уже эта клавиша другим действием (кроме текущего)
		var conflictingAction = currentBindings.FirstOrDefault(kvp => kvp.Value == newKey && kvp.Key != actionName).Key;

		if (conflictingAction != null)
		{
			// 2. Если занята — находим, какая клавиша у текущего действия (actionName)
			KeyCode oldKeyOfThisAction = currentBindings[actionName];

			// 3. Меняем местами в устройстве ввода
			inputDevice.RebindKey(actionName, newKey);
			inputDevice.RebindKey(conflictingAction, oldKeyOfThisAction);

			// 4. Обновляем UI для обоих полей
			UpdateInputFieldText(actionName, newKey);
			UpdateInputFieldText(conflictingAction, oldKeyOfThisAction);

			//Debug.Log($"Клавиши поменялись местами: {actionName} <-> {conflictingAction}");
		}
		else
		{
			// Если клавиша свободна — просто назначаем
			inputDevice.RebindKey(actionName, newKey);
			UpdateInputFieldText(actionName, newKey);
		}
	}

	// --- ДОБАВИТЬ ЭТИ МЕТОДЫ В САМЫЙ КОНЕЦ КЛАССА PauseSubMenuSettingsController ---
	// Вспомогательный метод для обновления текста в InputField по имени действия
	private void UpdateInputFieldText(string actionName, KeyCode key)
	{
		foreach (var field in KeyRebinds)
		{
			// Имя поля обычно выглядит как "JumpInputField" или "Jump (InputField)"
			// Мы ищем по началу строки, чтобы избежать проблем с пробелами или скобками
			if (field.name.StartsWith(actionName))
			{
				field.text = key.ToString();
				break;
			}
		}
	}
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
		//Debug.Log("=== СОХРАНЯЕМЫЕ ДАННЫЕ ===");
		foreach (var kvp in currentData.KeyBindings)
		{
			//Debug.Log($"Сохраняю: {kvp.Key} = {kvp.Value}");
		}
		//Debug.Log("=========================");
		// ---------------------------------

		// 3. Передаем объект классу-хранилищу
		pauseSubMenuSettingsPlayerPrefs.SaveSettings(currentData);

		//Debug.Log("GameSettings SAVED");
	}

	// Метод для кнопки "Сбросить"
	// В классе PauseSubMenuSettingsController

	// Полностью заменяем старый метод ResetSettings
	// В классе PauseSubMenuSettingsController

	// В классе PauseSubMenuSettingsController

	// В классе PauseSubMenuSettingsController

	public void ResetSettings()
	{
		// --- 1. СТАРТ ПРОЦЕССА ---
		//Debug.Log("=== НАЧАЛО СБРОСА НАСТРОЕК (С ПОЛНОЙ ОТЛАДКОЙ) ===");

		// 1. ПОЛНОЕ УДАЛЕНИЕ всех старых данных из PlayerPrefs
		//Debug.Log("1. Удаляем старые данные из PlayerPrefs...");
		pauseSubMenuSettingsPlayerPrefs.ResetSettings();
		//Debug.Log("1. Старые данные УДАЛЕНЫ.");


		// --- 2. ПРОВЕРЯЕМ, ЧТО ЛЕЖИТ В ДЕФОЛТНОМ СЛОВАРЕ ---
		//Debug.Log("2. Проверяем содержимое словаря DEFAULT (inputDevice.GetDefaultBindings()):");

		// Получаем "снимок" дефолтных значений
		var defaultBindingsSnapshot = inputDevice.GetDefaultBindings();

		// Проверяем, не пустой ли он вообще
		if (defaultBindingsSnapshot == null)
		{
			//Debug.LogError("ОШИБКА: defaultBindingsSnapshot == null! Словарь пуст.");
		}
		else if (defaultBindingsSnapshot.Count == 0)
		{
			//Debug.LogWarning("ВНИМАНИЕ: Словарь дефолтных биндингов ПУСТОЙ. В нем 0 элементов.");
		}
		else
		{
			// Если словарь не пустой, выводим его содержимое
			//Debug.Log($"Словарь содержит {defaultBindingsSnapshot.Count} элементов. Вот они:");
			foreach (var kvp in defaultBindingsSnapshot)
			{
				//Debug.Log($"   Действие: {kvp.Key} | Клавиша: {kvp.Value}");
			}
		}
		// --- КОНЕЦ ПРОВЕРКИ СЛОВАРЯ ---


		// 3. Создаем объект с дефолтными значениями для сохранения
		SettingsData defaultData = new SettingsData
		{
			FOV = MIN_FOV_VALUE,
			// Используем наш "снимок" для создания данных для сохранения
			KeyBindings = defaultBindingsSnapshot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		// 4. Сохраняем эти дефолтные значения в PlayerPrefs (перезапись)
		//Debug.Log("3. Сохраняем НОВЫЕ дефолтные данные в PlayerPrefs...");
		
		// Дополнительная проверка: что именно мы собираемся сохранить?
		if (defaultData.KeyBindings == null || defaultData.KeyBindings.Count == 0)
		{
			//Debug.LogWarning("ВНИМАНИЕ: Объект для сохранения содержит пустой словарь KeyBindings!");
		}

		pauseSubMenuSettingsPlayerPrefs.SaveSettings(defaultData);

		//Debug.Log("3. Данные успешно сохранены в PlayerPrefs.");


		// 5. Обновляем UI и текущую логику управления НЕМЕДЛЕННО
		//Debug.Log("4. Обновляем UI и InputDevice на форме...");

		SetFOV(MIN_FOV_VALUE);
		fovSlider.value = MIN_FOV_VALUE;

		// Обновляем поля ввода клавиш на форме и в логике устройства
		foreach (var field in KeyRebinds)
		{
			string actionName = field.name.Replace("InputField", "");

			if (defaultData.KeyBindings.TryGetValue(actionName, out var key))
			{
				field.text = key.ToString();
				inputDevice.RebindKey(actionName, key);
				//Debug.Log($"   [UI] Поле '{actionName}' обновлено на: {key}");
			}
			else
			{
				// Это важная проверка: если действие из поля ввода не нашлось в словаре дефолтов
				//Debug.LogWarning($"   [UI] ВНИМАНИЕ: Действие '{actionName}' не найдено в словаре дефолтных значений!");
			}
		}

		// --- 6. ФИНИШ ---
		//Debug.Log("=== СБРОС ЗАВЕРШЕН ===");
	}
	// В классе PauseSubMenuSettingsController

	// Этот метод берет данные и выставляет их на UI элементы
	private void ApplyLoadedSettings(SettingsData data)
	{
		//Debug.Log("--- ПРИМЕНЯЕМ ЗАГРУЖЕННЫЕ НАСТРОЙКИ ---");

		// 1. Применяем FOV (здесь все без изменений)
		if (PlayerPrefs.HasKey(pauseSubMenuSettingsPlayerPrefs.KEY_FOV))
		{
			SetFOV(data.FOV);
			fovSlider.value = data.FOV;
			//Debug.Log($"Применен FOV: {data.FOV}");
		}

		// 2. ПРИМЕНЯЕМ БИНДИНГИ (НОВАЯ ЛОГИКА!)
		// Мы просто проходим по всем загруженным данным и применяем их.
		// Нам не важно, какое поле ввода сейчас на экране.
		// Если действие есть в данных - оно будет применено.

		if (data.KeyBindings != null && data.KeyBindings.Count > 0)
		{
			//Debug.Log("Применяем биндинги клавиш:");
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
						//Debug.Log($"УСПЕШНО применено к '{actionName}': {savedKey}");
						foundField = true;
						break; // Выходим из внутреннего цикла, переходим к следующему действию
					}
				}
				if (!foundField)
				{
					//Debug.LogWarning($"Поле ввода для действия '{actionName}' не найдено на сцене.");
				}
			}
		}
		else
		{
			//Debug.Log("Словарь биндингов пуст. Сохраненных клавиш нет.");
		}
	}

}

