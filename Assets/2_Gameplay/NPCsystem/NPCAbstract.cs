using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public abstract class NPCAbstract : MonoBehaviour, IInteractable, IDamageable
{
	[SerializeField] protected string _NPCname;

	[SerializeField] NPCconfigBodyType _NPCconfigBodyType;

	[SerializeField] NPCconfigHealth _NPCconfigHealth;
	public bool IsNPCdead => _NPCconfigHealth.NPCCurrentHealth <= 0;


	private Dictionary<LanguagesEnum, List<string>> _localizedNPSphrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};

	protected NPCDialogueController _NPCdialogueController;
	private TextMeshProUGUI _NPCphrasesText;
	[SerializeField] private TextAsset _russianPhraseFile;
	[SerializeField] private TextAsset _englishPhraseFile;
	private LocalizationManager _localizationManager;
	protected NPCStateMachineController _npcStateMachineController;

	public string InteractionObjectNameSystem => _NPCname;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_NPCname);
	public string InteractionHintMessageMain => $"Talk to {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => throw new System.NotImplementedException();
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	public bool WasObjectDestroyed => throw new System.NotImplementedException();

	public float CurrentHealth => _NPCconfigHealth.NPCCurrentHealth;

	private void Start()
	{
		_NPCphrasesText = ServiceLocator.Resolve<TextMeshProUGUI>("TextPhraseLine");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_NPCdialogueController = GetComponent<NPCDialogueController>();

		LoadPhrasesFromFiles();
		_npcStateMachineController = GetComponent<NPCStateMachineController>();

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
		}
	}

	private void LoadPhrasesFromFiles()
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

	protected IEnumerator ShowAndHidePhrase()
	{
		_NPCphrasesText.gameObject.SetActive(true);

		var currentLanguage = _localizationManager.CurrentLanguage;
		if (_localizedNPSphrases[currentLanguage].Count > 0)
		{
			int randomIndex = Random.Range(0, _localizedNPSphrases[currentLanguage].Count);
			string selectedPhrase = _localizedNPSphrases[currentLanguage][randomIndex];
			string fullPhrase = $"{InteractionObjectNameUI}: {selectedPhrase}";
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

	public virtual void Interact()
	{

	}

	public void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";
		enabled = false;
		gameObject.AddComponent<Rigidbody>();
		InteractionObjectPickable.CreateWithName(gameObject, _NPCname);
		Destroy(this);
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");
		_NPCconfigHealth.NPCCurrentHealth -= amount;

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
		}
	}

	public void SetHealthToZero()
	{
		_NPCconfigHealth.NPCCurrentHealth = 0;
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{_NPCname} is Dead");

		StopAllCoroutines();
		ConvertToPickableObject();

		_NPCphrasesText.text = string.Empty;
		_NPCphrasesText.gameObject.SetActive(false);

		_npcStateMachineController.SetNPCState(NPCStateTypes.Dead);
	}
}