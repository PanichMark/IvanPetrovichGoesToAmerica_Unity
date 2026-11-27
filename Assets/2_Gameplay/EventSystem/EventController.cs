using UnityEngine;

public class EventController : MonoBehaviour
{
	public bool EventHappened { get; private set; }

	public bool EventWasDisrupted { get; private set; }

	[SerializeField] private bool _NPCeventChangeResponePhrases;

	[SerializeField] private GameObject _eventTriggersList;

	[SerializeField] private bool _includeNPCconversation;
	[SerializeField] private EventNPCconversationData _NPCconversationData;

	[SerializeField] private bool _includeNPCanimationSeries;
	[SerializeField] private EventNPCanimationSeriesData _NPCeventAnimationSeriesData;

	public void Initialize()
	{

	}
}
