using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NPCphrasesController : MonoBehaviour
{
	public delegate void HeadIKHandler(GameObject objectToLookAt);
	public event HeadIKHandler OnStartLookingAtObject;
	public event HeadIKHandler OnStopLookingAtObject;
	private GameObject _playerEyesLookAt;
	private AudioSource _audioSource;
	private NPCabstract _NPCabstract;
	private bool _isPhrasePlaying = false;
	private LocalizationManager _localizationManager;
	private Dictionary<LanguagesEnum, List<string>> _localizedNPSphrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};
	private Dictionary<LanguagesEnum, List<AudioClip>> _localizedVoiceLines = new Dictionary<LanguagesEnum, List<AudioClip>>
	{
		{LanguagesEnum.Russian, new List<AudioClip>() },
		{LanguagesEnum.English, new List<AudioClip>() }
	};

	private GameObject _NPCphrasesText;
	private TextMeshProUGUI _NPCphrasesTextComponent;

	private ViewModelHUDInteraction _viewModelHUDInteraction;

	private NPCphrasesData _NPCphrasesDefaultData;
	private NPCphrasesData _NPCphrasesInterestedData;
	private NPCphrasesData _NPCphrasesAlarmedData;
	private NPCphrasesData _NPCphrasesChasingData;
	private NPCphrasesData _NPCphrasesAttackingData;
	private NPCphrasesData _NPCphrasesSearchingData;
	private NPCphrasesData _NPCphrasesAfraidData;
	private NPCphrasesData _NPCphrasesGreetingData;
	private NPCphrasesData _NPCphrasesPurchaseData;

	private NPCphrasesDataTypes _NPCphrasesType;

	public void InitializePeaceful(
		NPCabstract NPCabstract,
		NPCphrasesDataPeaceful NPCphrasesDataPeaceful)
	{
		_NPCphrasesType = NPCphrasesDataTypes.Peaceful;

		_NPCabstract = NPCabstract;

		_audioSource = GetComponent<AudioSource>();

		_playerEyesLookAt = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.PlayerEyes);

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_NPCphrasesText = _viewModelHUDInteraction.TextPhraseLine;
		_NPCphrasesTextComponent = _NPCphrasesText.GetComponent<TextMeshProUGUI>();

		_NPCphrasesDefaultData = NPCphrasesDataPeaceful.PhrasesDefault;
		_NPCphrasesInterestedData = NPCphrasesDataPeaceful.PhrasesInterested;
		_NPCphrasesAlarmedData = NPCphrasesDataPeaceful.PhrasesAlarmed;
		_NPCphrasesAfraidData = NPCphrasesDataPeaceful.PhrasesAfraid;

		LoadPhrasesTextFiles();
		LoadVoiceLineFiles();
	}

	public void InitializeNonPeaceful(
		NPCabstract NPCabstract,
		NPCphrasesDataNonPeaceful NPCphrasesDataNonPeaceful)
	{
		_NPCphrasesType = NPCphrasesDataTypes.NonPeaceful;

		_NPCabstract = NPCabstract;

		_audioSource = GetComponent<AudioSource>();

		_playerEyesLookAt = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.PlayerEyes);

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_NPCphrasesText = _viewModelHUDInteraction.TextPhraseLine;
		_NPCphrasesTextComponent = _NPCphrasesText.GetComponent<TextMeshProUGUI>();

		_NPCphrasesDefaultData = NPCphrasesDataNonPeaceful.PhrasesDefault;
		_NPCphrasesInterestedData = NPCphrasesDataNonPeaceful.PhrasesInterested;
		_NPCphrasesAlarmedData = NPCphrasesDataNonPeaceful.PhrasesAlarmed;
		_NPCphrasesChasingData = NPCphrasesDataNonPeaceful.PhrasesChasing;
		_NPCphrasesAttackingData = NPCphrasesDataNonPeaceful.PhrasesAttacking;
		_NPCphrasesSearchingData = NPCphrasesDataNonPeaceful.PhrasesSeaching;

		LoadPhrasesTextFiles();
		LoadVoiceLineFiles();
	}

	public void InitializeAutomaton(
		NPCabstract NPCabstract,
		NPCphrasesDataAutomaton NPCphrasesDataAutomaton)
	{
		_NPCphrasesType = NPCphrasesDataTypes.Automaton;

		_NPCabstract = NPCabstract;

		_audioSource = GetComponent<AudioSource>();

		_playerEyesLookAt = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.PlayerEyes);

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_NPCphrasesText = _viewModelHUDInteraction.TextPhraseLine;
		_NPCphrasesTextComponent = _NPCphrasesText.GetComponent<TextMeshProUGUI>();

		_NPCphrasesGreetingData = NPCphrasesDataAutomaton.PhrasesGreeting;
		_NPCphrasesPurchaseData = NPCphrasesDataAutomaton.PhrasesPurchase;
	}

	private void LoadVoiceLineFiles()
	{
		if (_NPCphrasesDefaultData != null)
		{
			if (_NPCphrasesDefaultData.PhrasesVoicelinesRussian != null && _NPCphrasesDefaultData.PhrasesVoicelinesRussian.Length > 0)
			{
				foreach (var clip in _NPCphrasesDefaultData.PhrasesVoicelinesRussian)
				{
					if (clip != null)
					{
						_localizedVoiceLines[LanguagesEnum.Russian].Add(clip);
					}
				}
			}
			else if (_NPCphrasesDefaultData.PhrasesVoicelinesEnglish != null && _NPCphrasesDefaultData.PhrasesVoicelinesEnglish.Length > 0)
			{
				Debug.LogWarning("Russian voice lines array is not assigned!");
			}

			if (_NPCphrasesDefaultData.PhrasesVoicelinesEnglish != null && _NPCphrasesDefaultData.PhrasesVoicelinesEnglish.Length > 0)
			{
				foreach (var clip in _NPCphrasesDefaultData.PhrasesVoicelinesEnglish)
				{
					if (clip != null)
					{
						_localizedVoiceLines[LanguagesEnum.English].Add(clip);
					}
				}
			}
			else if (_NPCphrasesDefaultData.PhrasesVoicelinesRussian != null && _NPCphrasesDefaultData.PhrasesVoicelinesRussian.Length > 0)
			{
				Debug.LogWarning("English voice lines array is not assigned!");
			}
		}
		else
		{
			Debug.LogWarning("Default Phrases not assigned");
		}
	}

	private void LoadPhrasesTextFiles()
	{
		if (_NPCphrasesDefaultData.PhrasesTextFileRussian != null)
		{
			using (var reader = new StringReader(_NPCphrasesDefaultData.PhrasesTextFileRussian.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						_localizedNPSphrases[LanguagesEnum.Russian].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("Russian phrase file is not assigned!");
		}

		if (_NPCphrasesDefaultData.PhrasesTextFileEnglish != null)
		{
			using (var reader = new StringReader(_NPCphrasesDefaultData.PhrasesTextFileEnglish.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						_localizedNPSphrases[LanguagesEnum.English].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("English phrase file is not assigned!");
		}
		
	}

	public void TemporaryShowPhrases()
	{
		if (_isPhrasePlaying)
		{
			return;
		}

		StartCoroutine(InternalTemporaryShowPhrases());
	}

	private IEnumerator InternalTemporaryShowPhrases()
	{
		_isPhrasePlaying = true;
		_NPCphrasesText.gameObject.SetActive(true);
		OnStartLookingAtObject?.Invoke(_playerEyesLookAt);
		var currentLanguage = _localizationManager.CurrentLanguage;
		int phraseIndex = -1;

		if (_localizedNPSphrases[currentLanguage].Count > 0)
		{
			phraseIndex = Random.Range(0, _localizedNPSphrases[currentLanguage].Count);
			string selectedPhrase = _localizedNPSphrases[currentLanguage][phraseIndex];
			string fullPhrase = $"{_NPCabstract.InteractionObjectNameUI}: {selectedPhrase}";
			_NPCphrasesTextComponent.text = fullPhrase;
		}
		else
		{
			Debug.LogWarning("No phrases available for the selected language!");
		}

		if (phraseIndex != -1 && _localizedVoiceLines[currentLanguage].Count > phraseIndex)
		{
			AudioClip clipToPlay = _localizedVoiceLines[currentLanguage][phraseIndex];
			_audioSource.clip = clipToPlay;
			_audioSource.Play();
			yield return new WaitWhile(() => _audioSource.isPlaying);
		}
		else
		{
			yield return new WaitForSeconds(3f);
		}

		_NPCphrasesTextComponent.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
		OnStopLookingAtObject?.Invoke(_playerEyesLookAt);
		_isPhrasePlaying = false;
	}

	public void ClearPhrases()
	{
		_NPCphrasesTextComponent.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
	}
}
