using System;

public interface IMissionStepConditionWithCountdown
{
	event Action<float> OnStepConditionTimeTick;
}