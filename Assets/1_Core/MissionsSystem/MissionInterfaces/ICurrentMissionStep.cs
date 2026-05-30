// Файл: ICurrentMissionStep.cs
// Поместите этот файл в папку внутри сборки Core

using System.Collections.Generic;

public interface ICurrentMissionStep
{
	// Возвращает список условий текущего шага
	IReadOnlyList<ICurrentMissionCondition> Conditions { get; }
}