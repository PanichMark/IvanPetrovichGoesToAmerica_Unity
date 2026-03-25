using System;

[Serializable]
public struct NPCDialogueBranch
{
	public int DialogueBranchIndex;      
	public int GoToYesOptionIndex;      
	public int GoToNoOptionIndex;        
	public int FinalNoIndex;             
	public int GoToNoFinal;             
}