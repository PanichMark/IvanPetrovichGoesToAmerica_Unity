using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueController : MonoBehaviour
{
	[SerializeField] private NPCDialogueData _NPCdialogueData;

	public NPCDialogueData NPCdialogueData => _NPCdialogueData;


	[SerializeField] private List<NPCDialogueBranch> _dialogueBranchStructsList;
	private int _dialogueBranchStructIndex;
	private MenuManager _menuManager;
	private Button _buttonDialogueYes;
	private Button _buttonDialogueNo;

	private LocalizationManager _localizationManager;
	private bool _PerformActionOnYesFinal;


	private Dictionary<LanguagesEnum, List<string>> _localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};
	public Dictionary<LanguagesEnum, List<string>> LocalizedDialogue => _localizedDialogue;
	private TextMeshProUGUI _NPCdialogueText;
	private GameObject _canvasDialogueMenu;
	private GameSceneManager _gameSceneManager;
	private int _currentDialogueStepIndex;
	private bool _canSkip;
	private NPCStateMachineController _NPCstateMachineController;
	public bool IsDialogueActive { get; private set; }

	public void Initialize()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		LoadDialogueFromFiles();

		_buttonDialogueYes = ServiceLocator.Resolve<Button>("ButtonDialogueYes");
		_buttonDialogueNo = ServiceLocator.Resolve<Button>("ButtonDialogueNo");

		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_canvasDialogueMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuDialogue");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_NPCdialogueText = ServiceLocator.Resolve<TextMeshProUGUI>("TextDialogueLine");

		_NPCstateMachineController = GetComponent<NPCStateMachineController>();

		_menuManager.OnOpenPauseMenu += HideNPCDialogueCanvas;
		_menuManager.OnClosePauseMenu += ShowNPCDialogueCanvas;

		_gameSceneManager.OnBeginLoadingMainMenuScene += ExitNPCDialogue;
		_gameSceneManager.OnBeginLoadingGameplayScene += ExitNPCDialogue;

		_canSkip = true;
	}

	private void Update()
	{
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsDialogueActive && _canSkip && !_menuManager.IsPauseMenuOpened)
		{
			DisplayNextDialogueLine();
		}
	}

	private void ExitNPCDialogue()
	{
		if (IsDialogueActive)
		{
			_buttonDialogueYes.onClick.RemoveAllListeners();
			_buttonDialogueNo.onClick.RemoveAllListeners();

			_currentDialogueStepIndex = 0;
			_dialogueBranchStructIndex = 0;
			_menuManager.CloseDialogueMenu();
			HideNPCDialogueCanvas();
			IsDialogueActive = false;
			DeactivateButtons();

			if (_PerformActionOnYesFinal)
			{
				if (_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<IInteractable>() != null)
				{
					_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<IInteractable>().Interact();
				}
				if (_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<CutsceneController>() != null)
				{
					_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<CutsceneController>().TriggerCutscene();
				}
				_PerformActionOnYesFinal = false;
			}
		}
	}

	private void LoadDialogueFromFiles()
	{
		if (_NPCdialogueData.DialogueFileRussian != null)
		{
			using (var reader = new StringReader(_NPCdialogueData.DialogueFileRussian.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						_localizedDialogue[LanguagesEnum.Russian].Add(line.Trim());
					}
				}
			}
		}
		else if (_NPCdialogueData.DialogueFileEnglish != null)
		{
			Debug.LogWarning("Russian dialogue file is not set!");
		}

		if (_NPCdialogueData.DialogueFileEnglish != null)
		{
			using (var reader = new StringReader(_NPCdialogueData.DialogueFileEnglish.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						_localizedDialogue[LanguagesEnum.English].Add(line.Trim());
					}
				}
			}
		}
		else if (_NPCdialogueData.DialogueFileRussian != null)
		{
			Debug.LogWarning("English dialogue file is not set!");
		}
	}

	public void ShowNPCDialogueCanvas()
	{
		if (IsDialogueActive)
			_canvasDialogueMenu.SetActive(true);
	}

	private void HideNPCDialogueCanvas()
	{
		if (IsDialogueActive)
			_canvasDialogueMenu.SetActive(false);
	}

	public void Interact()
	{
		_currentDialogueStepIndex = 0;
		_dialogueBranchStructIndex = 0;

		_menuManager.OpenDialogueMenu();
		IsDialogueActive = true;

		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine();
	}

	private void DisplayNextDialogueLine()
	{
		var currentLanguage = _localizationManager.CurrentLanguage;

		if (_currentDialogueStepIndex >= _localizedDialogue[currentLanguage].Count)
		{
			ExitNPCDialogue();
			_NPCstateMachineController.RotateTowardsInitialRotation();
			return;
		}

		_NPCdialogueText.text = _localizedDialogue[currentLanguage][_currentDialogueStepIndex];

		if (_dialogueBranchStructsList.Count > 0)
		{
			for (int i = 0; i < _dialogueBranchStructsList.Count; i++)
			{
				if (_dialogueBranchStructsList[i].DialogueBranchLine == (_currentDialogueStepIndex + 1))
				{
					_dialogueBranchStructIndex = i;
					_canSkip = false;
					ActivateButtons();
					_buttonDialogueYes.onClick.AddListener(() => SelectOption(true));
					_buttonDialogueNo.onClick.AddListener(() => SelectOption(false));
					break;
				}
			}
		}

		_currentDialogueStepIndex++;

		if (_currentDialogueStepIndex == _dialogueBranchStructsList[_dialogueBranchStructIndex].FinalYesLine)
		{
			_currentDialogueStepIndex = _dialogueBranchStructsList[_dialogueBranchStructIndex].GoToYesFinalLine;

			if (_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer != null)
			{
				_PerformActionOnYesFinal = true;
			}
		}
	}

	private void ActivateButtons()
	{
		_buttonDialogueYes.gameObject.SetActive(true);
		_buttonDialogueNo.gameObject.SetActive(true);
	}

	private void DeactivateButtons()
	{
		_buttonDialogueYes.gameObject.SetActive(false);
		_buttonDialogueNo.gameObject.SetActive(false);
	}

	private void SelectOption(bool isYesSelected)
	{
		var currentLanguage = _localizationManager.CurrentLanguage;

		if (!isYesSelected)
			_currentDialogueStepIndex = _dialogueBranchStructsList[_dialogueBranchStructIndex].GoToNoOptionLine - 1;

		_buttonDialogueYes.onClick.RemoveAllListeners();
		_buttonDialogueNo.onClick.RemoveAllListeners();

		DisplayNextDialogueLine();

		DeactivateButtons();

		_canSkip = true;
	}
}