using System;

[Serializable]
public struct NPCDialogueBranch
{
	public int LineNumber;       // Номер строки диалога (указывается вручную, начиная с 1)
	public int YesOptionIndex;   // Индекс ветки, если выбрали "Да" (-1, если продолжения нет)
	public int NoOptionIndex;    // Индекс ветки, если выбрали "Нет" (-1, если продолжения нет)
}