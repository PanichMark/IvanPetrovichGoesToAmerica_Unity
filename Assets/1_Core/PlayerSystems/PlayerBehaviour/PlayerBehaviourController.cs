using UnityEngine;

public class PlayerBehaviourController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;

	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; } = false;

	public delegate void OnPlayerEventHandler();
	public event OnPlayerEventHandler OnPlayerArmed;
	public event OnPlayerEventHandler OnPlayerDisarmed;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;

		Debug.Log("PlayerBehaviourController Initialized");
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyHideWeapons())
		{
			DisarmPlayer();
		}
	}

	public void ArmPlayer()
	{
	//	Debug.Log("ARM!!!!");
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
		//Debug.Log("DISARM!!!!");

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
}