public class InteractionObjectTVcontroller : InteractionObjectTVabstract, IElectroShockable
{
	public delegate void TVstateChangedHandler(bool isOn);
	public event TVstateChangedHandler OnTVstateChanged;

	public void TogglePower()
	{
		if (IsTVturnedOn)
		{
			TurnOff();
		}
		else
		{
			TurnOn();
		}
	}

	protected override void TurnOn()
	{
		IsTVturnedOn = true;
		SetScreenActive(true);
		PlayChannel(_currentChannelIndex);
		OnTVstateChanged?.Invoke(IsTVturnedOn);
	}

	protected override void TurnOff()
	{
		IsTVturnedOn = false;
		_videoPlayer.Stop();
		SetScreenActive(false);
		OnTVstateChanged?.Invoke(IsTVturnedOn);
	}

	public void SwitchChannel(bool isNext)
	{
		if (!IsTVturnedOn) return;
		SwitchChannelInternal(isNext);
	}

	public void Electrify(float damage)
	{
		if (!IsTVturnedOn)
		{
			TurnOn();
		}
		else
		{
			SwitchChannel(true);
		}
	}
}