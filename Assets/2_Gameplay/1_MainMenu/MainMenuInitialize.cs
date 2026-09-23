using UnityEngine;

public class MainMenuInitialize : MonoBehaviour
{
    [SerializeField] private MainMenuChooseMissionController _mainMenuChooseMissionController;
    [SerializeField] private MainMenuReadNewsController _readNewsController;
    [SerializeField] private MainMenuCanvasController _canvasController;
    [SerializeField] private MainMenuDiegeticButtonController[] _diegeticButtonController;
	[SerializeField] private CutsceneController _cutsceneNewGame;
	[SerializeField] private Material _diegeticButtonMaterial;
	private Bootstrap _bootstrap;
    void Start()
    {
		_bootstrap = ServiceLocator.Resolve<Bootstrap>();

		_mainMenuChooseMissionController.Initialize(_bootstrap);
		_readNewsController.Initialize(_bootstrap);

		for (int i = 0; i < _diegeticButtonController.Length; i++)
		{
			_diegeticButtonController[i].Initialize(
				_mainMenuChooseMissionController,
				_readNewsController,
				_cutsceneNewGame,
				_diegeticButtonMaterial);
		}

		_canvasController.Initialize(_mainMenuChooseMissionController, _readNewsController);
	}
}
