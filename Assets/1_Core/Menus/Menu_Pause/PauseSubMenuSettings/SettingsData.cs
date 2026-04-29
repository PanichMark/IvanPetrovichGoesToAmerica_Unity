using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{

	public float FOV { get; set; }


	public Dictionary<string, KeyCode> KeyBindings { get; set; } = new Dictionary<string, KeyCode>();
}