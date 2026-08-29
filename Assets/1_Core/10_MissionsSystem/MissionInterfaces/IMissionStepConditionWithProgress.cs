using System;

public interface IMissionStepConditionWithProgress : IMissionStepCondition
{
	event Action<int, int> OnStepConditionProgressUpdated;
}