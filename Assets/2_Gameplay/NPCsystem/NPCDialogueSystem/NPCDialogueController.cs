using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueController : MonoBehaviour
{
	[SerializeField] private TextAsset russianDialogueFile;
	[SerializeField] private TextAsset englishDialogueFile;
	[SerializeField] private List<NPCDialogueBranch> dialogueSteps;
	private MenuManager menuManager;
	private Button buttonDialogueYes;
	private Button buttonDialogueNo;
	public TextAsset RussianDialogueFile => russianDialogueFile;
	private LocalizationManager localizationManager;
	public TextAsset EnglishDialogueFile => englishDialogueFile;
	// Словарь для фраз на разных языках
	private Dictionary<LanguagesEnum, List<string>> localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};
	private TextMeshProUGUI NPCdialogueText;
	// Публичное свойство, дающее доступ к приватному словарю только для чтения
	public Dictionary<LanguagesEnum, List<string>> LocalizedDialogue => localizedDialogue;
	private GameObject canvasDialogueMenu;
	private GameSceneManager gameSceneManager;
	private int currentDialogueStepIndex = 0; // Текущий индекс шага диалога
	private bool CanSkip;
	public bool IsDialogueActive { get; private set; }
	private void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		LoadDialogueFromFiles();

		buttonDialogueYes = ServiceLocator.Resolve<Button>("buttonDialogueYes");
		buttonDialogueNo = ServiceLocator.Resolve<Button>("buttonDialogueNo");

		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasDialogueMenu = ServiceLocator.Resolve<GameObject>("CanvasDialogueMenu");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		NPCdialogueText = ServiceLocator.Resolve<TextMeshProUGUI>("NPCdialogueText");

		buttonDialogueYes.onClick.AddListener(() =>
		{
			SelectOption(true); // Обрабатываем основной выбор (вариант "Да")
			CanSkip = true;
		});
		buttonDialogueNo.onClick.AddListener(() =>
		{
			SelectOption(false); // Обрабатываем основной выбор (вариант "Да")
			CanSkip = true;
		});

		// Связываем управление панелями меню с открытием и закрытием паузы
		menuManager.OnOpenPauseMenu += HideNPCDialogueCanvas;
		menuManager.OnClosePauseMenu += ShowNPCDialogueCanvas;

		// Закрываем головоломку при смене сцены
		gameSceneManager.OnBeginLoadMainMenuScene += ExitNPCDialogue;
		gameSceneManager.OnBeginLoadGameplayScene += ExitNPCDialogue;

		CanSkip = true;

	}
	private void Update()
	{
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsDialogueActive && CanSkip && !menuManager.IsPauseMenuOpened)
		{
			DisplayNextDialogueLine(); // Показываем следующую строку диалога по нажатию пробела
		}
	}
	private void ExitNPCDialogue()
	{
		menuManager.CloseDialogueMenu();
		HideNPCDialogueCanvas();
		IsDialogueActive = false;
		currentDialogueStepIndex = 0;

		DeactivateButtons();
	}
	private void LoadDialogueFromFiles()
	{
		// Русские фразы
		if (russianDialogueFile != null)
		{
			using (var reader = new StringReader(russianDialogueFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedDialogue[LanguagesEnum.Russian].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			if (russianDialogueFile == null && englishDialogueFile != null)
			Debug.LogWarning("Русская версия диалога не указана!");
		}

		// Английские фразы
		if (englishDialogueFile != null)
		{
			using (var reader = new StringReader(englishDialogueFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedDialogue[LanguagesEnum.English].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			if (russianDialogueFile != null && englishDialogueFile == null)
				Debug.LogWarning("Английская версия диалога не указана!");
		}
	}
	public void ShowNPCDialogueCanvas()
	{
		if (IsDialogueActive)
		{
			canvasDialogueMenu.SetActive(true);
		}
	}
	private void HideNPCDialogueCanvas()
	{
		if (IsDialogueActive)
		{
			canvasDialogueMenu.SetActive(false);
		}
	}
	public void Interact()
	{
		menuManager.OpenDialogueMenu();
		IsDialogueActive = true;
		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine(); // Показываем первую строку диалога
	}
	
	private void DisplayNextDialogueLine()
	{
		var currentLanguage = localizationManager.CurrentLanguage;



		// Получаем текущий шаг диалога
	


		// Проверяем, не вышел ли за пределы диалога
		if (currentDialogueStepIndex >= localizedDialogue[currentLanguage].Count)
		{
			Debug.Log("LMAO");
			ExitNPCDialogue(); // Завершаем диалог
			return;
		}

		// Определяем текущий язык
		NPCdialogueText.text = localizedDialogue[currentLanguage][currentDialogueStepIndex];

		if (dialogueSteps.Count > 0)
		{
			
			int i = 0;
			var step = 0;
			for (i = 0; i < dialogueSteps.Count; i++)
			{
				Debug.Log(dialogueSteps[i].LineNumber);
				Debug.Log(currentDialogueStepIndex + 1);

				if (dialogueSteps[i].LineNumber == (currentDialogueStepIndex + 1))
				{
				

					step = dialogueSteps[i].LineNumber;
					NPCdialogueText.text = localizedDialogue[currentLanguage][i];
					ActivateButtons();
					CanSkip = false;
					Debug.Log("FOUND!");
					break;
				}

			
			}
			Debug.Log("CONTINUE");
		}
		
			currentDialogueStepIndex++;
		
	}
	



	
	private void ActivateButtons()
	{
		buttonDialogueYes.gameObject.SetActive(true);
		buttonDialogueNo.gameObject.SetActive(true);
	}

	private void DeactivateButtons()
	{
		buttonDialogueYes.gameObject.SetActive(false);
		buttonDialogueNo.gameObject.SetActive(false);
	}
	private void SelectOption(bool isYesSelected)
	{
		if (isYesSelected)
		{
			currentDialogueStepIndex = dialogueSteps[currentDialogueStepIndex].YesOptionIndex;
		}
		else
		{
			currentDialogueStepIndex = dialogueSteps[currentDialogueStepIndex].NoOptionIndex;
		}

		currentDialogueStepIndex++;
	}

}