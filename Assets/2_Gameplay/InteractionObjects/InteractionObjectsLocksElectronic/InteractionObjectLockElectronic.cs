using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObjectLockElectronic : MonoBehaviour, IInteractable
{
	public bool WasUnlocked { get; private set; }
	public string InteractionObjectNameSystem => throw new NotImplementedException();
	public string InteractionObjectNameUI => "Электронный замок";
	public string InteractionHintMessageMain => "Взломать?";
	public string InteractionHintAction => throw new NotImplementedException();
	public string InteractionHintMessageAdditional => throw new NotImplementedException();
	private bool IsPuzzleActive;
	public bool IsInteractionHintMessageAdditionalActive => throw new NotImplementedException();

	private Button buttonExitLockpickElectronicMenu;
	private GameObject[] buttonsLockElectrical;
	private MenuManager menuManager;
	private GameObject canvasLockpickElectronicMenu;
	private SaveLoadController saveLoadController;
	private GameSceneManager gameSceneManager;
	private LocalizationManager localizationManager;

	// Список индексов кнопок-"alarm"
	private List<int> alarmIndices;

	// Кол-во оставшихся ходов
	private int movesLeft = 4;

	void Start()
	{
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasLockpickElectronicMenu = ServiceLocator.Resolve<GameObject>("CanvasLockpickElectronicMenu");
		buttonExitLockpickElectronicMenu = ServiceLocator.Resolve<Button>("ExitLockpickElectronic");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		buttonsLockElectrical = ServiceLocator.Resolve<GameObject[]>("buttonsLockElectrical");

		// Добавляем обработчик закрытия головоломки при выходе
		buttonExitLockpickElectronicMenu.onClick.AddListener(CloseElectronicLockPuzzle);

		// Связываем управление панелями меню с открытием и закрытием паузы
		menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		menuManager.OnClosePauseMenu += ShowPuzzleCanvas;

		// Закрываем головоломку при смене сцены
		gameSceneManager.OnBeginLoadMainMenuScene += CloseElectronicLockPuzzle;
		gameSceneManager.OnBeginLoadGameplayScene += CloseElectronicLockPuzzle;
	}

	private void CloseElectronicLockPuzzle()
	{
		if (IsPuzzleActive)
		{
			IsPuzzleActive = false;
			canvasLockpickElectronicMenu.SetActive(false);
			menuManager.CloseLockpickMenu();
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickElectronicMenu.SetActive(true);
		}
	}

	private void HidePuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickElectronicMenu.SetActive(false);
		}
	}

	public void Interact()
	{
		if (!IsPuzzleActive)
		{
			IsPuzzleActive = true;
			menuManager.OpenLockpickMenu();
			InitializeButtons();
			ShowPuzzleCanvas();
		}
	}

	private void InitializeButtons()
	{
		// Сброс количества ходов при перезапуске пазла
		movesLeft = 4;

		// Сначала очищаем состояние всех кнопок
		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.white; // Восстанавливаем первоначальный цвет
			button.colors = colors;
			button.interactable = true;       // Активируем кнопку
		}

		// Список индексов кнопок
		List<int> indices = Enumerable.Range(0, buttonsLockElectrical.Length).ToList();

		// Случайно выбираем 4 кнопки из 9 и помечаем их как "alarm"
		alarmIndices = new List<int>();
		while (alarmIndices.Count < 4)
		{
			int index = UnityEngine.Random.Range(0, indices.Count);
			alarmIndices.Add(indices[index]); // Запоминаем индексы alarm-кнопок
			indices.RemoveAt(index);          // Исключаем выбранный индекс из дальнейшего выбора
		}

		// Добавляем обработчики нажатия на каждую кнопку
		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.onClick.AddListener(() => OnButtonClicked(button));
		}
	}

	private void OnButtonClicked(Button clickedButton)
	{
		// Определяем индекс нажатой кнопки
		int buttonIndex = Array.IndexOf(buttonsLockElectrical, clickedButton.gameObject);

		// Проверяем, является ли данная кнопка "alarm"
		if (alarmIndices.Contains(buttonIndex)) // Если нажали на alarm-кнопку
		{
			// Проигрыш: красим все кнопки в красный
			Debug.Log("FAIL!");
			SetAllButtonsRed();
		
			return;
		}

		// Если кнопка нормальная: красим её в зелёный и делаем неактивной
		ColorBlock colors = clickedButton.colors;
		colors.normalColor = Color.green;
		clickedButton.colors = colors;
		clickedButton.interactable = false;

		// Раскрываем одну из случайных alarm-кнопок
		RevealAlarmButton();

		// Уменьшаем счётчик ходов
		movesLeft--;

		// Завершаем пазл, если закончились ходы
		if (movesLeft <= 0)
		{
			EndPuzzle();
		}
	}

	private void SetAllButtonsRed()
	{
		Debug.Log("SET ALL RED");
		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.red;
			button.colors = colors;
			button.interactable = false;
		}
		CloseElectronicLockPuzzle();
	}

	private void RevealAlarmButton()
	{
		// Получаем список доступных alarm-кнопок
		List<Button> availableAlarms = GetAvailableAlarmButtons();

		if (availableAlarms.Any())
		{
			// Выбираем случайную из доступных alarm-кнопок
			int randomIndex = UnityEngine.Random.Range(0, availableAlarms.Count);
			Button revealedButton = availableAlarms[randomIndex];

			// Красим выбранную alarm-кнопку в красный и отключаем
			ColorBlock colors = revealedButton.colors;
			colors.normalColor = Color.red;
			revealedButton.colors = colors;
			revealedButton.interactable = false;
		}
	}

	private void EndPuzzle()
	{
		WasUnlocked = true;
		IsPuzzleActive = false;
		HidePuzzleCanvas();
		menuManager.CloseLockpickMenu();
	}



	private List<Button> GetAvailableAlarmButtons()
	{
		List<Button> result = new List<Button>();
		foreach (int index in alarmIndices)
		{
			Button button = buttonsLockElectrical[index].GetComponent<Button>();
			if (button.interactable) // Только активные кнопки включаем в выбор
			{
				result.Add(button);
			}
		}
		return result;
	}
}