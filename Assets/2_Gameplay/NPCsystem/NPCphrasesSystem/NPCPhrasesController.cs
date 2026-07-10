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
	private GameObject _playerHead;
	private AudioSource _audioSource;
	private NPCAbstract _NPCabstract;
	private LocalizationManager _localizationManager;
	private Dictionary<LanguagesEnum, List<string>> _localizedNPSphrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
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
		_playerHead = ServiceLocator.Resolve<GameObject>("GameObjectPlayerHead");
		_audioSource = GetComponent<AudioSource>();
		LoadPhrasesTextFiles();
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

	public IEnumerator TemporaryShowPhrases()
	{
		_NPCphrasesText.gameObject.SetActive(true);
		OnStartLookingAtObject?.Invoke(_playerHead);
		var currentLanguage = _localizationManager.CurrentLanguage;
		if (_localizedNPSphrases[currentLanguage].Count > 0)
		{
			int randomIndex = Random.Range(0, _localizedNPSphrases[currentLanguage].Count);
			string selectedPhrase = _localizedNPSphrases[currentLanguage][randomIndex];
			string fullPhrase = $"{_NPCabstract.InteractionObjectNameUI}: {selectedPhrase}";
			_NPCphrasesTextComponent.text = fullPhrase;
		}
		else
		{
			Debug.LogWarning("No phrases available for the selected language!");
		}

		yield return new WaitForSeconds(3.5f);

		_NPCphrasesTextComponent.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
		OnStopLookingAtObject?.Invoke(_playerHead);
	}

	public void ClearPhrases()
	{
		_NPCphrasesTextComponent.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
	}
}
