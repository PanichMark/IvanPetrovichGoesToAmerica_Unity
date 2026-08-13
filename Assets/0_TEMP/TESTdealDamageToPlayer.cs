using UnityEngine;
using UnityEngine.SceneManagement;

public class TESTdealDamageToPlayer : MonoBehaviour
{
	private PlayerResourcesHealthManager _playerResourcesHealthManager;

	private void Start()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T) && SceneManager.GetSceneAt(1).name != GameScenesEnum.Scene_0_MainMenu.ToString())
		{
			_playerResourcesHealthManager.TakeDamage(99999);
		}
	}
}