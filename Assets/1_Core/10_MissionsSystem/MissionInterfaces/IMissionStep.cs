// Файл: ICurrentMissionStep.cs
// Поместите этот файл в папку внутри сборки Core

using System.Collections.Generic;
using UnityEngine;
public interface IMissionStep
{
	// Возвращает список условий текущего шага
	bool ShowMissionMarker { get; }
	IReadOnlyList<IMissionStepCondition> Conditions { get; }

	void Initialize(MissionsManager missionsManager);
	//void OnMissionStepStartDoSomwthing();

	void RegisterObjectsTurnOn(GameObject objectTurnOn);
	void RegisterObjectsTurnOff(GameObject objectTurnOff);
}