using System;
using UnityEngine;

[Serializable]
public struct NPCDialogueBranch
{
	public int DialogueBranchLine;      
	public int GoToNoOptionLine;        
	public int FinalYesLine;
	public int GoToYesFinalLine;
	public GameObject ActionOnYesAnswer;
	public GameObject ActionOnNoAnswer;
}