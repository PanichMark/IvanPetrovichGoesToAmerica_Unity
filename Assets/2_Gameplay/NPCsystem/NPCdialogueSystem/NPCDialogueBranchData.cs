using System;
using System.Collections.Generic;
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
	public List<NPCchangeStateStruct> NPCStateChangesOnYes;
	public List<NPCchangeStateStruct> NPCStateChangesOnNo;
}