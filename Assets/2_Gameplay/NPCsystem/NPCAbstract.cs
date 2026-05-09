using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public abstract class NPCAbstract : MonoBehaviour, IInteractable, IDamageable
{
	private float NPCMaxHealth;
	[SerializeField][Min(0)] private float NPCCurrentHealth;
	public bool IsNPCdead => NPCCurrentHealth <= 0;
	[SerializeField] protected string NPC_name;

	private Dictionary<LanguagesEnum, List<string>> localizedPhrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};

	protected NPCDialogueController _NPCDialogueController;
	private TextMeshProUGUI NPCphrasesText;
	[SerializeField] private TextAsset russianPhraseFile;
	[SerializeField] private TextAsset englishPhraseFile;
	private LocalizationManager localizationManager;
	protected NPCStateMachineController _npcStateMachineController;

	public string InteractionObjectNameSystem => NPC_name;
	public string InteractionObjectNameUI => localizationManager.GetLocalizedString(NPC_name);
	public string InteractionHintMessageMain => $"Talk to {InteractionObjectNameUI}";
	public string InteractionHintMessageAdditional => throw new System.NotImplementedException();
	public virtual bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction { get; protected set; }

	public bool WasObjectDestroyed => throw new System.NotImplementedException();

	public float Health => NPCCurrentHealth;

	private void Start()
	{
		NPCMaxHealth = NPCCurrentHealth;
		NPCphrasesText = ServiceLocator.Resolve<TextMeshProUGUI>("NPCphrases");
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_NPCDialogueController = GetComponent<NPCDialogueController>();

		LoadPhrasesFromFiles();
		_npcStateMachineController = GetComponent<NPCStateMachineController>();

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
		}
	}

	private void LoadPhrasesFromFiles()
	{
		if (russianPhraseFile != null)
		{
			using (var reader = new StringReader(russianPhraseFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedPhrases[LanguagesEnum.Russian].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("Russian phrase file is not assigned!");
		}

		if (englishPhraseFile != null)
		{
			using (var reader = new StringReader(englishPhraseFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedPhrases[LanguagesEnum.English].Add(line.Trim());
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
		NPCphrasesText.gameObject.SetActive(true);

		var currentLanguage = localizationManager.CurrentLanguage;
		if (localizedPhrases[currentLanguage].Count > 0)
		{
			int randomIndex = Random.Range(0, localizedPhrases[currentLanguage].Count);
			string selectedPhrase = localizedPhrases[currentLanguage][randomIndex];
			string fullPhrase = $"{InteractionObjectNameUI}: {selectedPhrase}";
			NPCphrasesText.text = fullPhrase;
		}
		else
		{
			Debug.LogWarning("No phrases available for the selected language!");
		}

		yield return new WaitForSeconds(3.5f);

		NPCphrasesText.text = string.Empty;
		NPCphrasesText.gameObject.SetActive(false);
	}

	public virtual void Interact()
	{
	}

	public void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";
		enabled = false;
		gameObject.AddComponent<Rigidbody>();
		InteractionObjectPickable.CreateWithName(gameObject, NPC_name);
		Destroy(this);
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {Health - amount}");
		NPCCurrentHealth -= amount;

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
		}
	}

	public void SetHealthToZero()
	{
		NPCCurrentHealth = 0;
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{NPC_name} is Dead");

		StopAllCoroutines();
		ConvertToPickableObject();

		NPCphrasesText.text = string.Empty;
		NPCphrasesText.gameObject.SetActive(false);

		_npcStateMachineController.SetNPCState(NPCStateTypes.Dead);
	}
}