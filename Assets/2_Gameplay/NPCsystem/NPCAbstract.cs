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

	// Словарь для фраз на разных языках
	private Dictionary<LanguagesEnum, List<string>> localizedPhrases = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};
	protected NPCDialogueController _NPCDialogueController;
	private TextMeshProUGUI NPCphrasesText;
	// Два слота для русского и английского файлов фраз
	[SerializeField] private TextAsset russianPhraseFile;
	[SerializeField] private TextAsset englishPhraseFile;
	private LocalizationManager localizationManager;
	protected NPCStateMachineController _npcStateMachineController;

	public string InteractionObjectNameSystem => NPC_name;
	public string InteractionObjectNameUI => localizationManager.GetLocalizedString(NPC_name);
	public string InteractionHintMessageMain => $"Поговорить с {InteractionObjectNameUI}";
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


		LoadPhrasesFromFiles(); // Читаем фразы из выбранных файлов
		_npcStateMachineController = GetComponent<NPCStateMachineController>();

		if (IsNPCdead)
		{
			ObjectIsFullyDamaged();
		}
	}

	// Метод загрузки фраз из обоих файлов
	private void LoadPhrasesFromFiles()
	{
		// Русские фразы
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
			Debug.LogWarning("Русская версия фраз не указана!");
		}

		// Английские фразы
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
			Debug.LogWarning("Английская версия фраз не указана!");
		}
	}

	// Метод выбора случайной фразы
	protected IEnumerator ShowAndHidePhrase()
	{
		// Активация объекта с текстом
		NPCphrasesText.gameObject.SetActive(true);

		// Получить текущую фразу и сформировать полный текст с именем NPC
		var currentLanguage = localizationManager.CurrentLanguage;
		if (localizedPhrases[currentLanguage].Count > 0)
		{
			int randomIndex = Random.Range(0, localizedPhrases[currentLanguage].Count);
			string selectedPhrase = localizedPhrases[currentLanguage][randomIndex];
			string fullPhrase = $"{InteractionObjectNameUI}: {selectedPhrase}"; // Объединяем имя NPC и фразу
			NPCphrasesText.text = fullPhrase;
		}
		else
		{
			Debug.LogWarning("Нет фраз для выбранного языка!");
		}

		
		yield return new WaitForSeconds(3.5f);

		// Очистка текста и скрытие объекта
		NPCphrasesText.text = string.Empty;
		NPCphrasesText.gameObject.SetActive(false);
	}

	public virtual void Interact()
	{

	}

	// Преобразование в пассивный объект
	public void ConvertToPickableObject()
	{
		gameObject.tag = "Interactable";
		enabled = false;
		gameObject.AddComponent<Rigidbody>();
		InteractionObjectPickable.CreateWithName(gameObject, NPC_name);
		Destroy(this);
	}

	// Повреждение NPC
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