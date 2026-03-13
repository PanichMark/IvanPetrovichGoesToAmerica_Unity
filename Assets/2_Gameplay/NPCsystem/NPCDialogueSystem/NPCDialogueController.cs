using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCDialogueController : MonoBehaviour
{
	[SerializeField] private TextAsset russianDialogueFile;
	[SerializeField] private TextAsset englishDialogueFile;
	private MenuManager menuManager;
	public TextAsset RussianDialogueFile => russianDialogueFile;
	public TextAsset EnglishDialogueFile => englishDialogueFile;
	// Словарь для фраз на разных языках
	private Dictionary<LanguagesEnum, List<string>> localizedDialogue = new Dictionary<LanguagesEnum, List<string>>
	{
		{LanguagesEnum.Russian, new List<string>() },
		{LanguagesEnum.English, new List<string>() }
	};
	// Публичное свойство, дающее доступ к приватному словарю только для чтения
	public Dictionary<LanguagesEnum, List<string>> LocalizedDialogue => localizedDialogue;
	private GameObject canvasDialogueMenu;
	private GameSceneManager gameSceneManager;
	public bool IsDialogueActive { get; private set; }
	private void Start()
	{
		LoadDialogueFromFiles();
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasDialogueMenu = ServiceLocator.Resolve<GameObject>("CanvasDialogueMenu");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");

		// Связываем управление панелями меню с открытием и закрытием паузы
		menuManager.OnOpenPauseMenu += HideNPCDialogueCanvas;
		menuManager.OnClosePauseMenu += ShowNPCDialogueCanvas;

		// Закрываем головоломку при смене сцены
		gameSceneManager.OnBeginLoadMainMenuScene += ExitNPCDialogue;
		gameSceneManager.OnBeginLoadGameplayScene += ExitNPCDialogue;

	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(1) && IsDialogueActive)
		{
			Debug.Log("EXIT!");
			ExitNPCDialogue();
		}
	}
	private void ExitNPCDialogue()
	{
		menuManager.CloseDialogueMenu();
		HideNPCDialogueCanvas();
		IsDialogueActive = false;

	}
	private void LoadDialogueFromFiles()
	{
		// Русские фразы
		if (russianDialogueFile != null)
		{
			using (var reader = new StringReader(russianDialogueFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedDialogue[LanguagesEnum.Russian].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			if (LocalizedDialogue.Count > 0)
			Debug.LogWarning("Русская версия диалога не указана!");
		}

		// Английские фразы
		if (englishDialogueFile != null)
		{
			using (var reader = new StringReader(englishDialogueFile.text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (!string.IsNullOrWhiteSpace(line))
					{
						localizedDialogue[LanguagesEnum.English].Add(line.Trim());
					}
				}
			}
		}
		else
		{
			if (LocalizedDialogue.Count > 0)
			Debug.LogWarning("Английская версия диалога не указана!");
		}
	}
	public void ShowNPCDialogueCanvas()
	{
		if (IsDialogueActive)
		canvasDialogueMenu.SetActive(true);
	}
	private void HideNPCDialogueCanvas()
	{
		if (IsDialogueActive)
		canvasDialogueMenu.SetActive(false);
	}
	public void Interact()
	{
		Debug.Log("DIALOGUE!");
		menuManager.OpenDialoguenMenu();
		IsDialogueActive = true;
		ShowNPCDialogueCanvas();
	}

}