using UnityEngine;

public class NPCeventController : MonoBehaviour
{
	public bool NPCeventHappened { get; private set; }

	public bool NPCeventWasDisrupted { get; private set; }

	[SerializeField] private bool _NPCeventChangeResponePhrases;
	[SerializeField] private bool _NPCconversation;
	[SerializeField] private bool _NPCanimationSeries;

	public void Initialize()
	{

	}
}
