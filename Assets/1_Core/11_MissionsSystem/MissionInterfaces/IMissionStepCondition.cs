// Файл: ICurrentMissionCondition.cs
// Поместите этот файл в папку внутри сборки Core

using UnityEngine;

public interface IMissionStepCondition
{
	// Проверяет, выполнено ли условие
	bool IsMet();

	// Позволяет получить владельца условия (объект в мире)
	GameObject Owner { get; }

	void ResetStepConditionMetStateInEditMode();
}