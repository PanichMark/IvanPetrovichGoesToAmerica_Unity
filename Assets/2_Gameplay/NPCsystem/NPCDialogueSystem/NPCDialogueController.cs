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
	private bool PerformActionOnYesFinal;
	public TextAsset EnglishDialogueFile => englishDialogueFile;
	private Dictionary<LanguagesEnum, List<string>> localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};
	public Dictionary<LanguagesEnum, List<string>> LocalizedDialogue => localizedDialogue;
	private TextMeshProUGUI NPCdialogueText;
	private GameObject canvasDialogueMenu;
	private GameSceneManager gameSceneManager;
	private int currentDialogueStepIndex = 0;
	private bool CanSkip;
	private NPCStateMachineController nPCStateMachineController;
	public bool IsDialogueActive { get; private set; }

	private void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		LoadDialogueFromFiles();

		buttonDialogueYes = ServiceLocator.Resolve<Button>("ButtonDialogueYes");
		buttonDialogueNo = ServiceLocator.Resolve<Button>("ButtonDialogueNo");

		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasDialogueMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuDialogue");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		NPCdialogueText = ServiceLocator.Resolve<TextMeshProUGUI>("TextDialogue");

		nPCStateMachineController = GetComponent<NPCStateMachineController>();

		menuManager.OnOpenPauseMenu += HideNPCDialogueCanvas;
		menuManager.OnClosePauseMenu += ShowNPCDialogueCanvas;

		gameSceneManager.OnBeginLoadMainMenuScene += ExitNPCDialogue;
		gameSceneManager.OnBeginLoadGameplayScene += ExitNPCDialogue;

		CanSkip = true;
	}

	private void Update()
	{
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsDialogueActive && CanSkip && !menuManager.IsPauseMenuOpened)
		{
			DisplayNextDialogueLine();
		}
	}

	private void ExitNPCDialogue()
	{
		if (IsDialogueActive)
		{

			buttonDialogueYes.onClick.RemoveAllListeners();
			buttonDialogueNo.onClick.RemoveAllListeners();


			currentDialogueStepIndex = 0;
			dialogueBranchStructIndex = 0;
			menuManager.CloseDialogueMenu();
			HideNPCDialogueCanvas();
			IsDialogueActive = false;
			DeactivateButtons();

			if (PerformActionOnYesFinal)
			{
				dialogueBranchStructsList[dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<IInteractable>().Interact();
				PerformActionOnYesFinal = false;
			}
		}
	}

	private void LoadDialogueFromFiles()
	{
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
		else if (englishDialogueFile != null)
		{
			Debug.LogWarning("Russian dialogue file is not set!");
		}

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
		else if (russianDialogueFile != null)
		{
			Debug.LogWarning("English dialogue file is not set!");
		}
	}

	public void ShowNPCDialogueCanvas()
	{
		if (IsDialogueActive)
			canvasDialogueMenu.SetActive(true);
	}

	private void HideNPCDialogueCanvas()
	{
		if (IsDialogueActive)
			canvasDialogueMenu.SetActive(false);
	}

	public void Interact()
	{
		currentDialogueStepIndex = 0;
		dialogueBranchStructIndex = 0;

		menuManager.OpenDialogueMenu();
		IsDialogueActive = true;

		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine();
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

		if (dialogueBranchStructsList.Count > 0)
		{
			for (int i = 0; i < dialogueBranchStructsList.Count; i++)
			{
				if (dialogueBranchStructsList[i].DialogueBranchLine == (currentDialogueStepIndex + 1))
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

		if (currentDialogueStepIndex == dialogueBranchStructsList[dialogueBranchStructIndex].FinalYesLine)
		{
			currentDialogueStepIndex = dialogueBranchStructsList[dialogueBranchStructIndex].GoToYesFinalLine;

			if (dialogueBranchStructsList[dialogueBranchStructIndex].ActionOnYesAnswer != null)
			{
				PerformActionOnYesFinal = true;
			}
		}
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

		if (!isYesSelected)
			currentDialogueStepIndex = dialogueBranchStructsList[dialogueBranchStructIndex].GoToNoOptionLine - 1;

		buttonDialogueYes.onClick.RemoveAllListeners();
		buttonDialogueNo.onClick.RemoveAllListeners();

		DisplayNextDialogueLine();

		DeactivateButtons();

		CanSkip = true;
	}
}