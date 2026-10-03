public sealed class EventBusPublisherExample
{
	private EventBus _eventBus;

	public void Initialize(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void PublishExampleEvents()
	{
		if (_eventBus == null)
		{
			return;
		}

		_eventBus.Publish(PlayerSystemsEvents.PlayerArmed);
		_eventBus.Publish(MissionsSystemEvents.CurrentStepChanged);
	}
}