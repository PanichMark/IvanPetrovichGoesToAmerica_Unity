using System;
using UnityEngine;

[Serializable]
public struct NPCdialogueBranchData
{
	public int DialogueBranchLine;      
	public int GoToNoOptionLine;        
	public int FinalYesLine;
	public int GoToYesFinalLine;
	public string YesOptionAnswer;
	public string NoOptionAnswer;
	public GameObject ActionOnYesAnswer;
	public GameObject ActionOnNoAnswer;
}