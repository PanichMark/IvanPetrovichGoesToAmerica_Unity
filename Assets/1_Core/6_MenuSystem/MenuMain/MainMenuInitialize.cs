using UnityEngine;

public class MainMenuInitialize : MonoBehaviour
{
    [SerializeField] private MainMenuChooseMissionController _mainMenuChooseMissionController;
    [SerializeField] private MainMenuReadNewsController _readNewsController;
    [SerializeField] private MainMenuCanvasController _canvasController;
    [SerializeField] private MainMenuDiegeticButtonController[] _diegeticButtonController;
	[SerializeField] private Material _diegeticButtonMaterial;

    void Start()
    {
		_mainMenuChooseMissionController.Initialize();
		_readNewsController.Initialize();

		for (int i = 0; i < _diegeticButtonController.Length; i++)
		{
			_diegeticButtonController[i].Initialize(_mainMenuChooseMissionController, _readNewsController, _diegeticButtonMaterial);
		}

		_canvasController.Initialize(_mainMenuChooseMissionController, _readNewsController);
	}
}
