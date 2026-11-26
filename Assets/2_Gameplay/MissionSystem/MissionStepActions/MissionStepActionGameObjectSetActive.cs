using UnityEngine;

public class MissionStepActionGameObjectSetActive : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private bool _enable;

	private void Start()
	{
		if (_linkedMissionStep != null)
		{
			_linkedMissionStep.RegisterAction(this);
		}
	}

	public void PerformAction()
	{
		gameObject.SetActive(_enable);
	}
}