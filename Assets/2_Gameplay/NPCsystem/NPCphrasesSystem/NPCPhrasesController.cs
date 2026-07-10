using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NPCPhrasesController : MonoBehaviour
{
	public delegate void HeadIKHandler(GameObject objectToLookAt);
	public event HeadIKHandler OnStartLookingAtObject;
	public event HeadIKHandler OnStopLookingAtObject;
	private GameObject _playerEyesLookAt;
	private AudioSource _audioSource;
	private NPCAbstract _NPCabstract;
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

	[SerializeField] private NPCPhrasesData _NPCphrasesData;
	private GameObject _NPCphrasesText;
	private TextMeshProUGUI _NPCphrasesTextComponent;

	public void Initialize()
	{
		_NPCabstract = GetComponent<NPCAbstract>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_NPCphrasesText = ServiceLocator.Resolve<GameObject>("TextPhraseLine");
		_NPCphrasesTextComponent = _NPCphrasesText.GetComponent<TextMeshProUGUI>();
		//Debug.Log(_NPCphrasesText);
		_playerEyesLookAt = ServiceLocator.Resolve<GameObject>("GameObjectPlayerEyesLookAt");
		_audioSource = GetComponent<AudioSource>();
		LoadPhrasesTextFiles();
		LoadVoiceLineFiles();
	}

	private void LoadVoiceLineFiles()
	{
		if (_NPCphrasesData.PhrasesVoicelinesRussian != null && _NPCphrasesData.PhrasesVoicelinesRussian.Length > 0)
		{
			foreach (var clip in _NPCphrasesData.PhrasesVoicelinesRussian)
			{
				if (clip != null)
				{
					_localizedVoiceLines[LanguagesEnum.Russian].Add(clip);
				}
			}
		}
		else if (_NPCphrasesData.PhrasesVoicelinesEnglish != null && _NPCphrasesData.PhrasesVoicelinesEnglish.Length > 0)
		{
			Debug.LogWarning("Russian voice lines array is not assigned!");
		}

		if (_NPCphrasesData.PhrasesVoicelinesEnglish != null && _NPCphrasesData.PhrasesVoicelinesEnglish.Length > 0)
		{
			foreach (var clip in _NPCphrasesData.PhrasesVoicelinesEnglish)
			{
				if (clip != null)
				{
					_localizedVoiceLines[LanguagesEnum.English].Add(clip);
				}
			}
		}
		else if (_NPCphrasesData.PhrasesVoicelinesRussian != null && _NPCphrasesData.PhrasesVoicelinesRussian.Length > 0)
		{
			Debug.LogWarning("English voice lines array is not assigned!");
		}
	}

	private void LoadPhrasesTextFiles()
	{
		if (_NPCphrasesData.PhrasesFileRussian != null)
		{
			using (var reader = new StringReader(_NPCphrasesData.PhrasesFileRussian.text))
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

		if (_NPCphrasesData.PhrasesFileEnglish != null)
		{
			using (var reader = new StringReader(_NPCphrasesData.PhrasesFileEnglish.text))
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
			yield return new WaitForSeconds(3.5f);
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
