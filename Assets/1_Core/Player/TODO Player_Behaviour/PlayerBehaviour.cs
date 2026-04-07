using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
	private IInputDevice inputDevice;

	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; } = false;

	// Делегаты для уведомлений
	public delegate void OnPlayerEventHandler();
	public event OnPlayerEventHandler OnPlayerArmed;
	public event OnPlayerEventHandler OnPlayerDisarmed;
	private bool _isInitialized = false;
	void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
		if (inputDevice.GetKeyHideWeapons())
		{
			DisarmPlayer();
		}
	}

	public void ArmPlayer()
	{
		if (!IsPlayerArmed)
		{
			IsPlayerArmed = true;
			WasPlayerArmed = false;

			// Вызвать событие OnPlayerArmed
			OnPlayerArmed?.Invoke();

			Debug.Log("Player Armed");
		}
	}

	public void DisarmPlayer()
	{
		if (IsPlayerArmed)
		{
			IsPlayerArmed = false;
			WasPlayerArmed = true;

			// Вызвать событие OnPlayerDisarmed
			OnPlayerDisarmed?.Invoke();

			Debug.Log("Player Disarmed");
		}
		else
		{
			WasPlayerArmed = false;
		}
	}

	public void Initialize(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
		_isInitialized = true;
		Debug.Log("PlayerBehaviour Initialized");
	}
}
