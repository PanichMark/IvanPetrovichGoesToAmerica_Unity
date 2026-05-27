using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NPCPhrasesController : MonoBehaviour
{
	private NPCAbstract _NPCabstract;
	private LocalizationManager _localizationManager;
	private Dictionary<LanguagesEnum, List<string>> _localizedNPSphrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};

	[SerializeField] private TextAsset _russianPhraseFile;
	[SerializeField] private TextAsset _englishPhraseFile;
	private TextMeshProUGUI _NPCphrasesText;

	public void Initialize()
	{
		_NPCabstract = GetComponent<NPCAbstract>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_NPCphrasesText = ServiceLocator.Resolve<TextMeshProUGUI>("TextPhraseLine");
		//Debug.Log(_NPCphrasesText);
		LoadPhrasesTextFiles();
	}

	private void LoadPhrasesTextFiles()
	{
		if (_russianPhraseFile != null)
		{
			using (var reader = new StringReader(_russianPhraseFile.text))
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

		if (_englishPhraseFile != null)
		{
			using (var reader = new StringReader(_englishPhraseFile.text))
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

		var currentLanguage = _localizationManager.CurrentLanguage;
		if (_localizedNPSphrases[currentLanguage].Count > 0)
		{
			int randomIndex = Random.Range(0, _localizedNPSphrases[currentLanguage].Count);
			string selectedPhrase = _localizedNPSphrases[currentLanguage][randomIndex];
			string fullPhrase = $"{_NPCabstract.InteractionObjectNameUI}: {selectedPhrase}";
			_NPCphrasesText.text = fullPhrase;
		}
		else
		{
			Debug.LogWarning("No phrases available for the selected language!");
		}

		yield return new WaitForSeconds(3.5f);

		_NPCphrasesText.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
	}

	public void ClearPhrases()
	{
		_NPCphrasesText.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);
	}
}
