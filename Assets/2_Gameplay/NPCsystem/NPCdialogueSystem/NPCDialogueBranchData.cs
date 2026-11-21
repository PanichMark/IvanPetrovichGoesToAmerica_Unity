using System;
using UnityEngine;

[Serializable]
public struct NPCdialogueBranchData
{
	public int DialogueBranchLine;      
	public int GoToNoOptionLine;        
	public GameObject ActionOnYesAnswer;
	public GameObject ActionOnNoAnswer;
}