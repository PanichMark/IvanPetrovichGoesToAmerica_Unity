using UnityEngine;

public class PlayerBehaviour: MonoBehaviour
{
	private IInputDevice inputDevice;


	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; }

	


	void Update()
	{

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
	
			Debug.Log("PlayerArmed");
		}
		
		
	}

	public void DisarmPlayer()
	{
		if (IsPlayerArmed)
		{
			IsPlayerArmed = false;

			
			
			WasPlayerArmed = true;

		
			Debug.Log("PlayerDisarmed");
		}
		else WasPlayerArmed = false;
		
	
	}


	public void Initialize(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
		Debug.Log("PlayerBehaviour Initialized");
	}
}

