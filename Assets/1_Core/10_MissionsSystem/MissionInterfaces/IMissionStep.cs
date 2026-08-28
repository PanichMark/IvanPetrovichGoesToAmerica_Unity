// Файл: ICurrentMissionStep.cs
// Поместите этот файл в папку внутри сборки Core

using System.Collections.Generic;

public interface IMissionStep
{
	// Возвращает список условий текущего шага
	bool ShowMissionMarker { get; }
	IReadOnlyList<IMissionStepCondition> Conditions { get; }
}