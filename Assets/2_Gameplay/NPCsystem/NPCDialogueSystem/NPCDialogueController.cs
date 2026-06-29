using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using uLipSync;

public class NPCDialogueController : MonoBehaviour
{
	public delegate void PhonemesBlendShapesHandler();
	public event PhonemesBlendShapesHandler OnResetPhonemesBlendShapes;

	[SerializeField] private NPCDialogueData _NPCdialogueData;
	private uLipSync.uLipSync _uLipSync;
	private uLipSyncBlendShape _uLipSyncBlendShape;
	public NPCDialogueData NPCdialogueData => _NPCdialogueData;
	private GameController _gameController;
	[SerializeField] private List<NPCDialogueBranch> _dialogueBranchStructsList;
	private AudioSource _audioSource;
	private int _dialogueBranchStructIndex;
	private MenuManager _menuManager;
	private Button _buttonDialogueYes;
	private Button _buttonDialogueNo;

	private LocalizationManager _localizationManager;
	private bool _PerformActionOnYesFinal;
	private GameObject _textDialogueYes;
	private GameObject _textDialogueNo;
	private NPCAbstract _NPCabstract;
	private TextMeshProUGUI _textComponentDialogueYes;
	private TextMeshProUGUI _textComponentDialogueNo;

	private Dictionary<LanguagesEnum, List<string>> _localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};
	private bool _isIvanPetrovichSpeaking;
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
		_uLipSyncBlendShape = GetComponent<uLipSyncBlendShape>();
		_uLipSync = GetComponent<uLipSync.uLipSync>();
		_audioSource = GetComponent<AudioSource>();
		_NPCabstract = GetComponent<NPCAbstract>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		LoadDialogueFromFiles();

		_buttonDialogueYes = ServiceLocator.Resolve<GameObject>("ButtonDialogueYes").GetComponent<Button>();
		_buttonDialogueNo = ServiceLocator.Resolve<GameObject>("ButtonDialogueNo").GetComponent<Button>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_textDialogueYes = ServiceLocator.Resolve<GameObject>("TextDialogueYes");
		_textDialogueNo = ServiceLocator.Resolve<GameObject>("TextDialogueNo");

		_textComponentDialogueYes = _textDialogueYes.GetComponent<TextMeshProUGUI>();
		_textComponentDialogueNo = _textDialogueNo.GetComponent<TextMeshProUGUI>();

		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_canvasDialogueMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuDialogue");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_NPCdialogueText = ServiceLocator.Resolve<GameObject>("TextDialogueLine").GetComponent<TextMeshProUGUI>();

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

		if (!_isIvanPetrovichSpeaking)
		{
			_uLipSyncBlendShape.ApplyBlendShapes();
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		LoadDialogueFromFiles();
	}

	private void ExitNPCDialogue()
	{
		if (IsDialogueActive)
		{
			_buttonDialogueYes.onClick.RemoveAllListeners();
			_buttonDialogueNo.onClick.RemoveAllListeners();
			_gameController.MakeGameSavable();
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
					_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer.GetComponent<IInteractable>().InteractCutscene();
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
		if (_NPCdialogueData.DialogueTextfileRussian != null)
		{
			using (var reader = new StringReader(_NPCdialogueData.DialogueTextfileRussian.text))
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
		else if (_NPCdialogueData.DialogueTextfileEnglish != null)
		{
			Debug.LogWarning("Russian dialogue file is not set!");
		}

		if (_NPCdialogueData.DialogueTextfileEnglish != null)
		{
			using (var reader = new StringReader(_NPCdialogueData.DialogueTextfileEnglish.text))
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
		else if (_NPCdialogueData.DialogueTextfileRussian != null)
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
		_gameController.MakeGameUnsavable();
		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine();
	}

	private void DisplayNextDialogueLine()
	{
		_isIvanPetrovichSpeaking = false;
		var currentLanguage = _localizationManager.CurrentLanguage;

		if (_currentDialogueStepIndex >= _localizedDialogue[currentLanguage].Count)
		{
			ExitNPCDialogue();
			_NPCstateMachineController.RotateTowardsInitialRotation();
			return;
		}

		StopPreviousVoiceLine();

		PlayVoiceLineForCurrentStep(currentLanguage);

		_NPCdialogueText.text = $"{_NPCabstract.InteractionObjectNameUI}: {_localizedDialogue[currentLanguage][_currentDialogueStepIndex]}";

		if (_dialogueBranchStructsList.Count > 0)
		{
			for (int i = 0; i < _dialogueBranchStructsList.Count; i++)
			{
				if ((_currentDialogueStepIndex + 1) == _dialogueBranchStructsList[i].DialogueBranchLine)
				{
					_dialogueBranchStructIndex = i;
					_canSkip = false;
					ActivateButtons();
					_buttonDialogueYes.onClick.AddListener(() => SelectOption(true));
					_buttonDialogueNo.onClick.AddListener(() => SelectOption(false));
					ShowDialogueAnswerOptions(i);
					break;
				}

				if ((_currentDialogueStepIndex == _dialogueBranchStructsList[i].DialogueBranchLine) || ((_currentDialogueStepIndex + 1) == _dialogueBranchStructsList[i].GoToNoOptionLine))
				{
					_NPCdialogueText.text = $"{_localizationManager.GetLocalizedString("IvanPetrovich")}: {_localizedDialogue[currentLanguage][_currentDialogueStepIndex]}";

					_isIvanPetrovichSpeaking = true;

					OnResetPhonemesBlendShapes?.Invoke();
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

	private void PlayVoiceLineForCurrentStep(LanguagesEnum currentLanguage)
	{
		AudioClip[] currentLanguageVoicelines = (currentLanguage == LanguagesEnum.Russian)
			? _NPCdialogueData.DialogueVoicelinesRussian
			: _NPCdialogueData.DialogueVoicelinesEnglish;

		if (currentLanguageVoicelines != null && _currentDialogueStepIndex < currentLanguageVoicelines.Length)
		{
			AudioClip clipToPlay = currentLanguageVoicelines[_currentDialogueStepIndex];
		
			_audioSource.clip = clipToPlay;
		
			_audioSource.Play();
		}
	}

	private void StopPreviousVoiceLine()
	{
		if (_audioSource.isPlaying)
		{
			_audioSource.Stop(); 
		}
	}

	private void ShowDialogueAnswerOptions(int index)
	{
		_textComponentDialogueYes.text = _localizationManager.GetLocalizedString(_dialogueBranchStructsList[index].YesOptionAnswer);
		_textComponentDialogueNo.text = _localizationManager.GetLocalizedString(_dialogueBranchStructsList[index].NoOptionAnswer);
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