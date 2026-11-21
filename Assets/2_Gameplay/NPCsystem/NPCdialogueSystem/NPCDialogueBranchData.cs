using System;
using UnityEngine;

[Serializable]
public struct NPCdialogueBranchData
{
	public int DialogueBranchLine;      
	public int GoToNoOptionLine;
	public bool DoesIvanAnswerOnYes;
	public bool DoesIvanAnswerOnNo;
	public GameObject ActionOnYesAnswer;
	public GameObject ActionOnNoAnswer;
}