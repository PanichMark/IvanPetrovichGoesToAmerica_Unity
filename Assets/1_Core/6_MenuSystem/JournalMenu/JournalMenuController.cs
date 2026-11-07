using UnityEngine;

public class JournalMenuController : MonoBehaviour
{
	private GameObject _canvasJournalMenu;
	private ViewModelJournalMenu _viewModelJournalMenu;
	private MenuManager _menuManager;

	public void Initialize(
		MenuManager menuManager,
		GameObject canvasJournalMenu,
		ViewModelJournalMenu viewModelJournalMenu)
	{
		_menuManager = menuManager;
		_canvasJournalMenu = canvasJournalMenu;
		_viewModelJournalMenu = viewModelJournalMenu;

		_menuManager.OnOpenJournalMenu += ShowJournalMenu;
		_menuManager.OnCloseJournalMenu += HideJournalMenu;
	}

	public void ShowJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(true);
	}
	public void HideJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(false);
	}
}
