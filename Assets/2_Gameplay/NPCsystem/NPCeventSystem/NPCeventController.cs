using UnityEngine;

public class NPCeventController : MonoBehaviour
{
	public bool NPCeventHappened { get; private set; }

	public bool NPCeventWasDisrupted { get; private set; }

	[SerializeField] private bool _NPCeventChangeResponePhrases;

	[SerializeField] private GameObject _eventTriggersList;

	[SerializeField] private bool _includeNPCconversation;
	[SerializeField] private NPCeventConversationData _NPCconversationData;

	[SerializeField] private bool _includeNPCanimationSeries;
	[SerializeField] private NPCeventAnimationSeriesData _NPCeventAnimationSeriesData;

	public void Initialize()
	{

	}
}
