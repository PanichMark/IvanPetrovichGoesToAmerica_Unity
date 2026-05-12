using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
	private IInputDevice _inputDevice;

	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; } = false;

	public delegate void OnPlayerEventHandler();
	public event OnPlayerEventHandler OnPlayerArmed;
	public event OnPlayerEventHandler OnPlayerDisarmed;
	private bool _isInitialized = false;
	void Update()
	{
		if (!_isInitialized)
			return;
		if (_inputDevice.GetKeyHideWeapons())
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
		this._inputDevice = inputDevice;
		_isInitialized = true;
		Debug.Log("PlayerBehaviour Initialized");
	}
}