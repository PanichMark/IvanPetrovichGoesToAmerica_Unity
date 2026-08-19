using UnityEngine;
using UnityEngine.SceneManagement;

public class TESTdealDamageToPlayer : MonoBehaviour
{
	private PlayerHealthController _playerResourcesHealthManager;

	private void Start()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerHealthController>("PlayerResourcesHealthManager");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T) && SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_0_MainMenu.ToString())
		{
			_playerResourcesHealthManager.TakeDamage(99999);
		}
	}
}