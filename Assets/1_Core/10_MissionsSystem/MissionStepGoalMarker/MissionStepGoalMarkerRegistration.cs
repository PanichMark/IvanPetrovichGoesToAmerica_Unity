using UnityEngine;

public class MissionStepGoalMarkerRegistration : MonoBehaviour
{
    [SerializeField] private MissionStep _missionStepGoalMarker;

    void Start()
    {
        if (_missionStepGoalMarker.ShowMissionMarker)
        {
            _missionStepGoalMarker.RegisterMissionStepGoalMarkerObject(gameObject);
        }
    }
}