using System.Collections.Generic;
using System.IO;
using TMPro;
using uLipSync;
using UnityEngine;
using UnityEngine.UI;

public class NPCdialogueController : MonoBehaviour
{
	public delegate void BlendShapesResetterHandler();
	public event BlendShapesResetterHandler OnResetAllBlendShapesFacialExpressions;
	public event BlendShapesResetterHandler OnResetAllBlendShapesPhonemes;
	private string _originalAnimationStateName;
	private PlayerMovementController _playerMovementController;
	private PlayerCameraController _playerCameraController;
	public delegate void BlendShapesFacialExpressionsHandler(string newFacialExpression);
	public event BlendShapesFacialExpressionsHandler OnChangeBlendShapeFacialExpression;
	private NPCmovementController _NPCMovementController;
	public delegate void HeadIKHandler(GameObject objectToLookAt);
	public event HeadIKHandler OnStartLookingAtObject;
	public event HeadIKHandler OnStopLookingAtObject;
	public event HeadIKHandler OnAfterAnimatorReapplyIKweights;

	private string _currentGestureAnimation;
	[SerializeField] private NPCdialogueData _NPCdialogueData;
	private uLipSyncBlendShape _uLipSyncBlendShape;
	public NPCdialogueData NPCdialogueData => _NPCdialogueData;
	private GameController _gameController;
	[SerializeField] private List<NPCdialogueBranchData> _dialogueBranchStructsList;

	[SerializeField] private AnimationClip _dialogueDefaultAnimationStateName;
	[SerializeField] private List<NPCdialogueGesturesData> _dialogueGesturesDataList;
	private Animator _animator;

	[SerializeField] private List<NPCdialogueFacialExpressionsData> _dialogueFacialExpressionsDataList;
	private AudioSource _audioSource;
	private int _dialogueBranchStructIndex;
	private MenuManager _menuManager;
	private Button _buttonDialogueYes;
	private Button _buttonDialogueNo;

	private LocalizationManager _localizationManager;
	private bool _performActionOnYesFinal;
	private bool _performActionOnNoFinal;
	private GameObject _textDialogueYes;
	private GameObject _textDialogueNo;
	private NPCabstract _NPCabstract;
	private TextMeshProUGUI _textComponentDialogueYes;
	private TextMeshProUGUI _textComponentDialogueNo;
	private PlayerInteractionController _interactionController;
	private Dictionary<LanguagesEnum, List<string>> _localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{ LanguagesEnum.Russian, new List<string>() },
		{ LanguagesEnum.English, new List<string>() }
	};
	private bool _isIvanPetrovichSpeaking;
	private GameObject _playerEyesLookAt;
	//public Dictionary<LanguagesEnum, List<string>> LocalizedDialogue => _localizedDialogue;
	private TextMeshProUGUI _NPCdialogueText;
	
	private GameScenesManager _gameSceneManager;
	private int _currentDialogueStepIndex;
	private bool _canSkip;
	private NPCstateMachineController _NPCstateMachineController;
	public bool IsDialogueActive { get; private set; }

	private GameCanvasesList _canvasesList;
	private GameObject _canvasDialogue;
	private ViewModelMenuDialogue _viewModelMenuDialogue;
	private Bootstrap _bootstrap;

	public void Initialize(
		NPCmovementController NPCMovementController,
		NPCstateMachineController NPCstateMachineController)
	{
		_NPCMovementController = NPCMovementController;
		_bootstrap = ServiceLocator.Resolve < Bootstrap>();
		//_canvasesList = ServiceLocator.Resolve<GameCanvasesList>();
		_canvasDialogue = _bootstrap._canvasMenuDialogue;
		_viewModelMenuDialogue = ServiceLocator.Resolve<ViewModelMenuDialogue>();
		_uLipSyncBlendShape = GetComponent<uLipSyncBlendShape>();
		_audioSource = GetComponent<AudioSource>();
		_NPCabstract = GetComponent<NPCabstract>();
	_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
	_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>();
	_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>();
	_animator = GetComponent<Animator>();

	_interactionController = ServiceLocator.Resolve<PlayerInteractionController>();
	_buttonDialogueYes = _viewModelMenuDialogue.ButtonDialogueYes.GetComponent<Button>();
	_buttonDialogueNo = _viewModelMenuDialogue.ButtonDialogueNo.GetComponent<Button>();
		_gameController = ServiceLocator.Resolve<GameController>();
_textDialogueYes = _viewModelMenuDialogue.TextDialogueYes;
_textDialogueNo = _viewModelMenuDialogue.TextDialogueNo;

_playerEyesLookAt = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerEyes);

_textComponentDialogueYes = _textDialogueYes.GetComponent<TextMeshProUGUI>();
_textComponentDialogueNo = _textDialogueNo.GetComponent<TextMeshProUGUI>();

_menuManager = ServiceLocator.Resolve<MenuManager>();

