using System;

public interface IMissionStepConditionWithProgress
{
	event Action<int, int> OnStepConditionProgressUpdated;
}