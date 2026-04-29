

using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{
	public LanguagesEnum Language { get; set; }
	public float FOV { get; set; }
	public int FPSLimit { get; set; }

	// НОВОЕ ПОЛЕ ДЛЯ ЧУВСТВИТЕЛЬНОСТИ МЫШИ
	public float MouseSensitivity { get; set; }

	public Dictionary<string, KeyCode> KeyBindings { get; set; } = new Dictionary<string, KeyCode>();
}