_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>();
_NPCdialogueText = _viewModelMenuDialogue.TextDialogueLine.GetComponent<TextMeshProUGUI>();

		_NPCstateMachineController = NPCstateMachineController;

		_menuManager.OnOpenPauseMenu += HideNPCDialogueCanvas;
		_menuManager.OnClosePauseMenu += ShowNPCDialogueCanvas;

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += ExitNPCDialogue;
		_gameSceneManager.OnBeginLoadingGameplayScene += ExitNPCDialogue;

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		LoadDialogueFromFiles();

		_canSkip = true;
	}

	private void Update()
	{
		if (IsDialogueActive)
		{
			if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && _canSkip && !_menuManager.IsPauseMenuOpened)
			{
				DisplayNextDialogueLine();
			}

			if (!_isIvanPetrovichSpeaking)
			{
				_uLipSyncBlendShape.ApplyBlendShapes();
			}

			/*
			if (_menuManager.IsPauseMenuOpened || _menuManager.IsInteractionMenuOpened)
			{

			}
			else
			{
				_animator.Update(Time.unscaledDeltaTime);

				//OnAfterAnimatorReapplyIKweights?.Invoke(_playerEyesLookAt);
			}
			*/
		}

		//Debug.Log(_animator.speed);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		int temp = _currentDialogueStepIndex;
		_localizationManager = localizationManager;
		LoadDialogueFromFiles();
		_currentDialogueStepIndex = temp;
	}

	public void Interact()
	{
		//_animator.speed = 1f;
		_currentDialogueStepIndex = 0;
		_dialogueBranchStructIndex = 0;
		_interactionController.ChangeLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
		_originalAnimationStateName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
		_menuManager.OpenDialogueMenu();
		IsDialogueActive = true;
		ChangeGestureAnimation(_dialogueDefaultAnimationStateName.name);
		_gameController.MakeGameUnsavable();
		ShowNPCDialogueCanvas();
		DisplayNextDialogueLine();
		_NPCMovementController.RotateTowardsPlayer();
		_playerMovementController.RotatePlayerTowardsNPC(gameObject);
		_playerCameraController.RotateCameraTowardsNPC(gameObject);
		OnStartLookingAtObject?.Invoke(_playerEyesLookAt);
		_uLipSyncBlendShape.updateMethod = UpdateMethod.External;
	}

	private void ExitNPCDialogue()
	{
		if (IsDialogueActive)
		{
			//_animator.speed = 1f;
			_buttonDialogueYes.onClick.RemoveAllListeners();
			_buttonDialogueNo.onClick.RemoveAllListeners();
			_gameController.MakeGameSavable();
			_currentDialogueStepIndex = 0;
			
			_menuManager.CloseDialogueMenu();
			HideNPCDialogueCanvas();
			IsDialogueActive = false;
			DeactivateButtons();

			Debug.Log(_performActionOnYesFinal);
			Debug.Log(_performActionOnNoFinal);

			if (_performActionOnYesFinal)
			{
				var yesActionObject = _dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer;
				if (yesActionObject != null)
				{
					var interactable = yesActionObject.GetComponent<IInteractable>();
					if (interactable != null)
					{
						interactable.InteractAfterDialogueOrCutscene();
					}
					var cutscene = yesActionObject.GetComponent<CutsceneController>();
					if (cutscene != null)
					{
						cutscene.TriggerCutscene(null);
					}
				}
				_performActionOnYesFinal = false;
			}

			if (_performActionOnNoFinal)
			{
				var noActionObject = _dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnNoAnswer;
				if (noActionObject != null)
				{
					var interactable = noActionObject.GetComponent<IInteractable>();
					if (interactable != null)
					{
						interactable.InteractAfterDialogueOrCutscene();
					}
					var cutscene = noActionObject.GetComponent<CutsceneController>();
					if (cutscene != null)
					{
						cutscene.TriggerCutscene(null);
					}
				}
				_performActionOnNoFinal = false;
			}

			_dialogueBranchStructIndex = 0;
			ChangeGestureAnimation(_originalAnimationStateName);
			_playerCameraController.SetPostDialogueCameraTransform();
			OnStopLookingAtObject?.Invoke(_playerEyesLookAt);
			_uLipSyncBlendShape.updateMethod = UpdateMethod.LipSyncUpdateEvent;
		}
	}

	private void LoadDialogueFromFiles()
	{
		foreach (var list in _localizedDialogue.Values)
		{
			list.Clear();
		}

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
		{
			_canvasDialogue.SetActive(true);
			_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		}
	}

	private void HideNPCDialogueCanvas()
	{
		if (IsDialogueActive)
		{
			_canvasDialogue.SetActive(false);
			_animator.updateMode = AnimatorUpdateMode.Normal;
		}
	}

	private void DisplayNextDialogueLine()
	{
		_isIvanPetrovichSpeaking = false;
		var currentLanguage = _localizationManager.CurrentLanguage;

		if (_currentDialogueStepIndex >= _localizedDialogue[currentLanguage].Count)
		{
			ExitNPCDialogue();
			_NPCMovementController.RotateTowardsInitialRotation();
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

				if (_currentDialogueStepIndex == _dialogueBranchStructsList[i].DialogueBranchLine + 2 && _dialogueBranchStructsList[i].DoesIvanAnswerOnYes)
				{
					_NPCdialogueText.text = $"{_localizationManager.GetLocalizedString("NPC_MainCharacter_IvanPetrovich")}: {_localizedDialogue[currentLanguage][_currentDialogueStepIndex]}";

					_isIvanPetrovichSpeaking = true;

					OnResetAllBlendShapesPhonemes?.Invoke();
				}
				if (_currentDialogueStepIndex == _dialogueBranchStructsList[i].GoToNoOptionLine - 1 && _dialogueBranchStructsList[i].DoesIvanAnswerOnNo)
				{
					_NPCdialogueText.text = $"{_localizationManager.GetLocalizedString("NPC_MainCharacter_IvanPetrovich")}: {_localizedDialogue[currentLanguage][_currentDialogueStepIndex]}";

					_isIvanPetrovichSpeaking = true;

					OnResetAllBlendShapesPhonemes?.Invoke();
				}
			}
		}

		_currentDialogueStepIndex++;

		if (_currentDialogueStepIndex == _dialogueBranchStructsList[_dialogueBranchStructIndex].GoToNoOptionLine - 1)
		{
			_currentDialogueStepIndex = _localizedDialogue[_localizationManager.CurrentLanguage].Count;
		}

		if (_dialogueGesturesDataList.Count > 0)
		{
			bool gestureFound = false;
			for (int i = 0; i < _dialogueGesturesDataList.Count; i++)
			{
				if ((_currentDialogueStepIndex) == _dialogueGesturesDataList[i].DialogueStep)
				{
					string gestureName = _dialogueGesturesDataList[i].Gesture.name;
					ChangeGestureAnimation(gestureName);
					//Debug.Log(gestureName);
					gestureFound = true;
					//break;
				}
			}
			if (!gestureFound && _currentGestureAnimation != null)
			{
				// Сбрасываем анимацию, если для шага ничего не задано
				ChangeGestureAnimation(_dialogueDefaultAnimationStateName.name);
			}
		}

		if (_dialogueFacialExpressionsDataList.Count > 0)
		{
			bool expressionFound = false;

			for (int i = 0; i < _dialogueFacialExpressionsDataList.Count; i++)
			{
				if ((_currentDialogueStepIndex) == _dialogueFacialExpressionsDataList[i].DialogueStep)
				{
					OnChangeBlendShapeFacialExpression?.Invoke(_dialogueFacialExpressionsDataList[i].FacialExpression.ToString());
					expressionFound = true;
				}
			}
			if (!expressionFound)
			{
				OnResetAllBlendShapesFacialExpressions?.Invoke();
			}
		}
	}

	private void PlayVoiceLineForCurrentStep(LanguagesEnum currentLanguage)
	{
		AudioClip[] currentLanguageVoicelines = null;

		if (currentLanguage == LanguagesEnum.Russian)
		{
			currentLanguageVoicelines = _NPCdialogueData.DialogueVoicelinesRussian;
		}
		else
		{
			currentLanguageVoicelines = _NPCdialogueData.DialogueVoicelinesEnglish;
		}

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
		_textComponentDialogueYes.text = _localizedDialogue[_localizationManager.CurrentLanguage][_currentDialogueStepIndex +1];
		_textComponentDialogueNo.text = _localizedDialogue[_localizationManager.CurrentLanguage][_currentDialogueStepIndex + 2];
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
		_performActionOnNoFinal = false;
		_performActionOnYesFinal = false;

		if (isYesSelected)
		{
			_currentDialogueStepIndex = _dialogueBranchStructsList[_dialogueBranchStructIndex].DialogueBranchLine + 2;

			if (_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnYesAnswer != null)
			{
				_performActionOnYesFinal = true;
			}
		}
		else
		{
			_currentDialogueStepIndex = _dialogueBranchStructsList[_dialogueBranchStructIndex].GoToNoOptionLine - 1;

			if (_dialogueBranchStructsList[_dialogueBranchStructIndex].ActionOnNoAnswer != null)
			{
				_performActionOnNoFinal = true;
			}
		}

			_buttonDialogueYes.onClick.RemoveAllListeners();
		_buttonDialogueNo.onClick.RemoveAllListeners();

		DisplayNextDialogueLine();
		DeactivateButtons();

		_canSkip = true;
	}

	private void ChangeGestureAnimation(string newAnimation, float crossfade = 0.2f)
	{
		if (_currentGestureAnimation != newAnimation)
		{
			_currentGestureAnimation = newAnimation;

			//Debug.Log(newAnimation);
		
			_animator.CrossFade(newAnimation, crossfade);
		}
	}
}