using System;

[Serializable]
public struct NPCDialogueBranch
{
	public int DialogueBranchIndex;       // Номер строки диалога
	public int GoToYesOptionIndex;        // Индекс ветки при выборе "Да" (-1, если нет)
	public int GoToNoOptionIndex;         // Индекс ветки при выборе "Нет" (-1, если нет)
	public int FinalNoIndex;              // Индекс, при котором срабатывает финальный переход по "Нет"
	public int GoToNoFinal;               // Индекс, на который перейти при достижении FinalNoIndex
}