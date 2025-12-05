using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSubMenuController : MonoBehaviour
{
	// Ссылка на контроллер паузы меню
	PauseMenuController pauseMenuController;

	// Интерфейсы настроек
	public Canvas SettingsSubMenuCanvas;
	public Button CloseSettingsSubMenuButton;

	// Добавляем компонент Slider для регулировки FOV камеры
	public Slider fovSlider;

	// Тексты для вывода значения FOV
	public TextMeshProUGUI fovDisplayText;

	// Камеры, на которые будем влиять значением FOV
	public Camera MainCamera;
	public Camera AdditionalCamera;

	// Ограничители FPS
	public Button LimitFPS_30_Button;
	public Button LimitFPS_60_Button;
	public Button LimitFPS_90_Button;
	public Button LimitFPS_144_Button;

	// Цвет активной кнопки
	public Color activeColor = Color.green;
	// Обычный цвет кнопки
	public Color normalColor = Color.white;

	// Хранится текущее активное ограничение FPS
	int currentFrameRateLimit = 60;

	private const float MIN_FOV_VALUE = 60f; // Минимальное значение FOV
	private const float MAX_FOV_VALUE = 120f; // Максимальное значение FOV

	void Start()
	{
		// Получаем ссылку на контроллер меню паузы
		pauseMenuController = GetComponent<PauseMenuController>();

		// Подписываем обработчик события нажатия кнопки закрытия
		CloseSettingsSubMenuButton.onClick.AddListener(CloseSettingsSubMenu);

		// Проверяем наличие ползунка и подписываем обработчик изменения значения
		if (fovSlider != null)
		{
			fovSlider.minValue = MIN_FOV_VALUE;
			fovSlider.maxValue = MAX_FOV_VALUE;

			// Подписываем обработчик изменения значения ползунка
			fovSlider.onValueChanged.AddListener(OnFovChanged);

			// Устанавливаем начальное положение ползунка
			SetFOV(MIN_FOV_VALUE);
		}

		// Настройка кнопок для изменения лимита FPS
		LimitFPS_30_Button.onClick.AddListener(() => ChangeFrameRateLimit(30));
		LimitFPS_60_Button.onClick.AddListener(() => ChangeFrameRateLimit(60));
		LimitFPS_90_Button.onClick.AddListener(() => ChangeFrameRateLimit(90));
		LimitFPS_144_Button.onClick.AddListener(() => ChangeFrameRateLimit(144));

		// Применяем выделение изначально выбранной кнопки (для 60 FPS)
		ChangeFrameRateLimit(60);
		ApplyButtonColors(currentFrameRateLimit);
	}

	private void Update()
	{
		// Закрываем меню, если нажата клавиша паузы и открыто окно настроек
		if (InputManager.Instance.GetKeyPauseMenu() && SettingsSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseSettingsSubMenu();
		}
	}

	// Метод закрывает настройки и возвращает основное меню паузы
	public void CloseSettingsSubMenu()
	{
		SettingsSubMenuCanvas.gameObject.SetActive(false);
		pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SettingsSubMenu closed");
	}

	// Обработчик изменения значения ползунка FOV
	public void OnFovChanged(float value)
	{
		SetFOV(value);
	}

	// Установщик значения FOV для обеих камер
	private void SetFOV(float newFov)
	{
		if (MainCamera != null)
		{
			MainCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);
		}
		if (AdditionalCamera != null)
		{
			AdditionalCamera.fieldOfView = Mathf.Clamp(newFov, MIN_FOV_VALUE, MAX_FOV_VALUE);
		}

		// Отображаем текущее значение в тексте
		if (fovDisplayText != null)
		{
			fovDisplayText.text = ((int)newFov).ToString(); // Округляем до целого числа
		}
	}

	// Меняет ограничение FPS и выделяет соответствующую кнопку
	private void ChangeFrameRateLimit(int frameRate)
	{
		Application.targetFrameRate = frameRate;
		currentFrameRateLimit = frameRate;
		ApplyButtonColors(frameRate);
		Debug.Log($"Frame rate limit set to {frameRate}");
	}

	// Выделяет нужную кнопку активным цветом и сбрасывает остальные
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

	// Сбрасывает цвет всех кнопок
	private void ResetAllButtons()
	{
		LimitFPS_30_Button.image.color = normalColor;
		LimitFPS_60_Button.image.color = normalColor;
		LimitFPS_90_Button.image.color = normalColor;
		LimitFPS_144_Button.image.color = normalColor;
	}

	// Выделяет указанную кнопку
	private void HighlightButton(Button button)
	{
		button.image.color = activeColor;
	}
}