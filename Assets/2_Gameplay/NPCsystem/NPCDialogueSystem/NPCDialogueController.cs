using NUnit.Framework;
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
	[SerializeField] private List<NPCDialogueBranch> dialogueBranchStructsList;
	private int dialogueBranchStructIndex;
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
	private NPCStateMachineController nPCStateMachineController;
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

		nPCStateMachineController = GetComponent<NPCStateMachineController>();

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
		buttonDialogueYes.onClick.RemoveAllListeners();
		buttonDialogueNo.onClick.RemoveAllListeners();

		currentDialogueStepIndex = 0;
		dialogueBranchStructIndex = 0;
		menuManager.CloseDialogueMenu();
		HideNPCDialogueCanvas();
		IsDialogueActive = false;

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
		currentDialogueStepIndex = 0;
		dialogueBranchStructIndex = 0;
		//dialogueStepsIndex = 0;
		menuManager.OpenDialogueMenu();
		IsDialogueActive = true;
		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine(); // Показываем первую строку диалога

	}

	private void DisplayNextDialogueLine()
	{
		var currentLanguage = localizationManager.CurrentLanguage;

		if (currentDialogueStepIndex >= localizedDialogue[currentLanguage].Count)
		{
			ExitNPCDialogue();
			nPCStateMachineController.RotateTowardsInitialRotation();
			return;
		}

		NPCdialogueText.text = localizedDialogue[currentLanguage][currentDialogueStepIndex];

		// Проверка на финальный переход по "Нет"
		if (dialogueBranchStructsList.Count > 0)
		{
			for (int i = 0; i < dialogueBranchStructsList.Count; i++)
			{
				if (dialogueBranchStructsList[i].DialogueBranchIndex == (currentDialogueStepIndex + 1))
				{
					dialogueBranchStructIndex = i;


					CanSkip = false;
					ActivateButtons();
					buttonDialogueYes.onClick.AddListener(() => SelectOption(true));
					buttonDialogueNo.onClick.AddListener(() => SelectOption(false));
					break;
				}
			}
		}
		
		currentDialogueStepIndex++;
		if (currentDialogueStepIndex == dialogueBranchStructsList[dialogueBranchStructIndex].FinalNoIndex)
		{
			currentDialogueStepIndex = dialogueBranchStructsList[dialogueBranchStructIndex].GoToNoFinal;
		}


		//Debug.Log(currentDialogueStepIndex);
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
		var currentLanguage = localizationManager.CurrentLanguage;

		if (isYesSelected)
		{
			currentDialogueStepIndex = dialogueBranchStructsList[dialogueBranchStructIndex].GoToYesOptionIndex - 1;
		}
		buttonDialogueYes.onClick.RemoveAllListeners();
		buttonDialogueNo.onClick.RemoveAllListeners();
		DisplayNextDialogueLine();
		DeactivateButtons();
		CanSkip = true;
	}
}