using UnityEngine;

public sealed class EventBusSubscriberExample
{
	private EventBus _eventBus;

	public void Initialize(EventBus eventBus)
	{
		if (_eventBus != null)
		{
			Unsubscribe();
		}

		_eventBus = eventBus;
		if (_eventBus == null)
		{
			return;
		}

		_eventBus.Subscribe(PlayerSystemsEvents.PlayerArmed, HandlePlayerArmed);
		_eventBus.Subscribe(MissionsSystemEvents.CurrentStepChanged, HandleMissionStepChanged);
	}

	public void Unsubscribe()
	{
		if (_eventBus == null)
		{
			return;
		}

		_eventBus.Unsubscribe(PlayerSystemsEvents.PlayerArmed, HandlePlayerArmed);
		_eventBus.Unsubscribe(MissionsSystemEvents.CurrentStepChanged, HandleMissionStepChanged);
		_eventBus = null;
	}

	private void HandlePlayerArmed()
	{
		Debug.Log("EventBus example received PlayerArmed.");
	}

	private void HandleMissionStepChanged()
	{
		Debug.Log("EventBus example received CurrentStepChanged.");
	}
}